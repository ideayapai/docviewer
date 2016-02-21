using System;
using System.Collections.Generic;
using System.Text;
using Krystalware.SlickUpload.Configuration;
using System.Security.Permissions;
using System.Collections.Specialized;
using System.IO;
using Krystalware.SlickUpload.Web.SessionStorage;
using System.Web;

namespace Krystalware.SlickUpload.Web
{
    internal class MimeUploadHandler : IMimePushHandler
    {
        UploadHttpRequest _httpRequest;
        UploadRequest _request;

        Stream _requestStream;
        byte[] _boundary;
        StringBuilder _textParts;

        //DateTime _lastDisconnectCheck = DateTime.MinValue;

        UploadedFile _currentFile;
        Stream _currentStream;
        long _fileLength;

        public MimeUploadHandler(UploadHttpRequest httpRequest, UploadRequest request)
        {
            //Current = this;

            _httpRequest = httpRequest;
            _request = request;

            _boundary = ExtractBoundary(_httpRequest.ContentType, _httpRequest.ContentEncoding);

            _requestStream = new RequestStream(_httpRequest.Worker);
        }

        public void ProcessRequest()
        {
            _textParts = new StringBuilder();

            PushReaderBase reader;

            if (string.Equals(_httpRequest.ContentType, "application/octet-stream", StringComparison.InvariantCultureIgnoreCase))
                reader = new SingleFilePushReader(_requestStream, this);
            else
                reader = new MimePushReader(_requestStream, this, _boundary, _httpRequest.ContentEncoding);

            // TODO: detect if ASP.NET has already read the request and throw an exception

            string data = _httpRequest.Headers["X-SlickUpload-Data"];

            if (!string.IsNullOrEmpty(data))
                MimeHelper.ParseQueryStringToDictionary(data, _request.Data);

            //try
            //{
            reader.Parse();
            /*}
            catch (DisconnectedException)
            {
                if (_currentStream != null)
                    _currentStream.Close();

                throw;
            }*/

            _httpRequest.InjectTextParts(_textParts.ToString());
        }

        UploadedFile CreateUploadedFile(NameValueCollection headers)
        {
            string contentDisposition = headers["content-disposition"];

            if (contentDisposition != null)
            {
                string[] dispositionParts = MimeHelper.QuotedSemiSplit(contentDisposition);

                if (dispositionParts.Length > 2)
                {
                    string fileName = MimeHelper.GetParts(dispositionParts[2], '=')[1];

                    if (fileName != "\"\"")
                    {
                        fileName = fileName.Replace("\"", string.Empty);

                        string sourceElement = MimeHelper.GetParts(dispositionParts[1], '=')[1].Replace("\"", string.Empty);

                        UploadedFile file = new UploadedFile(fileName, headers["content-type"], sourceElement, _request, _httpRequest);

                        return file;
                    }
                }
            }

            return null;
        }

        UploadedFile CreateSingleUploadedFile()
        {
            UploadedFile file = new UploadedFile(_httpRequest.Headers["X-File-Name"], _httpRequest.Headers["X-File-Content-Type"], _httpRequest.Headers["X-File-Source-Element"], _request, _httpRequest);

            return file;
        }

        /*UploadSettings LoadUploadSettings(string textParts)
        {
            // TODO: parse text parts for settings
        }*/

        public void BeginPart(NameValueCollection headers)
        {
            // Create a part for the file, or null if this isn't a valid file
            if (headers != null)
                _currentFile = CreateUploadedFile(headers);
            else
                _currentFile = CreateSingleUploadedFile();

            if (_currentFile != null)
            {
                //if (UploadSettings == null)
                //    LoadUploadSettings(_textParts.ToString());

                //SimpleLogger.Log("Starting file part", _uploadStatus.UploadId);
                //SimpleLogger.Log(_currentFile, _uploadStatus.UploadId);

                // Create a stream for the file
                if (_request.UploadFilter == null || _request.UploadFilter.ShouldHandleFile(_httpRequest, _currentFile))
                    _currentStream = _request.UploadStreamProvider.GetWriteStream(_currentFile);
                else
                    _currentStream = Stream.Null;

                _fileLength = 0;

                _request._uploadedFilesInternal.Add(_currentFile);

                SlickUploadContext.UpdateRequest(_request, true);
            }
            else
            {
                //SimpleLogger.Log("Starting non-file part", _uploadStatus.UploadId);

                // Write out the boundary start
                _textParts.Append(_httpRequest.ContentEncoding.GetString(_boundary) + "\r\n");

                // Write out the headers as textparts
                for (int i = 0; i < headers.Count; i++)
                {
                    _textParts.Append(headers.Keys[i] + ": " + headers[i] + "\r\n");
                }

                _textParts.Append("\r\n");

                // Add to our form collection
                string contentDisposition = headers["content-disposition"];

                if (contentDisposition != null)
                {
                    string[] dispositionParts = MimeHelper.QuotedSemiSplit(contentDisposition);

                    if (dispositionParts.Length == 2 && dispositionParts[1].Trim().StartsWith("name="))
                        _httpRequest.Form.Add(MimeHelper.GetParts(dispositionParts[1], '=')[1].Replace("\"", string.Empty), null);
                }
            }
        }

