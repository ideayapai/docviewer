using System.Collections.Specialized;
using System.Web;
using Infrasturcture.Store.Files;

namespace Infrasturcture.Web
{
    public interface IHttpRequestProvider
    {
        string GetMapPath(string path);

        string ClientIP { get; }
        
        bool IsAuthenticated { get; }

        string UserAgent { get; }

        BaseFileCollection FileCollection { get; }

        HttpRequest GetRequest();
        
        NameValueCollection GetForm();
        
        HttpContext HttpContext { get; }

        NameValueCollection GetQueryString();

        string this[string key]
        {
            get;
        }

        string UrlDecode(string source);
    }
}
