using System.Web.Mvc;
using WebSite2.Filters;

namespace WebSite2
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomHandlerErrorAttribute());
        }
    }
}