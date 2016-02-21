using System;
using System.Collections.Generic;
using System.Text;

namespace Krystalware.SlickUpload
{
    /// <summary>
    /// An enumeration of the possible states of an upload.
    /// </summary>
    public enum UploadState
    {
        /// <summary>
        /// The upload is still initializing and has not started yet.
        /// </summary>
        Initializing,
        /// <summary>
        /// The upload is in process.
        /// </summary>
        Uploading,
        /// <summary>
        /// The upload is complete.
        /// </summary>
        Complete,
        /// <summary>
        /// The upload has errored.
        /// </summary>
        Error
    }
    
    /// <summary>
    /// An enumeration of the possible error types generated during an upload.
    /// </summary>
    public enum UploadErrorType
    {
        /// <summary>
        /// No error.
        /// </summary>
        None,
        /// <summary>
        /// The maximum request length was exceeded.
        /// </summary>
        MaxRequestLengthExceeded,
        /// <summary>
        /// The client disconnected.
        /// </summary>
        Disconnected,
        /// <summary>
        /// The client cancelled the upload.
        /// </summary>
        Cancelled,
        /// <summary>
        /// The upload was terminated by an <see cref="IUploadFilter" />.
        /// </summary>
        UploadFilter,
        //InitializationTimeout,
        /// <summary>
        /// The request wasn't received, probably because the maximum request length was exceeded.
        /// </summary>
        RequestNotRecieved,
        /// <summary>
        /// A generic error occured.
        /// </summary>
        Other,
        /// <summary>
        /// The client marked this file as failed.
        /// </summary>
        FailedByClient
    }
}
