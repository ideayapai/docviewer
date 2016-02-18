using System;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using Infrasturcture.Web;
using Services.Contracts;

namespace Services.Context
{
    public class ContextService
    {
        public const string USER_ID_SESSION_KEY = "CurrentUserId";
        public const string USER_EMAIL_SESSION_KEY = "CurrentUserEmail";
        public const string USER_NICKNAME_SESSION_KEY = "CurrentUserNickname";
        public const string DEP_ID_SESSION_KEY = "DepId";
        public const string DEFAULT_SPACE_ID_SESSION_KEY = "DefaultSpaceId";
        public const string CUR_SPACE_SESSION_KEY = "CurrentSpace";

        private static ContextService _instance;

        private readonly ISessionProvider _sessionProvider;

        private static readonly Hashtable _cachePolicy = new Hashtable();

        /// <summary>
        /// 空的构造函数是为单元测试
        /// </summary>
        public ContextService()
        {
            
        }

        public ContextService(ISessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public static ContextService Instance()
        {
            return _instance ?? (_instance = new ContextService(new DefaultSessionProvider()));
        }

        public static ContextService Current
        {
            get { return Instance(); }
            set { _instance = value; }
        }

        public void Close()
        {
            _sessionProvider.RemoveAll();
            _sessionProvider.Abandon();
        }

        public void SetCookie(string key, string value)
        {
            if (FormsAuthentication.CookiesSupported)
            {
                var cookie = HttpContext.Current.Request.Cookies[key];
                if (cookie == null)
                {
                    cookie = new HttpCookie(key, HttpUtility.UrlEncode(value, Encoding.GetEncoding("UTF-8")));
                }
                else
                {
                    cookie.Value = HttpUtility.UrlEncode(value, Encoding.GetEncoding("UTF-8"));
                }
                cookie.Expires = DateTime.Now.AddDays(1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }

        public void RemoveCookie()
        {
            if (FormsAuthentication.CookiesSupported)
            {
                HttpContext.Current.Request.Cookies.Clear();
            }
        }

        public string GetCookieValue(string key)
        {
            if (FormsAuthentication.CookiesSupported)
            {
                var cookie = HttpContext.Current.Request.Cookies[key];
                if (cookie != null)
                    return HttpUtility.UrlDecode(cookie.Value, Encoding.GetEncoding("UTF-8"));
            }
            return string.Empty;
        }

        public virtual string UserId
        {
            get { return (string)_sessionProvider.Get(USER_ID_SESSION_KEY);}
            set { _sessionProvider.Set(USER_ID_SESSION_KEY, value);}
        }

        public virtual string NickName
        {
            get { return (string)_sessionProvider.Get(USER_NICKNAME_SESSION_KEY); }
            set { _sessionProvider.Set(USER_NICKNAME_SESSION_KEY, value); }
        }

        public virtual string DefaultSpaceId
        {
            get { return (string)_sessionProvider.Get(DEFAULT_SPACE_ID_SESSION_KEY); }
            set { _sessionProvider.Set(DEFAULT_SPACE_ID_SESSION_KEY, value); }
        }

        public virtual string DepId
        {
            get { return (string)_sessionProvider.Get(DEP_ID_SESSION_KEY); }
            set { _sessionProvider.Set(DEP_ID_SESSION_KEY, value); }
        }

        public string UserPhoto
        {
            get { return Convert.ToString(_sessionProvider.Get("UserPhoto")); }
            set { _sessionProvider.Set("UserPhoto", value); }
        }

        public virtual SpaceObject CurrentSpace
        {
            get { return (SpaceObject)_sessionProvider.Get(CUR_SPACE_SESSION_KEY); }
            set { _sessionProvider.Set(CUR_SPACE_SESSION_KEY, value); }
        }


        public static object Get(string key)
        {
            return !_cachePolicy.ContainsKey(key) ? null : _cachePolicy[key];
        }

        public static void Set(string key, object obj)
        {
            if (_cachePolicy.ContainsKey(key))
            {
                _cachePolicy[key] = obj;
            }
            else
            {
                _cachePolicy.Add(key, obj);
            }
        }

        public string SpaceTreeHtml
        {
            set
            {
                Set("spaceTree", value);
            }

            get
            {
                return Get("spaceTree") as string;
            }
        }
    }
}