using System.Web;

namespace Infrasturcture.Web
{
    public static class CerCommon
    {
        /// <summary>
        /// 获得IP
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            string ip;

            if (HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
            else
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            return ip;
        }
    }
}
