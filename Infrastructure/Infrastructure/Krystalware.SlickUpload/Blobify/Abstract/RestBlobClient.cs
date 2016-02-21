using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Krystalware.SlickUpload.Blobify.Abstract
{
    public abstract class RestBlobClient<TBlobInfo, TBlobPartInfo> : BlobClient<TBlobInfo> where TBlobInfo : RestBlobInfo<TBlobPartInfo>, new() where TBlobPartInfo: RestBlobPartInfo, new()
    {
        protected bool UseSinglePartIfLengthIsKnown { get; set; }

        protected string AccountName { get; private set; }
        protected string ContainerName { get; private set; }

        readonly HMAC _hmac;

        private const string Unused = "unused";

        protected abstract string CustomHeaderPrefix { get; }
        protected abstract string AuthorizationPrefix { get; }
        protected abstract string ResponseXmlns { get; }

        protected abstract string BlobRequestHost { get; }
        protected abstract string StringToSignFormat { get; }
        //protected static string CanonicalizedGetContainerResourceFormat { get { return "{1}"; } }
        //protected static string CanonicalizedContainersResourceFormatFormat { get { return "/{0}/"; } }
        protected abstract string CanonicalizedBlobResourceFormat { get; }
        protected abstract string RequestUrlFormat { get; }

        protected bool UseHttps { get; private set; }

        public RestBlobClient(string accountName, HMAC hmac, string containerName, bool useHttps)
        {
            AccountName = accountName;
            _hmac = hmac;
            ContainerName = containerName;
            UseHttps = useHttps;
        }

        public override Stream GetPutBlobStream(TBlobInfo blobInfo)
        {
            return new PutBlobRestClientWriteStream(this, blobInfo);
        }

        public override void PutBlob(TBlobInfo blobInfo, Stream data)
        {
            if (blobInfo.Length != null && (blobInfo.Length <= BlockSize || UseSinglePartIfLengthIsKnown))
                PutSinglePartBlob(blobInfo, data);
            else
                PutMultiPartBlob(blobInfo, data);
        }

        public void PutSinglePartBlob(string name, Stream data)
        {
            PutSinglePartBlob(CreateBlobInfo(name, data), data);
        }
        
        public void PutSinglePartBlob(TBlobInfo blobInfo, Stream data)
        {
            if (blobInfo.Length == null)
                throw new InvalidOperationException("Length must be known for PutSinglePartBlob. Use PutMultiPartBlob instead.");

            try
            {
                HttpWebRequest request = CreatePutBlobRequest(blobInfo);

                request.AllowWriteStreamBuffering = false;
                request.ContentLength = data.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] buffer = new byte[8192];
                    int read;

                    while ((read = data.Read(buffer, 0, buffer.Length)) != 0)
                        requestStream.Write(buffer, 0, read);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    ProcessPutBlobResponse(response, blobInfo);
                }
            }
            catch (Exception ex)
            {
                HandleRequestException(ex);

                throw;
            }
        }

        public void PutMultiPartBlob(string name, Stream data)
        {
            PutMultiPartBlob(CreateBlobInfo(name, data), data);
        }

        public void PutMultiPartBlob(TBlobInfo blobInfo, Stream data)
        {          
            try
            {
                InitializeBlob(blobInfo);

                if (blobInfo.Length != null)
                {
                    while (data.Position < data.Length)
                        PutPart(blobInfo, data, Math.Min(BlockSize, (int)(data.Length - data.Position)));
                }
                else
                {
                    // TODO: buffer pooling?
                    using (MemoryStream bufferStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[8192];
                        int read;

                        // TODO: should we use everything that memorystream allocates?
                        while ((read = data.Read(buffer, 0, Math.Min(buffer.Length, BlockSize - (int)bufferStream.Position))) != 0)
                        {
                            bufferStream.Write(buffer, 0, read);

                            if (bufferStream.Position == BlockSize)
                            {
                                PutPart(blobInfo, bufferStream.GetBuffer(), 0, (int)bufferStream.Position);

                                bufferStream.Position = 0;
                            }
                        }

                        if (bufferStream.Position > 0)
                            PutPart(blobInfo, bufferStream.GetBuffer(), 0, (int)bufferStream.Position);
                    }
                }

                CompleteBlob(blobInfo);
            }
            catch
            {
                if (blobInfo != null)
                    AbortBlob(blobInfo);

                throw;
            }
        }

        protected void InitializeBlob(TBlobInfo blobInfo)
        {
            try
            {
                HttpWebRequest request = CreateInitializeBlobRequest(blobInfo);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    ProcessInitializeBlobResponse(response, blobInfo);                
                }
            }
            catch (Exception ex)
            {
                HandleRequestException(ex);

                throw;
            }
        }

        protected void CompleteBlob(TBlobInfo blobInfo)
        {
            try
            {
                Action<Stream> writeAction;
                HttpWebRequest request = CreateCompleteBlobRequest(blobInfo, out writeAction);

                if (writeAction != null)
                {
                    using (Stream s = request.GetRequestStream())
                        writeAction(s);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    ProcessCompleteBlobResponse(response, blobInfo);
                }
            }
            catch (Exception ex)
            {
                HandleRequestException(ex);

                throw;
            }
        }

        protected void AbortBlob(TBlobInfo blobInfo)
        {
            try
            {
                HttpWebRequest request = CreateAbortBlobRequest(blobInfo);

                if (request != null)
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        ProcessAbortBlobResponse(response, blobInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleRequestException(ex);

                throw;
            }
        }

        public override void DeleteBlob(string name)
        {
            try
            {
                HttpWebRequest request = CreateDeleteBlobRequest(name);

                if (request != null)
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        ProcessDeleteBlobResponse(response, name);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleRequestException(ex);

                throw;
            }
        }

        public override Stream GetBlob(string name)
        {
            try
            {
                HttpWebRequest request = CreateGetBlobRequest(name);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return ProcessGetBlobResponse(response, name);
                }
            }
            catch (Exception ex)
            {
                HandleRequestException(ex);

                throw;
            }
        }

        protected void PutPart(TBlobInfo blobInfo, byte[] data, int offset, int count)
        {
            // TODO: is this dynamism slowing things down?
            PutPartInternal(blobInfo, (s) => s.Write(data, offset, count), count);
        }

        protected void PutPart(TBlobInfo blobInfo, Stream data, int count)
        {
            // TODO: is this dynamism slowing things down?
            PutPartInternal(blobInfo,
                (s) =>
                {
                    byte[] buffer = new byte[8192];
                    int read;
                    int maxPosition = (int)data.Position + count;

                    // TODO: should we use everything that memorystream allocates?
                    while ((read = data.Read(buffer, 0, Math.Min(buffer.Length, maxPosition - (int)data.Position))) != 0)
                    {
                        s.Write(buffer, 0, read);
                    }
                },
                count);
        }

        protected void PutPartInternal(TBlobInfo blobInfo, Action<Stream> writeAction, int count)
        {
            try
            {
                TBlobPartInfo partInfo = new TBlobPartInfo()
                {
                    PartId = GetNewPartId(blobInfo),
                    Length = count
                };

                HttpWebRequest request = CreatePutPartRequest(blobInfo, partInfo);

                request.AllowWriteStreamBuffering = false;
                request.ContentLength = count;

                using (Stream s = request.GetRequestStream())
                {
                    writeAction(s);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    ProcessPutPartResponse(response, blobInfo, partInfo);
                }

                blobInfo.Parts.Add(partInfo);
            }
            catch (Exception ex)
            {
                HandleRequestException(ex);

                throw;
            }
        }

        protected virtual void HandleRequestException(Exception ex)
        { }

        protected HttpWebRequest CreateRequest(string method, string objectKey, long? length, IEnumerable<KeyValuePair<string, string>> parameters, IEnumerable<KeyValuePair<string, string>> headers)
        {
            StringBuilder parameterSb = new StringBuilder();

            if (parameters != null)
            {
                foreach (KeyValuePair<string, string> parameter in parameters)
                {
                    if (parameterSb.Length == 0)
                        parameterSb.Append('?');
                    else
                        parameterSb.Append('&');

                    parameterSb.Append(parameter.Key);

                    if (!string.IsNullOrEmpty(parameter.Value))
                    {
                        parameterSb.Append('=');

                        parameterSb.Append(parameter.Value);
                    }
                }
            }

            // TODO: allow https
            string authority = UseHttps ? "https://" : "http://";

            string url = string.Format(RequestUrlFormat, authority, BlobRequestHost, ContainerName, objectKey, parameterSb.ToString());

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = method.ToUpperInvariant();
            request.Timeout = int.MaxValue;

            // http://msdn.microsoft.com/en-us/library/system.net.servicepoint.connectionlimit.aspx
            // http://support.microsoft.com/?id=821268
            // http://blogs.msdn.com/b/tess/archive/2006/02/23/537681.aspx
            // TODO: make this configurable
            request.ServicePoint.ConnectionLimit = 100;

            // The following is necessary to prevent the Cache-Control header from
            // getting sent up and preventing successful download when not behind
            // a proxy.
            request.Headers[HttpRequestHeader.CacheControl] = Unused;
            // The following may be necessary to prevent the If-None-Match and
            // If-Modified-Since headers from getting sent up and preventing
            // successful calls due to an Authorization header mis-match.
            /*if (NeedsBogusIfNoneMatch)
            {
                request.Headers[HttpRequestHeader.IfNoneMatch] = Unused;
            }*/

            SortedDictionary<string, string> headerDictionary = new SortedDictionary<string, string>();

            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                    headerDictionary.Add(header.Key, header.Value);
            }

            AddStoreHeaders(headerDictionary);

            StringBuilder canonicalizedHeadersSb = new StringBuilder();

            foreach (KeyValuePair<string, string> header in headerDictionary)
            {
                if (header.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                    request.ContentType = header.Value;
                else
                    request.Headers[header.Key] = header.Value;

                if (header.Key.StartsWith(CustomHeaderPrefix, StringComparison.InvariantCultureIgnoreCase))
                {
                    canonicalizedHeadersSb.Append(header.Key);
                    canonicalizedHeadersSb.Append(':');
                    canonicalizedHeadersSb.Append(header.Value);
                    canonicalizedHeadersSb.Append('\n');
                }
            }


            string canonicalizedResource =
                string.Format(
                    CultureInfo.InvariantCulture,
                    CanonicalizedBlobResourceFormat.ToLowerInvariant(),
                    ContainerName,
                    request.RequestUri.AbsolutePath,
                    request.RequestUri.Query/*,
                    string.Concat(
                        parameters
                            .OrderBy(p => p.Name)
                            .Select(p => "\n" + p.Name + ":" + p.Value)
                            .ToArray())*/);

            /*DateTime now = DateTime.Now;

            request.Date
            request.Headers[HttpRequestHeader.Date] = DateTime.UtcNow.ToString("r", CultureInfo.InvariantCulture);

            // Create Authorization header
            var stringToSign =
                string.Format(
                    CultureInfo.InvariantCulture,
                    StringToSignFormat,
                    request.Method,
                    null,
                    null,
                    null, //(length.HasValue ? length.Value.ToString(CultureInfo.InvariantCulture) : null),
                    DateTime.UtcNow.ToString("r", CultureInfo.InvariantCulture),
                    null,
                    null,
                    null,
                    null,
                    null, //NeedsBogusIfNoneMatch ? Unused : null,
                    null,
                    null,
                    null,
                    canonicalizedResource); 
            var signature = Convert.ToBase64String(_hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            var authorization = "AWS" + " " + AccountName + ":" + signature;
            request.Headers[HttpRequestHeader.Authorization] = authorization;*/

            string stringToSign =
                string.Format(
                    CultureInfo.InvariantCulture,
                    StringToSignFormat,
                    request.Method,
                    request.Headers[HttpRequestHeader.ContentMd5],
                    request.ContentType,
                    null,
                    (length.HasValue ? length.Value.ToString(CultureInfo.InvariantCulture) : null),
                    /*NeedsBogusIfNoneMatch ? Unused : null,*/
                    canonicalizedHeadersSb.ToString(),
                    canonicalizedResource);

            string signature = Convert.ToBase64String(_hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

            request.Headers[HttpRequestHeader.Authorization] = AuthorizationPrefix + " " + AccountName + ":" + signature;

            return request;
        }

        protected abstract void AddStoreHeaders(SortedDictionary<string, string> headerDictionary);
        protected abstract string GetNewPartId(TBlobInfo blobInfo);

        protected abstract HttpWebRequest CreateInitializeBlobRequest(TBlobInfo blobInfo);
        protected abstract HttpWebRequest CreateCompleteBlobRequest(TBlobInfo blobInfo, out Action<Stream> writeAction);
        protected abstract HttpWebRequest CreateAbortBlobRequest(TBlobInfo blobInfo);
        protected abstract HttpWebRequest CreatePutPartRequest(TBlobInfo blobInfo, TBlobPartInfo partInfo);
        protected abstract HttpWebRequest CreatePutBlobRequest(TBlobInfo blobInfo);
        protected abstract HttpWebRequest CreateDeleteBlobRequest(string name);
        protected abstract HttpWebRequest CreateGetBlobRequest(string name);

        protected abstract void ProcessInitializeBlobResponse(HttpWebResponse response, TBlobInfo blobInfo);
        protected abstract void ProcessCompleteBlobResponse(HttpWebResponse response, TBlobInfo blobInfo);
        protected abstract void ProcessAbortBlobResponse(HttpWebResponse response, TBlobInfo blobInfo);
        protected abstract void ProcessPutPartResponse(HttpWebResponse response, TBlobInfo blobInfo, TBlobPartInfo partInfo);
        protected abstract void ProcessPutBlobResponse(HttpWebResponse response, TBlobInfo blobInfo);
        protected abstract void ProcessDeleteBlobResponse(HttpWebResponse response, string name);
        protected abstract Stream ProcessGetBlobResponse(HttpWebResponse response, string name);

        /*protected abstract void PutPart(string name, string container, Stream data);
        protected abstract void PutPart(string name, string container, Action<Stream> writeAction, long length);*/

        public class PutBlobRestClientWriteStream : BlockWriteStreamBase
        {
            RestBlobClient<TBlobInfo, TBlobPartInfo> _client;
            TBlobInfo _blobInfo;

            internal PutBlobRestClientWriteStream(RestBlobClient<TBlobInfo, TBlobPartInfo> client, TBlobInfo blobInfo)
                : base(client.BlockSize)
            {
                // TODO: implement length
                _client = client;
                _blobInfo = blobInfo;
            }

            protected override void WriteBlock(byte[] data, int offset, int count)
            {
                _client.PutPart(_blobInfo, data, offset, count);
            }

            protected override void Initialize()
            {
                _client.InitializeBlob(_blobInfo);
            }

            protected override void CompleteInternal()
            {
                _client.CompleteBlob(_blobInfo);
            }

            protected override void AbortInternal()
            {
                _client.AbortBlob(_blobInfo);
            }
        }
    }
}
