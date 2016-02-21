using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Krystalware.SlickUpload.Web;

namespace Krystalware.SlickUpload
{
    /// <summary>
    /// Provides information about a file that has been uploaded. All files are streamed to an upload location as they arrive. Each <see cref="UploadedFile" /> instance contains information about a single uploaded file.
    /// </summary>
    [Serializable]
    public class UploadedFile
    {
        /// <summary>
        /// Gets the file name of the uploaded file as it was on the client machine.
        /// </summary>
        public string ClientName { get; private set; }
        /// <summary>
        /// Gets the server location where this file was written
        /// </summary>
        public string ServerLocation { get; set; }
        /// <summary>
        /// Gets the MIME content type of the uploaded file, as passed by the client browser.
        /// </summary>
        public string ContentType { get; private set; }
        /// <summary>
        /// Gets the name of the source form input element where the file was selected.
        /// </summary>
        public string SourceElement { get; private set; }
        /// <summary>
        /// Gets the length (in bytes) of the uploaded file.
        /// </summary>
        public long ContentLength { get; internal set; }

        /// <summary>
        /// Gets the ID of the <see cref="FileSelector" /> used to select this file, or null if no file selector was used.
        /// </summary>
        public string FileSelectorId
        {
            get
            {
                int endPos = SourceElement.LastIndexOf('_');

                if (endPos != -1)
                {
                    int startPos = SourceElement.LastIndexOf('_', endPos - 1);

                    if (startPos != -1)
                    {
                        /*if (SourceElement.Substring(startPos + 1, endPos - startPos - 1) == "html")
                        {
                            endPos = startPos;

                            startPos = SourceElement.LastIndexOf('_', endPos - 1);
                        }*/

                        return SourceElement.Substring(startPos + 1, endPos - startPos - 1);
                    }
                    else
                        return SourceElement.Substring(0, endPos);
                }

                return null;
            }
        }
        
        /// <summary>
        /// Gets the <see cref="UploadRequest" /> that contained this file.
        /// </summary>
        public UploadRequest UploadRequest { get; private set; }

        /// <summary>
        /// Gets the current <see cref="UploadHttpRequest" /> that contains this file, or null if the upload has already been completed.
        /// </summary>
        public UploadHttpRequest UploadHttpRequest { get; private set; }

        /// <summary>
        /// Gets a dictionary of information about the file, including form values for the file and information set by the upload process.
        /// </summary>
        public Dictionary<string, string> Data { get; private set; }

        internal UploadedFile(string fileName, string contentType, string sourceElement, UploadRequest request, UploadHttpRequest httpRequest)
        {
            // TODO: ensure this is cross-platform compatible
            ClientName = GetFileName(fileName);
            //_fileName = fileName;
            ContentType = contentType;
            SourceElement = sourceElement;

            Data = new Dictionary<string, string>();

            UploadRequest = request;
            UploadHttpRequest = httpRequest;
        }

        string GetFileName(string path)
        {
            int lastBackSlashPos = path.LastIndexOf('\\');
            //int lastForwardSlashPos = path.LastIndexOf('/');
            // TODO: worry about mac/unix paths with forward slash?
            if (lastBackSlashPos != -1)
                return path.Substring(lastBackSlashPos + 1);
            else
                return path;
        }

        internal UploadedFile(object[] values, UploadRequest request)
        {
            if (values == null || values.Length != 6)
                throw new FormatException("Invalid deserialization data.");

            ClientName = (string)values[0];
            ServerLocation = (string)values[1];
            ContentType = (string)values[2];
            SourceElement = (string)values[3];
            ContentLength = (long)values[4];

            Data = SerializationHelper.DeserializeDictionary(values[5]);

            UploadRequest = request;
        }

        internal object[] ToObjectArray()
        {
            return new object[]
            {
                ClientName,
                ServerLocation,
                ContentType,
                SourceElement,
                ContentLength,
                SerializationHelper.SerializeDictionary(Data)
            };
        }
    }
}
