#if NET35
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Linq.Expressions;
using Krystalware.SlickUpload.Web.Controls;
using System.Web;
using Krystalware.SlickUpload.Web.Internal;

namespace Krystalware.SlickUpload.Web.Mvc
{
    /// <summary>
    /// Represents a renderable upload connector control.
    /// </summary>
    public class UploadConnector : MvcComponentBase, IUploadConnector
    {
        /// <inheritdoc />
        public bool? AllowPartialError { get; set; }
        /// <inheritdoc />
        public string UploadHandlerUrl { get; set; }
        /// <inheritdoc />
        public string CompleteHandlerUrl { get; set; }
        /// <inheritdoc />
        public string CompletionMethod { get; set; }
        /// <inheritdoc />
        public string CompletionBody { get; set; }
        /// <inheritdoc />
        public string CompletionContentType { get; set; }
        //public string UploadSessionId { get; set; }
        /// <inheritdoc />
        public string UploadFormId { get; set; }
        /// <inheritdoc />
        public bool? AutoUploadOnSubmit { get; set; }
        /// <inheritdoc />
        public bool? AutoCompleteAfterLastFile { get; set; }
        /// <inheritdoc />
        public string ConfirmNavigateDuringUploadMessage { get; set; }
        /// <inheritdoc />
        public string UploadProfile { get; set; }

        /// <inheritdoc />
        public string OnClientBeforeSessionStart { get; set; }
        /// <inheritdoc />
        public string OnClientUploadSessionStarted { get; set; }
        /// <inheritdoc />
        public string OnClientUploadFileStarted { get; set; }
        /// <inheritdoc />
        public string OnClientUploadFileEnded { get; set; }
        /// <inheritdoc />
        public string OnClientUploadSessionProgress { get; set; }
        /// <inheritdoc />
        public string OnClientBeforeSessionEnd { get; set; }
        /// <inheritdoc />
        public string OnClientUploadSessionEnded { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string> Data { get; set; }
    }
}
#endif