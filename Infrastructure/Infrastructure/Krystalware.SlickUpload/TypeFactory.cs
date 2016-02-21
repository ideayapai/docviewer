using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Krystalware.SlickUpload
{
    internal class TypeFactory
    {
        internal static object CreateInstance(string reference, object[] parameters)
        {
            return CreateInstance(reference, parameters, BindingFlags.Default);
        }

        internal static object CreateInstance(string reference, object[] parameters, BindingFlags flags)
        {
            string[] components;

            if (!string.IsNullOrEmpty(reference))
                components = reference.Split(new char[] { ',' }, 2);
            else
                components = new string[] { string.Empty, string.Empty };

            Assembly a = null;

            if (components.Length == 1)
            {
                try
                {
                    a = Assembly.Load("App_Code");
                }
                catch
                {
                    // TODO: better error message?
                    return null;
                }
            }
            else
                a = Assembly.Load(components[1].Trim());

            try
            {
                // First try with params
                return a.CreateInstance(components[0].Trim(), false, flags, null, parameters, null, null);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            catch
            {
                // Then try default
                return a.CreateInstance(components[0].Trim(), false, flags, null, null, null, null);
            }
        }

        internal static T ParseEnum<T>(string value, T defaultValue)
        {
            if (!string.IsNullOrEmpty(value))
                return (T)Enum.Parse(typeof(T), value, true);
            else
                return defaultValue;
        }
    }
}
