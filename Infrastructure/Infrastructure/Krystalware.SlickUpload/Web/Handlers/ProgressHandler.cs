using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Globalization;
using Krystalware.SlickUpload.Configuration;

namespace Krystalware.SlickUpload.Web.Handlers
{
    /// <summary>
    /// Defines a handler that returns progress information for an upload request.
    /// </summary>
    public class ProgressHandler : IHttpHandler
    {
        /// <inheritdoc />
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string uploadRequestId = context.Request.QueryString["uploadRequestId"];
                string uploadSessionId = context.Request.QueryString["uploadSessionId"];

                //if (string.IsNullOrEmpty(uploadRequestId))
                //    throw new HttpException(500, "uploadRequestId parameter is required.");

                if (string.IsNullOrEmpty(uploadSessionId))
                    throw new HttpException(500, "uploadSessionId parameter is required.");

                if (!string.IsNullOrEmpty(uploadRequestId))
                {
                    UploadRequest request = SlickUploadContext.SessionStorageProvider.GetRequest(uploadSessionId, uploadRequestId);

                    if (request == null && string.Equals(context.Request.QueryString["isLastRetry"], "true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string uploadProfile = context.Request.QueryString["uploadProfile"];
                        UploadSession session = SlickUploadContext.SessionStorageProvider.GetSession(uploadSessionId);

                        if (session == null)
                        {
                            session = new UploadSession(uploadSessionId, uploadProfile);

                            session.State = UploadState.Uploading;

                            SlickUploadContext.SessionStorageProvider.SaveSession(session, true);
                        }

                        request = new UploadRequest(uploadSessionId, uploadRequestId, -1, uploadProfile);

                        request.State = UploadState.Error;
                        request.ErrorType = UploadErrorType.RequestNotRecieved;

                        //request._uploadedFilesInternal.Add(new UploadedFile(context.Request.QueryString["name"], null, null, request, null));

                        SlickUploadContext.SessionStorageProvider.SaveRequest(request, true);
                    }

                    RenderProgress(context, request);
                }
                else
                    RenderProgress(context, SlickUploadContext.SessionStorageProvider.GetSession(uploadSessionId));
            }
            catch (Exception ex)
            {
                UploadProfileElement profileElement = SlickUploadContext.Config.UploadProfiles.GetUploadProfileElement(context.Request.QueryString["uploadProfile"], false);

                StringBuilder sb = new StringBuilder();

                sb.Append('{');

                JsonHelper.AppendJson(sb, "progressException", ex.ToString());

                sb.Append('}');

                WriteResponse(context, sb.ToString(), profileElement);
            }
        }

        internal static void RenderProgress(HttpContext context, UploadRequest request)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('{');

            if (request != null)
            {
                JsonHelper.AppendJson(sb, "uploadRequestId", request.UploadRequestId);
                JsonHelper.AppendJson(sb, "uploadSessionId", request.UploadSessionId);
                JsonHelper.AppendJson(sb, "position", request.Position);
                JsonHelper.AppendJson(sb, "contentLength", request.ContentLength);
                JsonHelper.AppendJson(sb, "errorType", request.ErrorType.ToString());
                if (request.Error != null)
                    JsonHelper.AppendJson(sb, "errorMessage", request.Error.Message);
                JsonHelper.AppendJson(sb, "status", request.State.ToString());
                
                if (request.UploadedFiles.Count > 1)
                    sb.Append(", files: [");

                foreach (UploadedFile file in request.UploadedFiles)
                {
                    StringBuilder fileSb;

                    if (request.UploadedFiles.Count > 1)
                    {
                        fileSb = new StringBuilder();

                        fileSb.Append('{');
                    }
                    else
                    {
                        fileSb = sb;
                    }

                    JsonHelper.AppendJson(fileSb, "name", file.ClientName);
                    JsonHelper.AppendJson(fileSb, "fileSelectorId", file.FileSelectorId);
                    JsonHelper.AppendJson(fileSb, "sourceElement", file.SourceElement);

                    foreach (KeyValuePair<string, string> pair in file.Data)
                        JsonHelper.AppendJson(fileSb, pair.Key, pair.Value);

                    if (request.UploadedFiles.Count > 1)
                    {
                        fileSb.Append('}');

                        sb.Append(fileSb.ToString());
                    }
                }

                if (request.UploadedFiles.Count > 1)
                    sb.Append(']');
            }

            sb.Append('}');

            WriteResponse(context, sb.ToString(), request != null ? request.UploadProfile : null);
        }

        internal static void RenderProgress(HttpContext context, UploadSession session)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('{');

            if (session != null)
            {
                JsonHelper.AppendJson(sb, "uploadSessionId", session.UploadSessionId);
                JsonHelper.AppendJson(sb, "state", session.State.ToString());

                foreach (KeyValuePair<string, string> pair in session.ProcessingStatus)
                    JsonHelper.AppendJson(sb, pair.Key, pair.Value);
            }

            sb.Append('}');

            WriteResponse(context, sb.ToString(), session != null ? session.UploadProfile : null);
        }

        static void WriteResponse(HttpContext context, string response, UploadProfileElement profileElement)
        {
            if (string.Equals(context.Request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.InvariantCultureIgnoreCase))
            {
                context.Response.ContentType = "application/json";

                context.Response.Write(response);
            }
            else
            {
                string documentDomain = null;

                if (profileElement == null)
                    profileElement = SlickUploadContext.Config.UploadProfiles.GetUploadProfileElement(null, false);

                if (profileElement != null && !string.IsNullOrEmpty(profileElement.DocumentDomain))
                    documentDomain = "document.domain=\"" + profileElement.DocumentDomain + "\";\r\n";

                context.Response.ContentType = "text/html";
                // TODO: implement progress thunk
                context.Response.Write("<html><head><script type=\"text/javascript\">" + documentDomain + "if (parent.kw && parent.kw._uploadFrameLoaded) { parent.kw._uploadFrameLoaded(" + response + "); }</script></head><body></body></html>");
            }
        }
        
        /// <inheritdoc />
        public bool IsReusable { get { return true; } }
    }
}
