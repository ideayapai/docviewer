using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Globalization;
using Krystalware.SlickUpload.Web.SessionStorage;
using Krystalware.SlickUpload.Configuration;
using System.Configuration;
using System.Reflection;
using System.Web.Configuration;
using Krystalware.SlickUpload.Web.Handlers;
using System.Threading;
using Krystalware.SlickUpload.Licensing;
using Krystalware.SlickUpload.Web.Internal;
#if NET35
using Krystalware.SlickUpload.Web.Mvc;
using System.Web.Mvc;
#endif

namespace Krystalware.SlickUpload.Web
{
    /// <summary>
    /// The <see cref="IHttpModule" /> that processes and handles SlickUpload uploads.
    /// </summary>
    public class SlickUploadModule : IHttpModule
    {
        static DateTime _lastCleanupDate = DateTime.MinValue;
        static readonly object _cleanupThreadLock = new object();

        static IHttpHandlerFactory _handlerFactory = new SlickUploadHandlerFactory();

        readonly KrystalwareRuntimeLicense _license;

         LicenseType _licenseType;
        readonly LicenseScope _licenseScope;
        readonly string[] _licensedUrls;

        string _sessionCookieName;

        /// <summary>
        /// Creates a new instance of the <see cref="SlickUploadModule" /> class.
        /// </summary>
        public SlickUploadModule()
        {
            _licenseType = LicenseType.Commercial;
            _licenseScope = LicenseScope.WebSite;
            //_license = (KrystalwareRuntimeLicense)new KrystalwareLicenseProvider().GetLicense(null, typeof(SlickUploadModule), this, false);
            _license = (KrystalwareRuntimeLicense)KrystalwareLicenseProvider.GetLicense(typeof(SlickUploadModule), "<RSAKeyValue><Modulus>tk384UItx23mrOJcdDqipI05NXJKZAbUL0ewFaloFzpUBFV8AQd1hUSZ9XVgNAPqBLej9rUz4v08vi2rQ/fqJXtWieQSdwa7fe+ZjaS2qYGeBWZVpEDXwT5fG63+c2MlLjX0fKMQ7FU0OkZPo76Sj1O5cIjMlX9nvOQnevW0ABM=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
            
            List<string> licensedUrls = new List<string>();

            if (_license != null)
            {
                foreach (RuntimeLicensedProduct product in _license.LicensedProducts)
                {
                    if (product.AssemblyName.EndsWith(".SlickUpload") && Convert.ToInt32(product.Version) >= 6 && DateTime.Now <= product.ExpirationDate)
                    {
                        // changed to fix obfuscation issue
                        // if (product.Type > _licenseType)
                        if (product.Type != _licenseType)
                            _licenseType = product.Type;

                        // changed to fix obfuscation issue
                        // if (product.Scope > _licenseScope)
                        if (product.Scope != _licenseScope)
                                _licenseScope = product.Scope;

                        if (!string.IsNullOrEmpty(product.LicenseUrl))
                            licensedUrls.Add("." + product.LicenseUrl);
                    }
                }
            }

            _licensedUrls = licensedUrls.ToArray();
        }

        /// <inheritdoc />
        public void Init(HttpApplication context)
        {
            bool hasValidConfig = false;

            try
            {
                // TODO: detect autocreated config
                hasValidConfig = (SlickUploadContext.Config != null);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                throw new Exception("SlickUpload configuration is invalid. Message: " + ex.Message + " See http://krystalware.com/slickupload/documentation/installation for installation steps.");            
            }

            if (!hasValidConfig)
                throw new Exception("SlickUpload configuration section isn't registered. See http://krystalware.com/slickupload/documentation/installation for installation steps.");

            context.PostResolveRequestCache += new EventHandler(context_PostResolveRequestCache);
            
            if (string.Equals(SlickUploadContext.Config.AttachEvent, "BeginRequest", StringComparison.InvariantCultureIgnoreCase))
                context.BeginRequest += new EventHandler(context_PreRequestHandlerExecute);
            else
                context.PreRequestHandlerExecute += new EventHandler(context_PreRequestHandlerExecute);

            context.PostRequestHandlerExecute += new EventHandler(context_PostRequestHandlerExecute);
            context.Error += new EventHandler(context_Error);

            if (SlickUploadContext.SessionStorageProvider is SessionStateSessionStorageProvider || (SlickUploadContext.SessionStorageProvider is AdaptiveSessionStorageProvider && ((AdaptiveSessionStorageProvider)SlickUploadContext.SessionStorageProvider).InternalProvider is SessionStateSessionStorageProvider))
            {
                _sessionCookieName = ((SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState")).CookieName;

                context.EndRequest += new EventHandler(context_EndRequest);
            }


            //_licenseType = LicenseType.Commercial;
            //if (_licenseType == LicenseType.Evaluation || _licenseScope == LicenseScope.WebSite)
            //    context.ReleaseRequestState += new EventHandler(context_ReleaseRequestState);


#if NET35
            try
            {
                RegisterModelBinders();
            }
            catch
            {
                // TODO: log somewhere for debugging purposes?
            }
#endif
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            HttpCookie cookie = context.Items["SlickUploadSessionStateFixCookie"] as HttpCookie;
            //HttpCookie 
            // We're set to use session state, but we didn't create a cookie. Ensure we do.
            //if (cookie != null && context.Response.Cookies.AllKeys.in.Keys[cookie.Name] == null)
            if (cookie != null)
                context.Response.Cookies.Set(cookie);
        }

#if NET35
        void RegisterModelBinders()
        {
            if (!ModelBinders.Binders.ContainsKey(typeof(UploadSession)))
                // TODO: figure out why this breaks adding control to toolbox if mvc isn't installed
                ModelBinders.Binders.Add(typeof(UploadSession), (IModelBinder)UploadSessionModelBinderFactory.CreateInstance());
        }
#endif

        void context_PostResolveRequestCache(object sender, EventArgs e)
        {
            // TODO: document this, provide workaround for < .NET SP2?
            HttpContext context = HttpContext.Current;

            if (context.Request.Path.IndexOf("SlickUpload.axd", StringComparison.InvariantCultureIgnoreCase) != -1)
                TryRemapHandler(context, SlickUploadHandlerFactory.GetHandler(context));
        }

        private void TryRemapHandler(HttpContext context, IHttpHandler iHttpHandler)
        {
            try
            {
                context.RemapHandler(SlickUploadHandlerFactory.GetHandler(context));
            }
            catch
            {
                // TODO: error somewhere?
            }
        }

        void context_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            UploadSession session = SlickUploadContext.CurrentUploadSessionInternal;

            if (session != null && session.State != UploadState.Uploading)
                SlickUploadContext.CommitSession(session);

            ComponentHelper.EnsureScriptsRendered();

            // We're set to use session state, but we didn't create a cookie. Ensure we do.
            if (HttpContext.Current.Items["EnsureSessionCreated"] != null)
            {
                HttpCookie cookie = context.Response.Cookies[_sessionCookieName];

                if (cookie == null)
                    cookie = new HttpCookie(_sessionCookieName, context.Session.SessionID) { Path = "/", HttpOnly = true };
                else
                    cookie.Value = context.Session.SessionID;

                context.Items["SlickUploadSessionStateFixCookie"] = cookie;
            }
        }

        void context_Error(object sender, EventArgs e)
        {
            if (SlickUploadContext.CurrentUploadRequest != null && SlickUploadContext.CurrentUploadRequest.Error != null)
            {
                HttpContext context = HttpContext.Current;

                context.Response.TrySkipIisCustomErrors = true;
                context.Response.Clear();
                context.Server.ClearError();

                context.Response.StatusCode = 500;

                //new UploadHandler().ProcessRequest(context);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        { }

        void context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            //System.Diagnostics.Debug.WriteLine(context.Request.Url.ToString() + " - " + (context.Request.Cookies.Count > 0 ? context.Request.Cookies[0].Value : "null"));

            if (IsUploadRequest(context.Request))
            {
                if (context.Trace != null && context.Trace.IsEnabled)
                    throw new HttpException("Trace must be disabled for SlickUpload to intercept upload requests.");

                if ((DateTime.Now - _lastCleanupDate).TotalSeconds > SlickUploadContext.Config.SessionStorageProvider.StaleTimeout)
                {
                    lock (_cleanupThreadLock)
                    {
                        if ((DateTime.Now - _lastCleanupDate).TotalSeconds > SlickUploadContext.Config.SessionStorageProvider.StaleTimeout)
                        {
                            _lastCleanupDate = DateTime.Now;

                            Thread cleanupThread = new Thread(CleanupThread);

                            cleanupThread.Start();
                        }
                    }
                }

                string uploadSessionId = context.Request.QueryString["uploadSessionId"];

                // Generate an uploadSessionId if none was specified
                if (string.IsNullOrEmpty(uploadSessionId))
                    uploadSessionId = Guid.NewGuid().ToString();

                UploadSession session = SlickUploadContext.SessionStorageProvider.GetSession(uploadSessionId);

                if (session == null)
                {
                    session = new UploadSession(uploadSessionId, context.Request.QueryString["uploadProfile"]);

                    session.State = UploadState.Uploading;

                    SlickUploadContext.SessionStorageProvider.SaveSession(session, true);
                }

                SlickUploadContext.CurrentUploadSessionInternal = session;

                string uploadRequestId = context.Request.QueryString["uploadRequestId"];

                // Generate an uploadRequestId if none was specified
                if (string.IsNullOrEmpty(uploadRequestId))
                    uploadRequestId = Guid.NewGuid().ToString();

                UploadHttpRequest httpRequest = new UploadHttpRequest(context);
                UploadRequest request = new UploadRequest(uploadSessionId, uploadRequestId, httpRequest.ContentLength, context.Request.QueryString["uploadProfile"]);

                bool isCalculateSize = (context.Request.QueryString["handlerType"] == "calculatesize");

                try
                {
                    SlickUploadContext.CurrentUploadRequest = request;

                    if (httpRequest.ContentLength > request.UploadProfile.MaxRequestLengthBytes || (httpRequest.ContentLength < 0 && request.UploadProfile.MaxRequestLength > 4097152))
                    {
                        //SimpleLogger.Log("Request too big... aborting");

                        request.ErrorType = UploadErrorType.MaxRequestLengthExceeded;

                        context.ApplicationInstance.CompleteRequest();
                    }

                    if (request.ErrorType == UploadErrorType.None && !(request.UploadFilter == null || request.UploadFilter.ShouldHandleRequest(httpRequest)))
                    {
                        request.ErrorType = UploadErrorType.UploadFilter;
                    }

                    if (request.ErrorType != UploadErrorType.None)
                        request.State = UploadState.Error;
                    else if (isCalculateSize)
                        request.State = UploadState.Initializing;
                    else
                        request.State = UploadState.Uploading;
                }
                catch (Exception ex)
                {
                    request.State = UploadState.Error;
                    request.ErrorType = UploadErrorType.Other;
                    request.Error = ex;

                    throw;
                }
                finally
                {
                    bool hasExistingRequest = false;

                    try
                    {
                        hasExistingRequest = (SlickUploadContext.SessionStorageProvider.GetRequest(uploadSessionId, request.UploadRequestId) != null);
                    }
                    catch
                    { }

                    if (!hasExistingRequest)
                        SlickUploadContext.InsertRequest(request);
                    else
                        SlickUploadContext.UpdateRequest(request, true);
                }

                if (request.ErrorType == UploadErrorType.None)
                {
                    if (isCalculateSize)
                    {
                        httpRequest.Worker.CloseConnection();
                        context.ApplicationInstance.CompleteRequest();

                        return;
                    }

                    context.Server.ScriptTimeout = request.UploadProfile.ExecutionTimeout;

                    MimeUploadHandler handler = null;

                    try
                    {
                        handler = new MimeUploadHandler(httpRequest, request);

                        handler.ProcessRequest();

                        // TODO: should we check the session's state here, and cancel?
                        // TODO: maybe no, because MimeUploadHandler checks on part end and throws cancelled if it should
                        request.State = UploadState.Complete;
                    }
                    catch (Exception ex)
                    {
                        if (handler != null)
                        {
                            try
                            {
                                handler.CancelParse();
                            }
                            catch
                            {
                                // TODO: what do we do with this exception?
                            }
                        }

                        request.State = UploadState.Error;

                        if (ex is UploadDisconnectedException)
                        {
                            request.ErrorType = UploadErrorType.Disconnected;
                        }
                        else if (ex is UploadCancelledException)
                        {
                            request.ErrorType = UploadErrorType.Cancelled;
                        }
                        else
                        {
                            request.ErrorType = UploadErrorType.Other;

                            request.Error = ex;
                        }

                        try
                        {
                            SlickUploadContext.CleanupRequest(request);
                        }
                        catch
                        {
                            // TODO: what do we do with this exception?
                        }

                        // If we were disconnected, let everything pass through. Otherwise, rethrow it.
                        if (ex is UploadDisconnectedException || ex is UploadCancelledException)
                        {
                            httpRequest.Worker.CloseConnection();
                            context.ApplicationInstance.CompleteRequest();
                        }
                        else
                            throw;
                    }
                    finally
                    {
                        SlickUploadContext.UpdateRequest(request, true);
                    }
                }
            }
        }

        bool IsUploadRequest(HttpRequest request)
        {
            if (request.RequestType.Equals("POST", StringComparison.InvariantCultureIgnoreCase) &&
                (string.Compare(request.ContentType, 0, "multipart/form-data", 0, 19, true, CultureInfo.InvariantCulture) == 0 || string.Equals(request.ContentType, "application/octet-stream", StringComparison.InvariantCultureIgnoreCase)) &&
                (request.Path.IndexOf("SlickUpload.axd", StringComparison.InvariantCultureIgnoreCase) != -1 || SlickUploadContext.Config.HandleRequests))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void context_ReleaseRequestState(object sender, EventArgs e)
        {
            HttpRequest request = HttpContext.Current.Request;

            if (request.Path.IndexOf("SlickUpload.axd", StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                HttpResponse response = HttpContext.Current.Response;

                bool brandFilter = true;

                if (response.ContentType == null || string.Compare(request.Form["__ASYNCPOST"], "true", true) == 0 ||
                    !(response.ContentType.Equals("text/html", StringComparison.InvariantCultureIgnoreCase) ||
                        response.ContentType.Equals("application/xhtml+xml", StringComparison.InvariantCultureIgnoreCase)))
                    brandFilter = false;


                if (brandFilter)
                    response.Filter = new BrandFilter(response.Filter, response.ContentEncoding, request.Url.Scheme);
            }
        }

        /*long InitMaxRequestLength(HttpContext context)
        {
            long _maxRequestLength;

            long slickUploadMaxRequestLength = 0;
            string slickUploadMaxRequestLengthString = UploadParser["maxUploadRequestLength"];
            
            if (!long.TryParse(slickUploadMaxRequestLengthString, out slickUploadMaxRequestLength))
                slickUploadMaxRequestLength = -1;

            if (slickUploadMaxRequestLength > 0)
            {
                _maxRequestLength = slickUploadMaxRequestLength;
            }
            else
            {
                long maxAllowedContentLength = GetMaxAllowedContentLength(context);
                long maxRequestLength = GetMaxRequestLength(context);

                if (maxAllowedContentLength > 0)
                    _maxRequestLength = (maxAllowedContentLength < maxRequestLength ? maxAllowedContentLength : maxRequestLength);
                else
                    _maxRequestLength = maxRequestLength;

                if (_maxRequestLength <= 0)
                    _maxRequestLength = -1;//4096 * 1024;
            }

            return _maxRequestLength;
        }

        long GetMaxRequestLength(HttpContext context)
        {
            long maxRequestLength = -1;

            object httpRuntimeConfig = null;

            try
            {
                httpRuntimeConfig = context.GetSection("system.web/httpRuntime");
            }
            catch
            { }
            //_maxRequestLength = -1;

            if (httpRuntimeConfig != null)
                maxRequestLength = ((HttpRuntimeSection)httpRuntimeConfig).MaxRequestLength * 1024;

            return maxRequestLength;
        }

        long GetMaxAllowedContentLength(HttpContext context)
        {
            long maxAllowedContentLength = 0;

            try
            {
                Assembly a = Assembly.Load("Microsoft.Web.Administration, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

                if (a != null)
                {
                    Type man = a.GetType("Microsoft.Web.Administration.WebConfigurationManager");

                    object requestFilteringSection = man.InvokeMember("GetSection",
                                                                   BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null,
                                                                   new object[] { "system.webServer/security/requestFiltering" });

                    object requestLimitsSection = requestFilteringSection.GetType().InvokeMember("GetChildElement",
                                                                                                 BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, requestFilteringSection,
                                                                                                 new object[] { "requestLimits" });

                    object maxAllowedContentLengthAttribute = requestLimitsSection.GetType().InvokeMember("GetAttributeValue",
                                                                                                          BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, requestLimitsSection,
                                                                                                          new object[] { "maxAllowedContentLength" });

                    maxAllowedContentLength = (long)maxAllowedContentLengthAttribute;
                }
            }
            catch
            { }

            return maxAllowedContentLength;
        }*/


        void CleanupThread()
        {
            try
            {
                // TODO: log

                foreach (UploadSession session in SlickUploadContext.SessionStorageProvider.GetStaleSessions(DateTime.Now.AddSeconds(-SlickUploadContext.Config.SessionStorageProvider.StaleTimeout)))
                    SlickUploadContext.RollbackSession(session);

                // TODO: log
            }
            catch
            {
                // TODO: log
            }
        }
    }
}
