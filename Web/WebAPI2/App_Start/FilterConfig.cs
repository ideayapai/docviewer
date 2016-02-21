using System.Web.Mvc;

namespace WebAPI2
{
    /// <summary>
    /// 错误机制过滤器
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// 注册错误机制Attribute
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
