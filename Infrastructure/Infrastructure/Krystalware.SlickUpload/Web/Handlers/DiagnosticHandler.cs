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
    public class DiagnosticHandler : IHttpHandler, IReadOnlySessionState
    {
        /// <inheritdoc />
        public void ProcessRequest(HttpContext context)
        {
            // TODO: much more good shiz
            context.Response.Write("<html><head><title>SlickUpload Handler Diagnostic</title></head><body><h1>SlickUpload Handler Diagnostic</h1><p>SlickUpload handler is accessible at " + context.Request.Url.GetLeftPart(UriPartial.Path) + ".</p></body></html>");
        }

        /// <inheritdoc />
        public bool IsReusable { get { return true; } }
    }
}
