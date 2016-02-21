using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Globalization;

namespace Krystalware.SlickUpload.Web.Handlers
{
    /// <summary>
    /// Defines a handler that returns correct CORS options for an OPTIONS request.
    /// </summary>
    public class CorsOptionsHandler : IHttpHandler
    {
        /// <inheritdoc />
        public void ProcessRequest(HttpContext context)
        {
        }

        /// <inheritdoc />
        public bool IsReusable { get { return true; } }
    }
}