        public void PartData(ref byte[] data, int start, int length)
        {
            if (length > 0)
            {
                // If it's a UploadedFile
                if (_currentStream != null)
                {
                    if (_currentStream != Stream.Null)
                        _currentStream.Write(data, start, length);

                    _fileLength += length;
                }
                else
                {
                    string value = _httpRequest.ContentEncoding.GetString(data, start, length);

                    _textParts.Append(_httpRequest.ContentEncoding.GetString(data, start, length));

                    string currentValue = _httpRequest.Form[_httpRequest.Form.Count - 1];

                    if (string.IsNullOrEmpty(currentValue))
                        currentValue = value;
                    else
                        currentValue += value;

                    _httpRequest.Form[_httpRequest.Form.AllKeys[_httpRequest.Form.Count - 1]] = currentValue;
                }

                // TODO: fix ?
                /*if (SlickUploadConfiguration.StatusManager.UpdateInterval == 0 || DateTime.Now.Subtract(_lastDisconnectCheck).TotalSeconds > SlickUploadConfiguration.StatusManager.UpdateInterval)
                {
                    UploadStatus currentStatus = HttpUploadModule.StatusManager.GetUploadStatus(this._uploadStatus.UploadId);

                    if (currentStatus != null && currentStatus.State == UploadState.Terminated)
                        throw new UploadException(currentStatus.Reason);

                    _lastDisconnectCheck = DateTime.Now;
                }*/

                _request.Position = _requestStream.Position;

                SlickUploadContext.UpdateRequest(_request, false);
            }
        }

        public void EndPart(bool isLast, bool isComplete)
        {
            //SimpleLogger.Log("Part complete", _uploadStatus.UploadId);

            if (_currentStream != null)
            {
                UploadSession session = SlickUploadContext.SessionStorageProvider.GetSession(_request.UploadSessionId);
                
                if (session != null && session.State == UploadState.Uploading && (session.FailedRequests == null || Array.IndexOf<string>(session.FailedRequests, _request.UploadRequestId) == -1))
                {
                    _currentFile.ContentLength = _fileLength;

                    _request.UploadStreamProvider.CloseWriteStream(_currentFile, _currentStream, isComplete);

                    _currentStream = null;
                    _currentFile = null;
                }
                else
                {
                    CancelParse();

                    if (session != null && session.ErrorType == UploadErrorType.Cancelled)
                        throw new UploadCancelledException();
                    else
                        throw new UploadDisconnectedException();
                }
            }
            else
            {
                _textParts.Append("\r\n");

                if (_request.Data.Count == 0)
                {
                    string data = _httpRequest.Form["kw_uploadData"];

                    if (string.IsNullOrEmpty(data))
                        data = _httpRequest.Form["X-SlickUpload-Data"];

                    if (!string.IsNullOrEmpty(data))
                        MimeHelper.ParseQueryStringToDictionary(data, _request.Data);
                }
            }

            if (isLast)
            {
                if (_textParts.Length > 0)
                {
                    // Write out the boundary end
                    _textParts.Append(_httpRequest.ContentEncoding.GetString(_boundary) + "--\r\n\r\n");
                }
            }
        }

        public void CancelParse()
        {
            if (_currentStream != null)
            {
                try
                {
                    _request.UploadStreamProvider.CloseWriteStream(_currentFile, _currentStream, false);

                    // TODO: force close if exception?
                }
                finally
                {
                    _currentStream = null;
                }
            }
        }

        byte[] ExtractBoundary(string contentType, Encoding encoding)
        {
            int pos = contentType.IndexOf("boundary=");

            if (pos > 0)
                return encoding.GetBytes("--" + contentType.Substring(pos + 9));
            else
                return null;
        }
    }
}
