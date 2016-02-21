using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.IO;
using System.Web;
using System.Collections.Specialized;
using Krystalware.SlickUpload.Configuration;

namespace Krystalware.SlickUpload
{
    /// <summary>
    /// Contains information about the current status of an upload session. An upload session combines all <see cref="UploadRequest" />s
    /// with the same <see cref="UploadSession.UploadSessionId"/>. Properties are updated in real time as the upload progresses.
    /// </summary>
    public class UploadSession
    {
        //DateTime _startDate;
        //string _uploadSessionId;
        string _uploadProfileKey;

        UploadProfileElement _uploadProfile;
        List<UploadRequest> _uploadRequests;
        List<UploadedFile> _uploadedFiles;
        List<Exception> _allErrors;
        Dictionary<string, string> _processingStatus;
        Dictionary<string, string> _data;

        /// <summary>
        /// Gets the unique upload session id for this upload session.
        /// </summary>
        public string UploadSessionId { get; private set; }

        /// <summary>
        /// Gets the upload profile used for this session.
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
        /// Gets the <see cref="UploadErrorType" /> for this upload session.
        /// </summary>
        public UploadErrorType ErrorType { get; internal set; }
        /// <summary>
        /// Gets the <see cref="UploadState" /> for this upload session.
        /// </summary>
        public UploadState State { get; internal set; }

        /// <summary>
        /// Gets the <see cref="DateTime" /> that indicates when this upload session started.
        /// </summary>
        public DateTime Started { get; private set; }

        /// <summary>
        /// Gets the ID of the <see cref="UploadConnector" /> that sent this upload.
        /// </summary>
        // TODO: should this be set earlier?
        // TODO: should this be serialized to session store?
        public string SourceUploadConnectorId { get; internal set; }

        internal string[] FailedRequests { get; set; }
        internal string[] CancelledRequests { get; set; }

        /// <summary>
        /// Gets a dictionary that contains the current processing status information.
        /// </summary>
        public Dictionary<string, string> ProcessingStatus
        {
            get
            {
                if (_processingStatus == null)
                    _processingStatus = new Dictionary<string, string>();

                return _processingStatus;
            }
        }

        /// <summary>
        /// Gets a dictionary of information about the upload session, including data set using the SlickUpload.Data or UploadConnector.Data properties.
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

        internal UploadSession(string uploadSessionId, string uploadProfileKey)
        {
            UploadSessionId = uploadSessionId;
            _uploadProfileKey = uploadProfileKey;

            Started = DateTime.Now;
        }

        /// <summary>
        /// Gets a collection of the <see cref="UploadRequest" />s for this upload session.
        /// </summary>
        public ICollection<UploadRequest> UploadRequests
        {
            get
            {
                List<UploadRequest> uploadRequests = _uploadRequests ?? new List<UploadRequest>(SlickUploadContext.SessionStorageProvider.GetRequestsForSession(UploadSessionId));

                if (_uploadRequests == null && (State == UploadState.Complete || State == UploadState.Error))
                    _uploadRequests = uploadRequests;
                
                return uploadRequests.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets a collection of the <see cref="Exception" />s that occurred during this upload session.
        /// </summary>
        public ICollection<Exception> AllErrors
        {
            get
            {
                List<Exception> allErrors = _allErrors ?? new List<Exception>();

                if (_allErrors == null)
                {
                    foreach (UploadRequest request in UploadRequests)
                    {
                        if (request.Error != null)
                            allErrors.Add(request.Error);

                        if (State == UploadState.Complete || State == UploadState.Error)
                            _allErrors = allErrors;
                    }
                }

                return allErrors.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets a collection of the <see cref="UploadedFile" />s for this upload session.
        /// </summary>
        public ICollection<UploadedFile> UploadedFiles
        {
            get
            {
                List<UploadedFile> uploadedFiles = _uploadedFiles ?? new List<UploadedFile>();

                if (_uploadedFiles == null)
                {
                    if (ErrorType != UploadErrorType.Cancelled)
                    {
                        foreach (UploadRequest request in UploadRequests)
                        {
                            if (request.State != UploadState.Error)
                                uploadedFiles.AddRange(request._uploadedFilesInternal);
                        }
                    }

                    if (State == UploadState.Complete || State == UploadState.Error)
                        _uploadedFiles = uploadedFiles;
                }

                return uploadedFiles.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets an error message summary for this upload session, or null if no errors occurred.
        /// </summary>
        public string ErrorSummary
        {
            get
            {
                if (State == UploadState.Error)
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append(ErrorType.ToString());

                    if (AllErrors.Count > 0)
                    {
                        sb.Append(" - ");

                        List<Exception> uniqueErrors = new List<Exception>();

                        foreach (Exception ex in AllErrors)
                        {
                            bool isUnique = true;

                            foreach (Exception uniqueEx in uniqueErrors)
                            {
                                if (ex.GetType() == uniqueEx.GetType() && ex.Message == uniqueEx.Message)
                                {
                                    isUnique = false;

                                    break;
                                }
                            }

                            if (isUnique)
                                uniqueErrors.Add(ex);
                        }

                        for (int i = 0; i < uniqueErrors.Count; i++)
                        {
                            if (i > 0)
                                sb.Append(", ");

                            sb.Append(uniqueErrors[i].GetType().Name + ": " + uniqueErrors[i].Message);
                        }
                    }

                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
        }

        internal UploadSession(object[] values)
        {
            if (values == null || values.Length != 9)
                throw new FormatException("Invalid deserialization data.");

            UploadSessionId = (string)values[0];
            _uploadProfileKey = (string)values[1];
            State = (UploadState)values[2];
            ErrorType = (UploadErrorType)values[3];
            Started = (DateTime)values[4];
            FailedRequests = values[5] as string[];
            CancelledRequests = values[6] as string[];

            _processingStatus = SerializationHelper.DeserializeDictionary(values[7]);
            _data = SerializationHelper.DeserializeDictionary(values[8]);
        }

        private object[] ToObjectArray()
        {
            object[] values = new object[]
                {
                    UploadSessionId,
                    _uploadProfileKey,
                    State,
                    ErrorType,
                    Started,
                    FailedRequests,
                    CancelledRequests,
                    SerializationHelper.SerializeDictionary(_processingStatus),
                    SerializationHelper.SerializeDictionary(_data)
                };

            return values;
        }

        /// <summary>
        /// Returns this <see cref="UploadSession" /> instance serialized into a string.
        /// </summary>
        /// <returns>The serialized string.</returns>
        public string Serialize()
        {
            ObjectStateFormatter formatter = new ObjectStateFormatter();

            return formatter.Serialize(ToObjectArray());
        }

        /// <summary>
        /// Deserializes a string generated by <see cref="UploadSession.Serialize" /> into an <see cref="UploadSession" /> instance.
        /// </summary>
        /// <param name="value">The string to deserialize.</param>
        /// <returns>An <see cref="UploadSession" /> instance.</returns>
        public static UploadSession Deserialize(string value)
        {
            ObjectStateFormatter formatter = new ObjectStateFormatter();

            object[] values = formatter.Deserialize(value) as object[];

            if (values != null && values.Length > 0)
                return new UploadSession(values);
            else
                return null;
        }       
    }
}
