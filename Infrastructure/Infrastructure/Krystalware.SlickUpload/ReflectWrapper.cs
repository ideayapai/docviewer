using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Krystalware.SlickUpload
{
    internal class ReflectWrapper
    {
        object _target;
        Type _type;

        public ReflectWrapper(object target)
        {
            _target = target;
            _type = target.GetType();
        }

        internal T Invoke<T>(string name, params object[] parms)
        {
            return (T)_type.InvokeMember(name, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, _target, parms);
        }

        internal void InvokeVoid(string name, params object[] parms)
        {
            _type.InvokeMember(name, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, _target, parms);
        }

        internal T GetProp<T>(string name)
        {
            return (T)_type.InvokeMember(name, BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, _target, null);
        }

        internal void SetProp(string name, object value)
        {
            _type.InvokeMember(name, BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.Instance, null, _target, new object[] { value });
        }

        internal object Target
        {
            get { return _target; }
        }
    }
}
