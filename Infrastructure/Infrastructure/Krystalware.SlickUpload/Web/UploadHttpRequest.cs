using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Permissions;
using System.Reflection;
using System.IO;
using Krystalware.SlickUpload.Configuration;
using System.Web.Configuration;
using System.Security;

namespace Krystalware.SlickUpload.Web
{
    /// <summary>
    /// Enables SlickUpload to stream uploads and read the HTTP values sent by a client during a Web request.
    /// </summary>
    public class UploadHttpRequest
    {
        static readonly bool _hasUnmanagedCodePermission = HasUnmanagedCodePermission();

        HttpWorkerRequest _worker;

        long _contentLength;

        HttpRequest _request;
        NameValueCollection _params;
        NameValueCollection _form = new NameValueCollection();

        internal HttpWorkerRequest Worker { get { return _worker; } }

        #region HttpRequest Properties
        /// <see cref="HttpRequest.AcceptTypes" copy="true" />
        public string[] AcceptTypes { get { return _request.AcceptTypes; } }
        /// <see cref="HttpRequest.AnonymousID" copy="true" />
        public string AnonymousID { get { return _request.AnonymousID; } }
        /// <see cref="HttpRequest.ApplicationPath" copy="true" />
        public string ApplicationPath { get { return _request.ApplicationPath; } }
        /// <see cref="HttpRequest.AppRelativeCurrentExecutionFilePath" copy="true" />
        public string AppRelativeCurrentExecutionFilePath { get { return _request.AppRelativeCurrentExecutionFilePath; } }
        /// <see cref="HttpRequest.Browser" copy="true" />
        public HttpBrowserCapabilities Browser { get { return _request.Browser; } }
        /// <see cref="HttpRequest.ClientCertificate" copy="true" />
        public HttpClientCertificate ClientCertificate { get { return _request.ClientCertificate; } }
        /// <see cref="HttpRequest.ContentEncoding" copy="true" />
        public Encoding ContentEncoding { get { return _request.ContentEncoding; } }
        /// <see cref="HttpRequest.ContentLength" copy="true" />
        public long ContentLength { get { return _contentLength; } }
        /// <see cref="HttpRequest.ContentType" copy="true" />
        public string ContentType { get { return _request.ContentType; } }
        /// <see cref="HttpRequest.Cookies" copy="true" />
        public HttpCookieCollection Cookies { get { return _request.Cookies; } }
        /// <see cref="HttpRequest.CurrentExecutionFilePath" copy="true" />
        public string CurrentExecutionFilePath { get { return _request.CurrentExecutionFilePath; } }
        /// <see cref="HttpRequest.FilePath" copy="true" />
        public string FilePath { get { return _request.FilePath; } }
        /// <see cref="HttpRequest.Form" copy="true" />
        public NameValueCollection Form
        {
            get
            {
                return _form;
            }
        }
        /// <see cref="HttpRequest.Headers" copy="true" />
        public NameValueCollection Headers { get { return _request.Headers; } }
        /// <see cref="HttpRequest.HttpChannelBinding" copy="true" />
        public ChannelBinding HttpChannelBinding { get { return _request.HttpChannelBinding; } }
        /// <see cref="HttpRequest.HttpMethod" copy="true" />
        public string HttpMethod { get { return _request.HttpMethod; } }
        /// <see cref="HttpRequest.IsAuthenticated" copy="true" />
        public bool IsAuthenticated { get { return _request.IsAuthenticated; } }
        /// <see cref="HttpRequest.IsLocal" copy="true" />
        public bool IsLocal { get { return _request.IsLocal; } }
        /// <see cref="HttpRequest.IsSecureConnection" copy="true" />
        public bool IsSecureConnection { get { return _request.IsSecureConnection; } }
        /// <see cref="HttpRequest.LogonUserIdentity" copy="true" />
        public WindowsIdentity LogonUserIdentity { get { return _request.LogonUserIdentity; } }
        /// <see cref="HttpRequest.Params" copy="true" />
        public NameValueCollection Params
        {
            get
            {
                if (_params == null)
                {
                    _params = new NameValueCollection();

                    _params.Add(QueryString);
                    _params.Add(Form);

                    foreach (HttpCookie cookie in Cookies)
                        _params.Add(cookie.Name, cookie.Value);

                    _params.Add(ServerVariables);
                }

                return _params;
            }
        }
        /// <see cref="HttpRequest.Path" copy="true" />
        public string Path { get { return _request.Path; } }
        /// <see cref="HttpRequest.PathInfo" copy="true" />
        public string PathInfo { get { return _request.PathInfo; } }
        /// <see cref="HttpRequest.PhysicalApplicationPath" copy="true" />
        public string PhysicalApplicationPath { get { return _request.PhysicalApplicationPath; } }
        /// <see cref="HttpRequest.PhysicalPath" copy="true" />
        public string PhysicalPath { get { return _request.PhysicalPath; } }
        /*/// <see cref="HttpRequest.Params" copy="true" />*/
        /// <summary>
        /// Gets the collection of HTTP query string variables.
        /// </summary>
        /// <returns>
        /// A <see cref="NameValueCollection" /> containing the collection of query string variables sent by the client.
        /// </returns>
        public NameValueCollection QueryString { get { return _request.QueryString; } }
        /// <see cref="HttpRequest.RawUrl" copy="true" />
        public string RawUrl { get { return _request.RawUrl; } }
        /// <see cref="HttpRequest.RequestType" copy="true" />
        public string RequestType { get { return _request.RequestType; } }
        /// <see cref="HttpRequest.ServerVariables" copy="true" />
        public NameValueCollection ServerVariables { get { return _request.ServerVariables; } }
        /// <see cref="HttpRequest.Url" copy="true" />
        public Uri Url { get { return _request.Url; } }
        /// <see cref="HttpRequest.UrlReferrer" copy="true" />
        public Uri UrlReferrer { get { return _request.UrlReferrer; } }
        /// <see cref="HttpRequest.UserAgent" copy="true" />
        public string UserAgent { get { return _request.UserAgent; } }
        /// <see cref="HttpRequest.UserHostAddress" copy="true" />
        public string UserHostAddress { get { return _request.UserHostAddress; } }
        /// <see cref="HttpRequest.UserHostName" copy="true" />
        public string UserHostName { get { return _request.UserHostName; } }
        /// <see cref="HttpRequest.UserLanguages" copy="true" />
        public string[] UserLanguages { get { return _request.UserLanguages; } }
        /// <see cref="HttpRequest.this[string]" copy="true" />
        public string this[string key] { get { return Params[key]; } }
        #endregion

