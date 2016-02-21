using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Krystalware.SlickUpload.Web.Handlers
{
    /// <summary>
    /// Defines a handler that returns a blank page.
    /// </summary>
    public class BlankHandler : IHttpHandler, IReadOnlySessionState
    {
        /// <inheritdoc />
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            context.Response.Write("<html><body></body></html>");
        }

        /// <inheritdoc />
        public bool IsReusable { get { return true; } }
    }
}
