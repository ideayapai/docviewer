using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Globalization;
using Krystalware.SlickUpload.Configuration;

namespace Krystalware.SlickUpload.Web.Handlers
{
    /// <summary>
    /// Defines a handler that provides a trampoline page for AJAX cross domain requests.
    /// </summary>
    public class CompletionTrampolineHandler : IHttpHandler
    {
        /// <inheritdoc />
        public void ProcessRequest(HttpContext context)
        {
            string documentDomain = null;

            UploadProfileElement profileElement = SlickUploadContext.Config.UploadProfiles.GetUploadProfileElement(context.Request.QueryString["uploadProfile"], false);

            if (profileElement != null && !string.IsNullOrEmpty(profileElement.DocumentDomain))
                documentDomain = "document.domain=\"" + profileElement.DocumentDomain + "\";\r\n";

            context.Response.ContentType = "text/html";
            // TODO: implement progress thunk
            context.Response.Write("<html><head><script type=\"text/javascript\">" + documentDomain +
                
@"    var getXmlReq = (function ()
    {
        if (window.XMLHttpRequest)
        {
            return function () { return new XMLHttpRequest(); };
        }
        else
        {
            var progIds = [""Msxml2.XMLHTTP.6.0"", ""MSXML2.XMLHTTP.3.0"", ""MSXML2.XMLHTTP"", ""Microsoft.XMLHTTP""];

            for (var i = 0; i < progIds.length; i++)
            {
                try
                {
                    var constructor = function () { return new ActiveXObject(progIds[i]); };

                    if (constructor())
                        return constructor;
                }
                catch (ex)
                {
                    // Eat it and try the next one
                }
            }

            return function () { return null; }
        }
    })();"         
                
                + "if (parent.kw && parent.kw._completionActions) { for (var i = 0; i < parent.kw._completionActions.length; i++) { parent.kw._completionActions[i](getXmlReq()); } parent.kw._completionActions = []; }</script></head><body></body></html>");
        }
        
        /// <inheritdoc />
        public bool IsReusable { get { return true; } }
    }
}
