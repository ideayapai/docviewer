#if NET35
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Reflection;
using System.ComponentModel;
using System.Reflection.Emit;

namespace Krystalware.SlickUpload.Web.Mvc
{
    /// <summary>
    /// Maps an upload request to a model.
    /// </summary>
    [Obfuscation(Feature = "hiding-place")]
    public static class UploadSessionModelBinderFactory
    {
        public static object CreateInstance()
        {
            AssemblyName aName = new AssemblyName("UploadSessionModelBinder");
            AppDomain appDomain = System.Threading.Thread.GetDomain();
            AssemblyBuilder aBuilder = appDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
            ModuleBuilder module = aBuilder.DefineDynamicModule("UploadSessionModelBinder");

            TypeBuilder type = module.DefineType(
                "Krystalware.SlickUpload.Web.Mvc.UploadSessionModelBinder",
                TypeAttributes.Public | TypeAttributes.Class,
                typeof(Object),
                new Type[]{
            typeof(IModelBinder)
            }
                );
            //return type;

            // Declaring method builder
            // Method attributes
            System.Reflection.MethodAttributes methodAttributes =
                  System.Reflection.MethodAttributes.Public
                | System.Reflection.MethodAttributes.Virtual
                | System.Reflection.MethodAttributes.Final
                | System.Reflection.MethodAttributes.HideBySig
                | System.Reflection.MethodAttributes.NewSlot;
            MethodBuilder method = type.DefineMethod("BindModel", methodAttributes);
            // Preparing Reflection instances
            MethodInfo method1 = typeof(SlickUploadContext).GetMethod(
                "get_CurrentUploadSession",
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new Type[]{
            },
                null
                );
            // Setting return type
            method.SetReturnType(typeof(Object));
            // Adding parameters
            method.SetParameters(
                typeof(ControllerContext),
                typeof(ModelBindingContext)
                );
            // Parameter controllerContext
            ParameterBuilder controllerContext = method.DefineParameter(1, ParameterAttributes.None, "controllerContext");
            // Parameter bindingContext
            ParameterBuilder bindingContext = method.DefineParameter(2, ParameterAttributes.None, "bindingContext");
            ILGenerator gen = method.GetILGenerator();
            // Preparing locals
            LocalBuilder locals = gen.DeclareLocal(typeof(Object));
            // Preparing labels
            Label label9 = gen.DefineLabel();
            // Writing body
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Call, method1);
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Br_S, label9);
            gen.MarkLabel(label9);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);
            // finished
            return type.CreateType().InvokeMember(null, BindingFlags.CreateInstance, null, null, null);

        }
    }

    // TODO: figure out why this breaks adding control to toolbox if mvc isn't installed
    /*public class UploadSessionModelBinder : IModelBinder
    {
        /// <see cref="IModelBinder.BindModel" copy="true" />
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            return SlickUploadContext.CurrentUploadSession;
        }
    }*/
}
#endif