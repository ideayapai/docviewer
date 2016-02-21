using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Configuration;
using Krystalware.SlickUpload.Configuration;
using System.Web.SessionState;
using System.Net;
using Krystalware.SlickUpload.Web.Handlers;
using System.IO;
using System.Web.Caching;

namespace Krystalware.SlickUpload.Web.SessionStorage
{
    /// <summary>
    /// An <see cref="ISessionStorageProvider" /> that stores upload session state in the ASP.NET session state store.
    /// </summary>
    public class SessionStateSessionStorageProvider : SessionStorageProviderBase
    {
        // TODO: make configurable?
        readonly string _handlerUrl;

        /// <summary>
        /// Creates a new instance of the <see cref="SessionStateSessionStorageProvider" /> class with the specified settings.
        /// </summary>
        /// <param name="settings">The <see cref="SessionStorageProviderElement" /> object that holds the configuration settings.</param>
        public SessionStateSessionStorageProvider(SessionStorageProviderElement settings)
            : base(settings)
        {
            _handlerUrl = settings.Parameters["handlerUrl"];

            if (string.IsNullOrEmpty(_handlerUrl))
                _handlerUrl = "~/SlickUpload.axd";
        }

        /// <inheritdoc />
        public override void SaveSession(UploadSession session, bool isCreate)
        {
            CallHandler("SaveSession", session);

            RegisterCleanupSessionIfStaleTimeout(session.UploadSessionId);
        }

        private void RegisterCleanupSessionIfStaleTimeout(string uploadSessionId)
        {
            string currentUrl = GetHandlerUrl(HttpContext.Current, "CleanupSessionIfStale");
            HttpCookieCollection cookies = HttpContext.Current.Request.Cookies;

            Action<string> cleanUpIfStaleCallback = delegate(string uploadSessionIdInternal)
            {
                CallHandlerInternal(currentUrl, uploadSessionIdInternal, cookies);
            };

            HttpContext.Current.Cache.Insert("session_" + uploadSessionId, cleanUpIfStaleCallback, null,
                                             Cache.NoAbsoluteExpiration,
                                             TimeSpan.FromSeconds(SlickUploadContext.Config.SessionStorageProvider.StaleTimeout),
                                             CacheItemPriority.High,
                                             new CacheItemRemovedCallback(CacheItem_Removed));
        }

        void CacheItem_Removed(string key, object value, CacheItemRemovedReason reason)
        {
            if (reason != CacheItemRemovedReason.Removed)
            {
                string uploadSessionId = key.Substring("session_".Length);

                Action<string> cleanUpIfStaleCallback = (Action<string>)value;

                cleanUpIfStaleCallback(uploadSessionId);
            }
        }

        /// <inheritdoc />
        public override UploadSession GetSession(string uploadSessionId)
        {
            return (UploadSession)CallHandler("GetSession", uploadSessionId);
        }

        /// <inheritdoc />
        public override void RemoveSession(string uploadSessionId)
        {
            CallHandler("RemoveSession", uploadSessionId);
        }

        /// <inheritdoc />
        public override void SaveRequest(UploadRequest request, bool isCreate)
        {
            CallHandler("SaveRequest", request);

            RegisterCleanupSessionIfStaleTimeout(request.UploadSessionId);
        }

        /// <inheritdoc />
        public override UploadRequest GetRequest(string uploadSessionId, string uploadRequestId)
        {
            return (UploadRequest)CallHandler("GetRequest", new string[] { uploadSessionId, uploadRequestId });
        }

        //public override void RemoveRequest(string uploadSessionId, string uploadRequestId)
        //{
        //    CallHandler("RemoveRequest", new string[] { uploadSessionId, uploadRequestId });
        //}

        /// <inheritdoc />
        public override IEnumerable<UploadRequest> GetRequestsForSession(string uploadSessionId)
        {
            return (IEnumerable<UploadRequest>)CallHandler("GetRequestsForSession", uploadSessionId);
        }

        /// <inheritdoc />
        public override IEnumerable<UploadSession> GetStaleSessions(DateTime staleAfter)
        {
            // we don't do this
            return new List<UploadSession>();
            //return (IEnumerable<UploadSession>)CallHandler("GetStaleSessions", staleAfter);
        }

