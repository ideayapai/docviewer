using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Web.Http.Dependencies;
using Ninject;
using Ninject.Syntax;

namespace WebSite2.Common
{
    public class NinjectDependencyScope : IDependencyScope
    {
        private IResolutionRoot resolver;

        internal NinjectDependencyScope(IResolutionRoot resolver)
        {
            Contract.Assert(resolver != null);
            this.resolver = resolver;
        }

        public void Dispose()
        {
            resolver = null;
        }
        public object GetService(Type serviceType)
        {
            return resolver.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return resolver.GetAll(serviceType);
        }
    }
}