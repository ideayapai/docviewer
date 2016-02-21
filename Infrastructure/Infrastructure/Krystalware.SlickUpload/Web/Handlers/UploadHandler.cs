using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Krystalware.SlickUpload.Web.Handlers
{
    /// <summary>
    /// Defines a handler that handles uploads.
    /// </summary>
    public class UploadHandler : IHttpHandler, IReadOnlySessionState
    {
        /// <inheritdoc />
        public void ProcessRequest(HttpContext context)
        {
            ProgressHandler.RenderProgress(context, SlickUploadContext.CurrentUploadRequest);
        }

        /// <inheritdoc />
        public bool IsReusable { get { return true; } }
    }
}
