﻿using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Ninject;

namespace WebAPI2.Common
{
    /// <summary>
    /// Controller依赖注入
    /// </summary>
    public class ControllerConfigMachine
    {
        /// <summary>
        /// 注入IKernel
        /// </summary>
        /// <param name="kernel"></param>
        public static void ConfigureDependencies(IKernel kernel)
        {
            var controllerFactory = new DependentInjectionControllerFactory(kernel);

            ControllerBuilder.Current.SetControllerFactory(controllerFactory);

            //预先绑顶Controlller,保证线程安全
            foreach (Type type in Assembly.GetExecutingAssembly().GetExportedTypes().Where(IsController))
                controllerFactory.Kernel.Bind(type).ToSelf();
        }


        private static bool IsController(Type type)
        {
            return typeof(IController).IsAssignableFrom(type) && type.IsPublic && !type.IsAbstract && !type.IsInterface;
        }
    }
}