using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Web.Http.Dependencies;
using Ninject;
using Ninject.Syntax;

namespace WebAPI2.Common
{
    /// <summary>
    /// 依赖注入
    /// </summary>
    public class NinjectDependencyScope : IDependencyScope
    {
        private IResolutionRoot resolver;

        internal NinjectDependencyScope(IResolutionRoot resolver)
        {
            Contract.Assert(resolver != null);
            this.resolver = resolver;
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        public void Dispose()
        {
            resolver = null;
        }

        /// <summary>
        /// GetService
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            return resolver.TryGet(serviceType);
        }

        /// <summary>
        /// GetServices
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return resolver.GetAll(serviceType);
        }
    }
}