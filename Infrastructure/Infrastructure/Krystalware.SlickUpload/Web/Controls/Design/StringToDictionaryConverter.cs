using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace Krystalware.SlickUpload.Web.Controls.Design
{
    public class StringToDictionaryConverter : TypeConverter
    {
        public class SelfDictionary : Dictionary<string, string>
        {
            public SelfDictionary(string s)
            {
                try
                {
                    // TODO: make this more resiliant
                    foreach (string part in s.Split(','))
                    {
                        string[] values = part.Split('=', ':');

                        Add(values[0].Trim(), values[1].Trim());
                    }
                }
                catch
                {
                    throw new FormatException("Couldn't parse Data property value \"" + s + "\". Value must be a dictionary string in the format of \"key=value,key2=value2\".");
                }
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                ConstructorInfo constructorInfo = typeof(SelfDictionary).GetConstructor(new Type[] { typeof(string) });
                InstanceDescriptor instanceDescriptor = new InstanceDescriptor(constructorInfo, new object[] { value });
                return instanceDescriptor;
            }
            else if (destinationType == typeof(string))
            {
                StringBuilder sb = new StringBuilder();

                foreach (KeyValuePair<string, string> pair in (Dictionary<string, string>)value)
                {
                    if (sb.Length > 0)
                        sb.Append(',');

                    sb.Append(pair.Key + "=" + pair.Value);
                }

                return sb.ToString();
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                return new SelfDictionary((string)value);
                //return value;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
    }
}
