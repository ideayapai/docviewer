using System;
using Ninject;

namespace Services.Ioc
{
    /// <summary>
    /// 依赖注入框架
    /// </summary>
    public class ServiceActivator
    {        
        private readonly IKernel _kernel;
        private static readonly ServiceActivator _instance = new ServiceActivator();

        private ServiceActivator()
        {
            _kernel = new StandardKernel(new ServiceBindingModule(),
                                         new RepositoryBindingModule(),
                                         new CommonBindingModule());
        }

        public static ServiceActivator Instance
        {
            get
            {
                return _instance;
            }
        }

        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }

        public static object Get(Type type)
        {
            return Kernel.Get(type);
        }

        public static IKernel Kernel
        {
            get { return Instance._kernel; }
        }   

       
    }
}