        internal UploadHttpRequest(HttpContext context)
        {
            _request = context.Request;
            _worker = GetWorkerRequest(context);

            // TODO: should we silently ignore?
            if (_worker == null)
                throw new HttpException("Could not intercept worker.");

            string fileSizeHeader = _worker.GetUnknownRequestHeader("X-File-Size");

            if (string.IsNullOrEmpty(fileSizeHeader) || !long.TryParse(fileSizeHeader, out _contentLength))
                _contentLength = long.Parse(_worker.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentLength));
        }

        //[SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
        HttpWorkerRequest GetWorkerRequest(HttpContext context)
        {
            if (_hasUnmanagedCodePermission)
            {
                IServiceProvider provider = (IServiceProvider)context;

                return (HttpWorkerRequest)provider.GetService(typeof(HttpWorkerRequest));
            }
            else
            {
                Type c = context.GetType();

                PropertyInfo p = c.GetProperty("WorkerRequest", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (p != null)
                    return (HttpWorkerRequest)p.GetValue(context, null);
                else
                    return null;
            }
        }

        internal void InjectTextParts(string value)
        {
            byte[] textParts = ContentEncoding.GetBytes(value);

            if (_worker.GetType().FullName.StartsWith("Mono."))
                InjectTextParts(_request, textParts);
            else if (_worker.GetType().Name == "IIS7WorkerRequest")
                InjectTextPartsIIS7(_request, textParts);
            else
            {
                InjectTextParts(_worker, textParts);
                InjectTextParts(_request, textParts);
            }
        }

        [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
        void InjectTextParts(HttpWorkerRequest worker, byte[] textParts)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            Type type = worker.GetType();

            while ((type != null) && !(type.FullName == "System.Web.Hosting.ISAPIWorkerRequest" ||
                type.FullName == "Microsoft.VisualStudio.WebHost.Request" ||
                type.FullName == "Cassini.Request"))
            {
                type = type.BaseType;
            }

            if (type != null)
            {
                switch (type.FullName)
                {
                    case "System.Web.Hosting.ISAPIWorkerRequest":
                        type.GetField("_contentAvailLength", bindingFlags).SetValue(worker, textParts.Length);
                        type.GetField("_contentTotalLength", bindingFlags).SetValue(worker, textParts.Length);
                        type.GetField("_preloadedContent", bindingFlags).SetValue(worker, textParts);
                        type.GetField("_preloadedContentRead", bindingFlags).SetValue(worker, true);

                        break;
                    case "Cassini.Request":
                    case "Microsoft.VisualStudio.WebHost.Request":
                        type.GetField("_contentLength", bindingFlags).SetValue(worker, textParts.Length);
                        type.GetField("_preloadedContent", bindingFlags).SetValue(worker, textParts);
                        type.GetField("_preloadedContentLength", bindingFlags).SetValue(worker, textParts.Length);

                        break;
                }
            }
        }

        [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
        void InjectTextPartsIIS7(HttpRequest request, byte[] textParts)
        {
            object content = TypeFactory.CreateInstance(
                                "System.Web.HttpRawUploadedContent, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                                new object[] { textParts.Length, textParts.Length },
                                BindingFlags.NonPublic | BindingFlags.Instance);

            Type contentT = content.GetType();

            contentT.InvokeMember("AddBytes", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                  null, content, new object[] { textParts, 0, textParts.Length });

            contentT.InvokeMember("DoneAddingBytes", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod,
                                  null, content, null);

            Type requestT = request.GetType();

            requestT.InvokeMember("_rawContent", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetField,
                                  null, request, new object[] { content });
        }

        [ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
        void InjectTextParts(HttpRequest request, byte[] textParts)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;

            Type type = request.GetType();

            FieldInfo contentField;
            FieldInfo contentLengthField;

            // Mono
            contentField = type.GetField("_arrRawContent", bindingFlags);

            // ASP.NET
            if (contentField == null)
                contentField = type.GetField("_rawContent", bindingFlags);

            // Mono
            contentLengthField = type.GetField("_iContentLength", bindingFlags);

            // ASP.NET
            if (contentLengthField == null)
                contentLengthField = type.GetField("_contentLength", bindingFlags);

            if (contentLengthField != null)
                contentLengthField.SetValue(request, textParts.Length);

            if (contentField != null && contentField.FieldType == typeof(byte[]))
                contentField.SetValue(request, textParts);
        }

        static bool HasUnmanagedCodePermission()
        {
            return new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).IsUnrestricted();
        }
    }
}
