using System;
using System.Web.Http.Dependencies;
using Ninject;

namespace WebAPI2.Common
{
    /// <summary>
    /// WebAPI依赖注入
    /// </summary>
    public class NinjectDependencyResolverForWebApi : NinjectDependencyScope, IDependencyResolver
    {
        private IKernel kernel;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="kernel"></param>
        public NinjectDependencyResolverForWebApi(IKernel kernel)
            : base(kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }
            this.kernel = kernel;
        }

        /// <summary>
        /// BeginScope
        /// </summary>
        /// <returns></returns>
        public IDependencyScope BeginScope()
        {
            return new NinjectDependencyScope(kernel);
        }
    }
}