        /// <summary>
        /// Writes a value to session state using the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteToSessionState(string key, object value)
        {
            CallHandler("WriteToSessionState", new object[] { key, value });
        }

        /*public override DateTime LastCleanupDate
        {
            get
            {
                object lastCleanupDate = HttpContext.Current.Session["SlickUpload_LastCleanupDate"];

                if (lastCleanupDate != null && lastCleanupDate is DateTime)
                    return (DateTime)lastCleanupDate;
                else
                    return DateTime.MinValue;
            }
            set
            {
                CallHandler("WriteToSessionState", new object[] { "SlickUpload_LastCleanupDate", value });
            }
        }

        public static void RollbackAllUploadSessions(HttpSessionState state)
        {
            List<UploadSession> uploadSessionList = new List<UploadSession>();

            foreach (string key in state.Keys)
            {
                if (key.StartsWith("kw_UploadSession_"))
                    uploadSessionList.Add((UploadSession)state[key]);
            }

            foreach (UploadSession session in uploadSessionList)
                SlickUploadContext.RollbackSession(session);
        }*/

        object CallHandler(string command, object data)
        {
            if (HttpContext.Current.Handler is IRequiresSessionState && !(HttpContext.Current.Handler is IReadOnlySessionState))
                return SessionHandler.ExecuteCommand(command, data);

            return CallHandlerInternal(GetHandlerUrl(HttpContext.Current, command), data, HttpContext.Current.Request.Cookies);
        }

        string GetHandlerUrl(HttpContext context, string command)
        {
            UriBuilder uriBuilder;

            // Build the base URL
            if (_handlerUrl.StartsWith("~/"))
                uriBuilder = new UriBuilder(new Uri(new Uri(context.Request.Url.GetLeftPart(UriPartial.Authority)), _handlerUrl));
            else
                uriBuilder = new UriBuilder(_handlerUrl);

            // Make sure we add cookieless session path if needed
            if (uriBuilder.Path.StartsWith("/~"))
                uriBuilder.Path = context.Response.ApplyAppPathModifier(uriBuilder.Path.Substring(1));

            string url = uriBuilder.Uri.ToString();
            
            url += (url.IndexOf('?') != -1) ? "&" : "?";

            return url + "handlerType=sessionWrite&command=" + command;
        }
        
        object CallHandlerInternal(string url, object data, HttpCookieCollection cookies)
        {
            Uri uri = new Uri(url);

            string postData;

            if (data != null)
                postData = "data=" + SessionHandler.GetSerializedString(data);
            else
                postData = "";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);

            try
            {
                // TODO: do we need to do better cookie handling?
                CookieContainer cookieContainer = new CookieContainer();

                if (cookies != null)
                {
                    foreach (string name in cookies.AllKeys)
                    {
                        string quotedCookieValue = cookies[name].Value;

                        if (quotedCookieValue == null)
                            quotedCookieValue = "";
                        else if (quotedCookieValue.IndexOfAny(new char[] { ',', ';' }) != -1)
                            quotedCookieValue = "\"" + quotedCookieValue.Replace("\"", "\\\"") + "\"";

                        try
                        {
                            cookieContainer.Add(new Cookie(name, quotedCookieValue, "/", uri.Host));
                        }
                        catch (Exception ex)
                        {
                            // Eat it -- we mainly want the session cookies anyway
                            // TODO: log
                        }
                    }
                }

                string cookieHeader = cookieContainer.GetCookieHeader(uri);

                if (!string.IsNullOrEmpty(cookieHeader))
                    req.Headers["Cookie"] = cookieHeader;

                req.Method = "POST";
                req.ContentLength = postData.Length;
                req.ContentType = "application/x-www-form-urlencoded";
                
                // TODO: make this configurable
                // TODO: add better errors
                // TODO: use the polltimeout value on the control
                req.Timeout = 10000;
                req.ServicePoint.ConnectionLimit = 200;
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
                    return SessionHandler.GetStringDeserialized(responseData);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
