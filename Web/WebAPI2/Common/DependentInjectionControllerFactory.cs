using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;

namespace WebAPI2.Common
{
    /// <summary>
    /// Controller依赖注入工厂
    /// </summary>
    public class DependentInjectionControllerFactory : DefaultControllerFactory
    {
        private readonly IKernel _kernel;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="container"></param>
        public DependentInjectionControllerFactory(IKernel container)
        {
            _kernel = container;
        }

        /// <summary>
        /// 依赖注入Kernel
        /// </summary>
        public IKernel Kernel
        {
            get
            {
                return _kernel;
            }
        }
        
        /// <summary>
        /// 依赖注入生成Controller
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="controllerType"></param>
        /// <returns></returns>
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
             if (controllerType == null)
            {
                throw new HttpException(0x194,
                                        string.Format(CultureInfo.CurrentUICulture, @"未发现指定的 Controller {0}。",
                                                      requestContext.HttpContext.Request.Path));
            }
           
            if (!typeof(Controller).IsAssignableFrom(controllerType))
            {
                throw new ArgumentException(string.Format("Type requested is not a controller: {0}", controllerType.Name),
                                           "controllerType");
            }
               
            var controller = _kernel.Get(controllerType) as Controller;
            controller.ActionInvoker = _kernel.Get<ControllerActionInvoker>();
            return controller;
        }

        
    }
}

