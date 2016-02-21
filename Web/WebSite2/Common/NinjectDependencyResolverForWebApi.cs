using System;
using System.Web.Http.Dependencies;
using Ninject;

namespace WebSite2.Common
{
    public class NinjectDependencyResolverForWebApi : NinjectDependencyScope, IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolverForWebApi(IKernel kernel)
            : base(kernel)
        {
            if (kernel == null)
            {
                throw new ArgumentNullException("kernel");
            }
            this.kernel = kernel;
        }
        public IDependencyScope BeginScope()
        {
            return new NinjectDependencyScope(kernel);
        }
    }
}