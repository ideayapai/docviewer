using System.Web.Mvc;
using System.Web.Routing;

namespace WebSite2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(name: "HomePage", url: "{controller}/{action}/{page}", defaults: new { controller = "Home", action = "Index", page = UrlParameter.Optional });
            routes.MapRoute(name: "SpacePage", url: "Home/Space/{spaceid}/page/{page}", defaults: new { controller = "Home", action = "Space", spaceid = UrlParameter.Optional, page = UrlParameter.Optional});
            routes.MapRoute(name: "DocumentIndex", url: "{controller}/{action}/{page}", defaults: new { controller = "Document", action = "Index", page = UrlParameter.Optional });
            routes.MapRoute(name: "DocumentOffice", url: "{controller}/{action}/{page}", defaults: new { controller = "Document", action = "Office", page = UrlParameter.Optional });
            routes.MapRoute(name: "DocumentImage", url: "{controller}/{action}/{page}", defaults: new { controller = "Document", action = "Image", page = UrlParameter.Optional });
            routes.MapRoute(name: "DocumentCAD", url: "{controller}/{action}/{page}", defaults: new { controller = "Document", action = "CAD", page = UrlParameter.Optional });
            routes.MapRoute(name: "DocumentTrash", url: "{controller}/{action}/{page}", defaults: new { controller = "Document", action = "Trash", page = UrlParameter.Optional });
         
            //routes.MapRoute(name: "SpaceAdd", url: "{controller}/{action}", defaults: new { controller = "Space", action = "Add"});

            //忽略SignalR/ping的错误
#if DEBUG
            routes.IgnoreRoute("{*browserlink}", new { browserlink = @".*/arterySignalR/ping" });
#endif
        }
    }
}