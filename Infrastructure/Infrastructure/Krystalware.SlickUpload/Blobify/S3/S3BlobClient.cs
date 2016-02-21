using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using Krystalware.SlickUpload.Blobify.Abstract;

namespace Krystalware.SlickUpload.Blobify.S3
{
    public class S3BlobClient : RestBlobClient<S3BlobInfo, S3BlobPartInfo>
    {
        public S3BlobClient(string accessKeyId, string secretAccessKey, string bucketName)
            : this(accessKeyId, secretAccessKey, bucketName, true)
        {
        }

        public S3BlobClient(string accessKeyId, string secretAccessKey, string bucketName, bool useHttps)
            : base(accessKeyId, new HMACSHA1(Encoding.UTF8.GetBytes(secretAccessKey)), bucketName, useHttps)
        {
            BlockSize = 5 * 1024 * 1024;
            UseSubDomains = false;
        }

        protected override string CustomHeaderPrefix { get { return "x-amz-"; } }
        protected override string AuthorizationPrefix { get { return "AWS"; } }
        protected override string ResponseXmlns { get { return "http://s3.amazonaws.com/doc/2006-03-01/"; } }

        //protected override string BlobRequestHost { get { return ContainerName + ".s3.amazonaws.com/"; } }
        protected override string BlobRequestHost { get { return UseSubDomains ? ContainerName + ".s3.amazonaws.com/" : "s3.amazonaws.com/"; } }
        //protected static string CanonicalizedGetContainerResourceFormat { get { return "{1}"; } }
        //protected static string CanonicalizedContainersResourceFormatFormat { get { return "/{0}/"; } }
        protected override string CanonicalizedBlobResourceFormat { get { return UseSubDomains ? "/{0}{1}{2}" : "{1}{2}"; } }
        protected override string RequestUrlFormat { get { return UseSubDomains ? "{0}{1}{3}{4}" : "{0}{1}{2}/{3}{4}"; } }

        public bool UseSubDomains { get; set; }

        protected override string StringToSignFormat
        {
            get
            {
                return
                    /* Request method */ "{0}\n" +
                    /* Content-MD5 */ "{1}\n" +
                    /* Content-Type */ "{2}\n" +
                    /* Date */ "{3}\n" +
                    /* Canonicalized headers */ "{5}" +
                    /* Canonicalized resource */ "{6}";
            }
        }

        protected override HttpWebRequest CreateInitializeBlobRequest(S3BlobInfo blobInfo)
        {
            return CreateRequest("POST", blobInfo.Name, null,
                new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("uploads", null)
                },
                GetBlobHeaders(blobInfo));
        }

        IEnumerable<KeyValuePair<string, string>> GetBlobHeaders(S3BlobInfo blobInfo)
        {
            foreach (KeyValuePair<string, string> pair in blobInfo.Metadata)
                yield return new KeyValuePair<string, string>(pair.Key.StartsWith(CustomHeaderPrefix) ? pair.Key : CustomHeaderPrefix + pair.Key, pair.Value);

            if (!string.IsNullOrEmpty(blobInfo.CacheControl))
                yield return new KeyValuePair<string, string>("Cache-Control", blobInfo.CacheControl);
            if (!string.IsNullOrEmpty(blobInfo.ContentDisposition))
                yield return new KeyValuePair<string, string>("Content-Disposition", blobInfo.ContentDisposition);
            if (!string.IsNullOrEmpty(blobInfo.ContentEncoding))
                yield return new KeyValuePair<string, string>("Content-Encoding", blobInfo.ContentEncoding);
            if (!string.IsNullOrEmpty(blobInfo.ContentType))
                yield return new KeyValuePair<string, string>("Content-Type", blobInfo.ContentType);
            if (blobInfo.Expires != null)
                yield return new KeyValuePair<string, string>("Expires", blobInfo.Expires.Value.ToUniversalTime().ToString("r"));
            if (blobInfo.CannedAcl != S3CannedAcl.Private)
                yield return new KeyValuePair<string, string>("x-amz-acl", GetAclString(blobInfo.CannedAcl));
            if (blobInfo.StorageClass != S3StorageClass.Standard)
                yield return new KeyValuePair<string, string>("x-amz-storage-class", GetStorageClassString(blobInfo.StorageClass));
        }

        private string GetStorageClassString(S3StorageClass storageClass)
        {
            switch (storageClass)
            {
                default:
                case S3StorageClass.Standard:
                    return "STANDARD";
                case S3StorageClass.ReducedRedundancy:
                    return "REDUCED_REDUNCANCY";
            }
        }

        private string GetAclString(S3CannedAcl cannedAcl)
        {
            switch (cannedAcl)
            {
                default:
                case S3CannedAcl.Private:
                    return "private";
                case S3CannedAcl.PublicRead:
                    return "public-read";
                case S3CannedAcl.PublicReadWrite:
                    return "public-read-write";
                case S3CannedAcl.AuthenticatedRead:
                    return "authenticated-read";
                case S3CannedAcl.BucketOwnerRead:
                    return "bucket-owner-read";
                case S3CannedAcl.BucketOwnerFullControl:
                    return "bucket-owner-full-control";
                case S3CannedAcl.LogDeliveryWrite:
                    return "log-delivery-write";
            }
        }

