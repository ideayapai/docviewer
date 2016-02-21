using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Krystalware.SlickUpload.Web;
using Krystalware.SlickUpload.Configuration;
using Krystalware.SlickUpload.Storage;
using System.Web.UI;
using System.IO;
using System.Collections.ObjectModel;

namespace Krystalware.SlickUpload
{   
    /// <summary>
    /// Contains information about the current status of an upload request. Properties are updated in real time as the upload progresses.
    /// </summary>
    public class UploadRequest
    {
        string _uploadProfileKey;

        UploadProfileElement _uploadProfile;
        IUploadStreamProvider _uploadStreamProvider;
        IUploadFilter _uploadFilter;

        internal List<UploadedFile> _uploadedFilesInternal;
        ReadOnlyCollection<UploadedFile> _publicUploadedFiles;
        Dictionary<string, string> _data;

        /// <summary>
        /// Gets the upload profile used for this request.
        /// </summary>
        public UploadProfileElement UploadProfile
        {
            get
            {
                if (_uploadProfile == null)
                    _uploadProfile = SlickUploadContext.Config.UploadProfiles.GetUploadProfileElement(_uploadProfileKey, true);

                return _uploadProfile;
            }
        }

        /// <summary>
        /// Gets the <see cref="IUploadStreamProvider" /> used for this request.
        /// </summary>
        public IUploadStreamProvider UploadStreamProvider
        {
            get
            {
                if (_uploadStreamProvider == null)
                    _uploadStreamProvider = UploadProfile.UploadStreamProvider.Create();

                return _uploadStreamProvider;
            }
        }

        /// <summary>
        /// Gets the <see cref="IUploadFilter" /> used for this request.
        /// </summary>
        public IUploadFilter UploadFilter
        {
            get
            {
                if (_uploadFilter == null)
                    _uploadFilter = UploadProfile.UploadFilter.Create();

                return _uploadFilter;
            }
        }

        /// <summary>
        /// Gets the collection of files uploaded during this upload request.
        /// </summary>
        public ICollection<UploadedFile> UploadedFiles
        {
            get
            {
                if (_publicUploadedFiles == null)
                    _publicUploadedFiles = new ReadOnlyCollection<UploadedFile>(_uploadedFilesInternal);

                return _publicUploadedFiles;
            }
        }

        /// <summary>
        /// Gets the unique upload request id for this upload request.
        /// </summary>
        public string UploadRequestId { get; private set; }
        /// <summary>
        /// Gets the unique upload session id for this upload request.
        /// </summary>
        public string UploadSessionId { get; private set; }

        /// <summary>
        /// Gets the current position in this upload request.
        /// </summary>
        public long Position { get; internal set; }
        /// <summary>
        /// Gets the length of the entire current upload request.
        /// </summary>
        public long ContentLength { get; private set; }

        /// <summary>
        /// Gets the <see cref="UploadErrorType" /> for this upload request.
        /// </summary>
        public UploadErrorType ErrorType { get; internal set; }
        /// <summary>
        /// Gets the <see cref="UploadState" /> for this upload request.
        /// </summary>
        public UploadState State { get; internal set; }

        /// <summary>
        /// Gets the <see cref="Exception" /> for this upload request, or null if no exception occured.
        /// </summary>
        public Exception Error { get; internal set; }

        /// <summary>
        /// Gets the <see cref="DateTime" /> that indicates when this upload request started.
        /// </summary>
        public DateTime Started { get; private set; }
        /// <summary>
        /// Gets the <see cref="DateTime" /> that indicates when this upload request information was last updated.
        /// </summary>
        public DateTime LastUpdated { get; internal set; }

        /// <summary>
        /// Gets a dictionary of information about the upload request, including data set using the SlickUpload.Data or UploadConnector.Data properties.
        /// </summary>
        public Dictionary<string, string> Data
        {
            get
            {
                if (_data == null)
                    _data = new Dictionary<string, string>();

                return _data;
            }
        }

        internal UploadRequest(string uploadSessionId, string uploadRequestId, long contentLength, string uploadProfileKey)
        {
            UploadSessionId = uploadSessionId;
            UploadRequestId = uploadRequestId;

            ContentLength = contentLength;

            _uploadProfileKey = uploadProfileKey;

            _uploadedFilesInternal = new List<UploadedFile>();

            Started = LastUpdated = DateTime.Now;
        }

        internal UploadRequest(object[] values)
        {
            if (values == null || values.Length != 12)
                throw new FormatException("Invalid deserialization data.");

            UploadRequestId = (string)values[0];
            UploadSessionId = (string)values[1];
            Position = (long)values[2];
            ContentLength = (long)values[3];
            ErrorType = (UploadErrorType)values[4];
            State = (UploadState)values[5];
            Error = (Exception)values[6];
            Started = (DateTime)values[7];
            LastUpdated = (DateTime)values[8];
            _uploadProfileKey = (string)values[9];

            object[] uploadedFiles = (object[])values[10];

            _uploadedFilesInternal = new List<UploadedFile>();

            if (uploadedFiles != null && uploadedFiles.Length > 0)
            {
                for (int i = 0; i < uploadedFiles.Length; i++)
                    _uploadedFilesInternal.Add(new UploadedFile((object[])uploadedFiles[i], this));
            }

            _data = SerializationHelper.DeserializeDictionary(values[11]);
        }

        private object[] ToObjectArray()
        {
            object[] uploadedFiles = null;
            
            if (_uploadedFilesInternal != null && _uploadedFilesInternal.Count > 0)
            {
                uploadedFiles = new object[_uploadedFilesInternal.Count];

                for (int i = 0; i < uploadedFiles.Length; i++)
                    uploadedFiles[i] = _uploadedFilesInternal[i].ToObjectArray();
            }

            return new object[]
            {
                UploadRequestId,
                UploadSessionId,
                Position,
                ContentLength,
                ErrorType,
                State,
                Error,
                Started,
                LastUpdated,
                _uploadProfileKey,
                uploadedFiles,
                SerializationHelper.SerializeDictionary(_data)
            };
        }

        /// <summary>
        /// Returns this <see cref="UploadRequest" /> instance serialized into a string.
        /// </summary>
        /// <returns>The serialized string.</returns>
        public string Serialize()
        {
            ObjectStateFormatter formatter = new ObjectStateFormatter();

            return formatter.Serialize(ToObjectArray());            
        }

        /// <summary>
        /// Deserializes a string generated by <see cref="UploadRequest.Serialize" /> into an <see cref="UploadRequest" /> instance.
        /// </summary>
        /// <param name="value">The string to deserialize.</param>
        /// <returns>An <see cref="UploadRequest" /> instance.</returns>
        public static UploadRequest Deserialize(string value)
        {
            ObjectStateFormatter formatter = new ObjectStateFormatter();

            object[] values = formatter.Deserialize(value) as object[];

            if (values != null && values.Length > 0)
                return new UploadRequest(values);
            else
                return null;
        }
    }
}