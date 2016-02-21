using System.Collections.Specialized;
using System.Web;
using Infrasturcture.Store.Files;

namespace Infrasturcture.Web
{
    public class DefaultRequestProvider : IHttpRequestProvider
    {
        public virtual string GetMapPath(string path)
        {
            
            return HttpContext.Current.Server.MapPath(path);
        }


        public string UserAgent
        {
            get { return HttpContext.Current.Request.UserAgent; }
        }

        public BaseFileCollection FileCollection
        {
            get
            {
                BaseFileCollection fileCollection = new BaseFileCollection();

                for (var i = 0; i < HttpContext.Current.Request.Files.Count; ++i)
                {
                    var file = new HttpPostedFileWrapper(HttpContext.Current.Request.Files[i]);
                    fileCollection.Add(new HttpFile(file));
                }
                return fileCollection;
            }
        }

        public HttpRequest GetRequest()
        {
            return HttpContext.Current.Request;
        }

      

        public HttpContext HttpContext
        {
            get
            {
                return HttpContext.Current;
            }
        }

        public string ClientIP
        {
            get
            {
                try
                {
                    return HttpContext != null ? HttpContext.Request["REMOTE_ADDR"] : string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public NameValueCollection GetForm()
        {
            return GetRequest().Form;
        }

        public string this[string key]
        {
            get
            {
                return HttpContext.Request[key];
            }
        }

        public string UrlDecode(string source)
        {

            return HttpContext.Server.UrlDecode(source);
        }


        public NameValueCollection GetQueryString()
        {
            return GetRequest().QueryString;
        }

        public bool IsAuthenticated
        {
            get
            {
                return HttpContext.Current.User.Identity.IsAuthenticated;
            }
        }
    }
}
