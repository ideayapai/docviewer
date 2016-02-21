using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Globalization;

namespace Krystalware.SlickUpload.Web.Handlers
{
    /// <summary>
    /// Defines a handler that cancels an upload request.
    /// </summary>
    public class CancelHandler : IHttpHandler
    {
        /// <inheritdoc />
        public void ProcessRequest(HttpContext context)
        {
            string uploadRequestId = context.Request.QueryString["uploadRequestId"];
            string uploadSessionId = context.Request.QueryString["uploadSessionId"];

            if (string.IsNullOrEmpty(uploadRequestId))
                throw new HttpException(500, "uploadRequestId parameter is required.");

            if (string.IsNullOrEmpty(uploadSessionId))
                throw new HttpException(500, "uploadSessionId parameter is required.");

            UploadRequest request = SlickUploadContext.SessionStorageProvider.GetRequest(uploadSessionId, uploadRequestId);

            SlickUploadContext.CancelRequest(request);
        }

        /// <inheritdoc />
        public bool IsReusable { get { return true; } }
    }
}
