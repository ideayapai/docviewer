using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Common.Logging;
using Services.Ioc;
using WebSite2.Common;
using WebSite2.Controllers;

namespace WebSite2
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start()
        {
            _logger.Info("应用程序启动");

            //ControllerConfigMachine.ConfigureDependencies(ControllerDependencyInjection.Kernel);
            //NinjectDependencyResolver dependencyResolver = new NinjectDependencyResolver();
            //dependencyResolver.Register<IContactRepository, DefaultContactRepository>();
            //GlobalConfiguration.Configuration.DependencyResolver = dependencyResolver;
            //DependencyResolver.SetResolver(new NinjectDependencyResolver());

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //优化前端代码
            //BundleTable.EnableOptimizations = true;
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            AuthConfig.RegisterAuth();

            ControllerConfigMachine.ConfigureDependencies(ServiceActivator.Kernel);
          
        }

       
        public static void RegisterFovWebApi(HttpConfiguration config)
        {
            config.DependencyResolver = new NinjectDependencyResolverForWebApi(ServiceActivator.Kernel);
        }

    }
}