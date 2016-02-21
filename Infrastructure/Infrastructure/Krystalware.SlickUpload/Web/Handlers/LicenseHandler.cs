using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Reflection;
using System.Web.UI;

namespace Krystalware.SlickUpload.Web.Handlers
{
    /// <summary>
    /// Defines a handler that returns a boolean that specifies whether the component is licensed.
    /// </summary>
    public class LicenseHandler : IHttpHandler, IReadOnlySessionState
    {
        /// <inheritdoc />
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Write("{ isLicensed: " + SlickUploadModule.IsLicensed.ToString().ToLower() +
                                   ", brandLocation: \"" + (SlickUploadContext.Config.BrandLocation == BrandLocation.BottomRight ? "bottom-right" : "inline") +
                                   "\", version: \"" + Assembly.GetExecutingAssembly().GetName().Version.ToString() +
                                   "\", brandUrl: \"" + new Page().ClientScript.GetWebResourceUrl(typeof(LicenseHandler), "Krystalware.SlickUpload.Resources.PoweredBy.png") + "\" }");
        }

        /// <inheritdoc />
        public bool IsReusable { get { return true; } }
    }
}