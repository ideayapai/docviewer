using System.Collections.Generic;

namespace Krystalware.SlickUpload.Web.Internal
{
    interface IUploadConnector : IRenderableComponent
    {
        /// <summary>
        /// Gets or sets a value that specifies whether to allow an upload session to complete successfully even when some of the files errored.
        /// </summary>
        /// <default-value>null</default-value>
        bool? AllowPartialError { get; }
        /// <summary>
        /// Gets or sets the URL to which to POST the upload.
        /// </summary>
        /// <default-value>"~/SlickUpload.axd"</default-value>
        string UploadHandlerUrl { get; }
        /// <summary>
        /// Gets or sets the URL to which to POST after the upload step is finished to complete the upload.
        /// </summary>
        string CompleteHandlerUrl { get; }
        /// <summary>
        /// Gets or sets the HTTP method to use for the completion request, either "GET" or "POST".
        /// </summary>
        string CompletionMethod { get; }
        /// <summary>
        /// Gets or sets the request body to send for the completion request.
        /// </summary>
        string CompletionBody { get; }
        /// <summary>
        /// Gets or sets the Content-Type header to set for the completion request.
        /// </summary>
        string CompletionContentType { get; }

        //string UploadSessionId { get; }
        /// <summary>
        /// Gets or sets the HTML element id of the upload form to use.
        /// </summary>
        string UploadFormId { get; }
        /// <summary>
        /// Gets or sets a boolean that specifies whether to automatically start an upload when the associated form is posted back.
        /// </summary>
        /// <default-value>true if UploadFormId is specified, otherwise false</default-value>
        bool? AutoUploadOnSubmit { get; }
        /// <summary>
        /// Gets or sets a boolean that specifies whether to automatically complete an upload after the last file has been uploaded.
        /// </summary>
        /// <default-value>true</default-value>
        bool? AutoCompleteAfterLastFile { get; }
        /// <summary>
        /// Gets or sets a message to display if the user attempts to navigate away during an upload, confirming the user wants to cancel.
        /// </summary>
        string ConfirmNavigateDuringUploadMessage { get; }
        /// <summary>
        /// Gets or sets the name of the upload profile to use.
        /// </summary>
        string UploadProfile { get; }

        /// <summary>
        /// Gets or sets a javascript function to call before an upload session starts.
        /// </summary>
        string OnClientBeforeSessionStart { get; }
        /// <summary>
        /// Gets or sets a javascript function to call when an upload session has been started.
        /// </summary>
        string OnClientUploadSessionStarted { get; }
        /// <summary>
        /// Gets or sets a javascript function to call when a file upload has been started.
        /// </summary>
        string OnClientUploadFileStarted { get; }
        /// <summary>
        /// Gets or sets a javascript function to call when a file upload has ended.
        /// </summary>
        string OnClientUploadFileEnded { get; }
        /// <summary>
        /// Gets or sets a javascript function to call on upload session progress updates.
        /// </summary>
        string OnClientUploadSessionProgress { get; }
        /// <summary>
        /// Gets or sets a javascript function to call before an upload session ends.
        /// </summary>
        string OnClientBeforeSessionEnd { get; }
        /// <summary>
        /// Gets or sets a javascript function to call when an upload session has ended.
        /// </summary>
        string OnClientUploadSessionEnded { get; }

        /// <summary>
        /// Gets or sets the data dictionary to send with the upload.
        /// </summary>
        Dictionary<string, string> Data { get; }
    }
}