        protected override void ProcessInitializeBlobResponse(HttpWebResponse response, S3BlobInfo blobInfo)
        {
            XmlTextReader reader = new XmlTextReader(response.GetResponseStream())
            {
                WhitespaceHandling = WhitespaceHandling.Significant,
                Namespaces = false
            };

            reader.MoveToContent();

            reader.ReadStartElement("InitiateMultipartUploadResult");
            reader.ReadToFollowing("UploadId");

            blobInfo.UploadId = reader.ReadElementString();
        }

        protected override HttpWebRequest CreateCompleteBlobRequest(S3BlobInfo blobInfo, out Action<Stream> writeAction)
        {
            StringBuilder sb = new StringBuilder();
            //XmlTextWriter w = new XmlTextWriter(

            sb.Append("<CompleteMultipartUpload>\n");

            foreach (S3BlobPartInfo part in blobInfo.Parts)
            {
                sb.Append("<Part>\n");
                sb.Append("<PartNumber>");
                sb.Append(part.PartId);
                sb.Append("</PartNumber>\n");
                sb.Append("<ETag>");
                sb.Append(part.ETag);
                sb.Append("</ETag>\n");
                sb.Append("</Part>\n");
            }

            sb.Append("</CompleteMultipartUpload>\n");

            writeAction = (Stream s) =>
            {
                byte[] data = Encoding.UTF8.GetBytes(sb.ToString());

                s.Write(data, 0, data.Length);
            };

            return CreateRequest("POST", blobInfo.Name, sb.Length,
                new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("uploadId", blobInfo.UploadId)
                },
                null);
        }

        protected override void ProcessCompleteBlobResponse(HttpWebResponse response, S3BlobInfo blobInfo)
        {
            // TODO: do we have to call GetResponseStream()?
            // TODO: handle errors
        }

        protected override HttpWebRequest CreateAbortBlobRequest(S3BlobInfo blobInfo)
        {
            return CreateRequest("DELETE", blobInfo.Name, null,
                new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("uploadId", blobInfo.UploadId)
                },
                null);
        }

        protected override void ProcessAbortBlobResponse(HttpWebResponse response, S3BlobInfo blobInfo)
        {
            // TODO: do we have to call GetResponseStream()?
            // TODO: handle errors
        }

        protected override HttpWebRequest CreatePutPartRequest(S3BlobInfo blobInfo, S3BlobPartInfo partInfo)
        {
            return CreateRequest("PUT", blobInfo.Name, partInfo.Length,
                new KeyValuePair<string, string>[] {
                    new KeyValuePair<string, string>("partNumber", partInfo.PartId),
                    new KeyValuePair<string, string>("uploadId", blobInfo.UploadId)
                },
                null);
        }

        protected override void ProcessPutPartResponse(HttpWebResponse response, S3BlobInfo blobInfo, S3BlobPartInfo partInfo)
        {
            partInfo.ETag = response.Headers["ETag"];

            // TODO: do we have to call GetResponseStream()?
            // TODO: handle errors
        }

        protected override HttpWebRequest CreatePutBlobRequest(S3BlobInfo blobInfo)
        {
            return CreateRequest("PUT", blobInfo.Name, blobInfo.Length, null, GetBlobHeaders(blobInfo));
        }

        protected override void ProcessPutBlobResponse(HttpWebResponse response, S3BlobInfo blobInfo)
        {
            // TODO: do we have to call GetResponseStream()?
            // TODO: handle errors
        }

        protected override HttpWebRequest CreateDeleteBlobRequest(string name)
        {
            return CreateRequest("DELETE", name, null, null, null);
        }

        protected override void ProcessDeleteBlobResponse(HttpWebResponse response, string name)
        {
            // TODO: do we have to call GetResponseStream()?
            // TODO: handle errors
        }

        protected override HttpWebRequest CreateGetBlobRequest(string name)
        {
            return CreateRequest("GET", name, null, null, null);
        }

        protected override Stream ProcessGetBlobResponse(HttpWebResponse response, string name)
        {
            // TODO: do we have to call GetResponseStream()?
            // TODO: handle errors

            return response.GetResponseStream();
        }

        protected override void HandleRequestException(Exception ex)
        {
            WebException webEx = ex as WebException;

            if (webEx != null && webEx.Response != null)
            {
                string responseXml = null;
                string code = null;

                try
                {
                    using (StreamReader r = new StreamReader(webEx.Response.GetResponseStream()))
                        responseXml = r.ReadToEnd();

                    using (StringReader responseReader = new StringReader(responseXml))
                    {
                        XmlReader r = new XmlTextReader(responseReader);

                        r.ReadToDescendant("Code");

                        code = r.ReadElementString();

                        if (r.Name != "Message")
                            r.ReadToNextSibling("Message");

                        string message = r.ReadElementString();

                        throw new S3Exception(message, code, responseXml, ex);
                    }
                }
                catch (Exception parseEx)
                {
                    // TODO: figure out how to pass parseEx back
                    throw new S3Exception("Couldn't parse S3 error response.", code, responseXml, ex);
                }
            }
            else
            {
                throw new S3Exception("Error connecting to S3.", null, null, ex);
            }
        }
        
        protected override string GetNewPartId(S3BlobInfo blobInfo)
        {
            return (blobInfo.Parts.Count + 1).ToString();
        }
        
        protected override void AddStoreHeaders(SortedDictionary<string, string> headerDictionary)
        {
            headerDictionary["x-amz-date"] = DateTime.UtcNow.ToString("r");
        }
    }
}