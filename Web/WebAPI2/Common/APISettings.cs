using System.Web;

namespace WebAPI2.Common
{
    /// <summary>
    /// API设置
    /// </summary>
    public static class ApiSettings
    {
        /// <summary>
        /// 当前Url
        /// </summary>
        public static string GetCurrentUrl
        {
            get
            {
                string host = HttpContext.Current.Request.Url.Host;
                int port = HttpContext.Current.Request.Url.Port;

                return string.Format("http://{0}:{1}", host, port);
            }
         
        }
    }
}