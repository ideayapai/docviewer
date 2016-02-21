using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Web.SessionState;
using System.Web.UI;

using Krystalware.SlickUpload.Web.Handlers;

namespace Krystalware.SlickUpload.Web
{
    // TODO: optimize to use streams through and through
    public static class SessionHandlerHelper
    {
        // TODO: make configurable?
        readonly static string _handlerUrl = "~/SlickUpload.axd?handlerType=sessionWrite";

        public static object CallHandler(string command, object data)
        {
            if (HttpContext.Current.Handler is IRequiresSessionState && !(HttpContext.Current.Handler is IReadOnlySessionState))
                return SessionHandler.ExecuteCommand(command, data);

            HttpRequest request = HttpContext.Current.Request;

            // Build the base URL
            UriBuilder uriBuilder = new UriBuilder(new Uri(new Uri(request.Url.GetLeftPart(UriPartial.Authority)), _handlerUrl));

            // Make sure we add cookieless session path if needed
            if (uriBuilder.Path.StartsWith("/~"))
                uriBuilder.Path = HttpContext.Current.Response.ApplyAppPathModifier(uriBuilder.Path.Substring(1));

            string postData = "";

            if (data != null)
                postData += "&data=" + GetSerializedString(data);

            WebRequest req = WebRequest.Create(uriBuilder.Uri.ToString() + "&command=" + command);

            try
            {
                // TODO: do we need to do better cookie handling?
                req.Headers["Cookie"] = request.Headers["Cookie"];
                req.Method = "POST";
                req.ContentLength = postData.Length;
                req.ContentType = "application/x-www-form-urlencoded";
                // TODO: pass through windows auth

                using (Stream s = req.GetRequestStream())
                using (StreamWriter w = new StreamWriter(s))
                {
                    w.Write(postData);
                }

                WebResponse resp = req.GetResponse();

                string responseData;

                using (Stream s = resp.GetResponseStream())
                using (StreamReader r = new StreamReader(s))
                {
                    responseData = r.ReadToEnd();
                }

                if (!string.IsNullOrEmpty(responseData))
                {
                    return GetStringDeserialized(responseData);
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }

        internal static string GetSerializedString(object data)
        {
            using (StringWriter sw = new StringWriter())
            {
                new LosFormatter().Serialize(sw, data);

                // TODO: encrypt

                return sw.ToString();
            }
        }

        internal static object GetStringDeserialized(string value)
        {
            // TODO: decrypt
            return new LosFormatter().Deserialize(value);
        }
    }
}
