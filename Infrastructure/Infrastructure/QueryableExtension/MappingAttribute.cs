using System;

namespace Infrasturcture.QueryableExtension
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class MappingAttribute : Attribute
    {
        public string To { get; set; }

        public string From { get; set; }

        public Type ToType { get; set; }

        public Type FromType { get; set; }
    }
}
