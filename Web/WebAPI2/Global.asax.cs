using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Services.Ioc;
using WebAPI2.Common;

namespace WebAPI2
{
    /// <summary>
    /// WebAPI应用程序
    /// </summary>
    public class WebApiApplication : HttpApplication
    {
        /// <summary>
        /// WebAPI引用程序启动入口
        /// </summary>
        protected void Application_Start()
        {
            //允许跨域访问
            GlobalConfiguration.Configuration.EnableCors();

            //支持Jsonp.
            GlobalConfiguration.Configuration.Formatters.Insert(0, new JsonpMediaTypeFormatter());
            
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ControllerConfigMachine.ConfigureDependencies(ServiceActivator.Kernel);

            RegisterFovWebApi(GlobalConfiguration.Configuration);
        }

        private static void RegisterFovWebApi(HttpConfiguration config)
        {
            config.DependencyResolver = new NinjectDependencyResolverForWebApi(ServiceActivator.Kernel);
        }
    }
}
