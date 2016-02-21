using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

using Krystalware.SlickUpload.Web.Handlers;

namespace Krystalware.SlickUpload.Web
{
    /// <summary>
    /// An <see cref="IHttpHandlerFactory" /> that creates SlickUpload handler instances.
    /// </summary>
    public class SlickUploadHandlerFactory : IHttpHandlerFactory
    {
        /// <summary>
        /// Selects and returns the correct SlickUpload handler for the specified <see cref="HttpContext" />.
        /// </summary>
        /// <param name="context">The context for which to return a SlickUpload handler.</param>
        /// <returns>The correct SlickUpload handler for the specified <see cref="HttpContext" />.</returns>
        public static IHttpHandler GetHandler(HttpContext context)
        {                
            Uri originUri = null;

            // TODO: is this right?
            if (context.Request.Headers["ORIGIN"] != null)
                originUri = new Uri(context.Request.Headers["ORIGIN"]);
            else
                originUri = context.Request.UrlReferrer;

            if (originUri != null)
                context.Response.Headers["Access-Control-Allow-Origin"] = originUri.GetLeftPart(UriPartial.Authority);

            context.Response.Headers["Access-Control-Allow-Headers"] = "X-Requested-With,X-File-Size,X-File-Name,X-File-Content-Type,X-File-Source-Element,X-SlickUpload-Data"; //"*";
            context.Response.Headers["Access-Control-Allow-Methods"] =  "GET, POST, OPTIONS";
            context.Response.Headers["Access-Control-Allow-Credentials"] = "true";

            // TODO: make sure this can be set here
            context.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.Cache.SetNoServerCaching();
            context.Response.Cache.SetExpires(DateTime.Now);

            string handlerType = context.Request.QueryString["handlerType"];

            if (string.Equals(context.Request.HttpMethod, "OPTIONS", StringComparison.InvariantCultureIgnoreCase) || string.Equals(handlerType, "blank", StringComparison.InvariantCultureIgnoreCase))
                return new BlankHandler();
            else if (string.Equals(handlerType, "progress", StringComparison.InvariantCultureIgnoreCase))
                return new ProgressHandler();
            else if (string.Equals(handlerType, "sessionWrite", StringComparison.InvariantCultureIgnoreCase))
                return new SessionHandler();
            else if (string.Equals(handlerType, "upload", StringComparison.InvariantCultureIgnoreCase))
                return new UploadHandler();
            else if (string.Equals(handlerType, "calculatesize", StringComparison.InvariantCultureIgnoreCase))
                return new UploadHandler();
            //else if (string.Equals(handlerType, "cancel", StringComparison.InvariantCultureIgnoreCase))
            //    return new CancelHandler();
            else if (string.Equals(handlerType, "complete", StringComparison.InvariantCultureIgnoreCase))
                return new CompletionTrampolineHandler();
            //else if (string.Equals(handlerType, "license", StringComparison.InvariantCultureIgnoreCase))
            //    return new LicenseHandler();
            else if (string.Equals(handlerType, "diagnostic", StringComparison.InvariantCultureIgnoreCase) || (HttpContext.Current.IsDebuggingEnabled && context.Request.QueryString.Count == 0))
                return new DiagnosticHandler();
            else
                throw new HttpException(500, "Valid handlerType parameter is required.");
        }

        /// <inheritdoc />
        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            return GetHandler(context);
        }

        /// <inheritdoc />
        public void ReleaseHandler(IHttpHandler handler)
        { }
    }
}