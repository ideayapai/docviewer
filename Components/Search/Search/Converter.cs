using System;
using System.Globalization;
using System.Reflection;

namespace Search
{
    public static class Converter
    {
        public static void SetValue(this object obj, PropertyInfo propertyInfo, string value)
        {
            var propertyType = propertyInfo.PropertyType;
            if (propertyType == typeof(Guid))
            {
                propertyInfo.SetValue(obj, ConvertToGuid(value), null);
            }
            else if (propertyType == typeof(bool))
            {
                propertyInfo.SetValue(obj, ConvertToBoolean(value), null);
            }
            else if (propertyType == typeof(DateTime))
            {
                propertyInfo.SetValue(obj, ConvertToDateTime(value), null);
            }
            else if (propertyType == typeof(int))
            {
                propertyInfo.SetValue(obj, ConvertStringToInt(value), null);
            }
            else if (propertyType == typeof(decimal))
            {
                propertyInfo.SetValue(obj, ConvertStringToDecimal(value), null);
            }
            else if (propertyType == typeof(double))
            {
                propertyInfo.SetValue(obj, ConvertStringToDouble(value), null);
            }
            else
            {
                propertyInfo.SetValue(obj, value, null);
            }
        }

        public static Guid ConvertToGuid(this string str)
        {
            if (string.IsNullOrEmpty(str) || str == "NULL")
                return Guid.Empty;
            return new Guid(str);
        }

        public static bool ConvertToBoolean(this string str)
        {
            if (string.IsNullOrEmpty(str) || str == "NULL")
                return false;
            return bool.Parse(str);
        }

        public static DateTime ConvertToDateTime(this string str)
        {
            if (string.IsNullOrEmpty(str) || str == "NULL")
            {
                return DateTime.MinValue;
            }

            DateTime dateTime = new DateTime();
            if (DateTime.TryParseExact(str, "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTime))
            {
                return dateTime;
            }

            return DateTime.Parse(str, CultureInfo.CurrentCulture);
        }

        public static int ConvertStringToInt(this string str)
        {
            if (string.IsNullOrEmpty(str) || str == "NULL")
                return 0;
            return int.Parse(str);
        }

        public static decimal ConvertStringToDecimal(this string str)
        {
            if (string.IsNullOrEmpty(str) || str == "NULL")
                return 0;
            return decimal.Parse(str);
        }

        public static double ConvertStringToDouble(this string str)
        {
            if (string.IsNullOrEmpty(str) || str == "NULL")
                return 0;
            return double.Parse(str);
        }

        public static string ConvertEmptyToZero(this string str)
        {
            if (string.IsNullOrEmpty(str) || str == "NULL")
                return "0";
            return str;
        }
        public static double? ConvertToNullableDouble(object val)
        {
            try
            {
                return Convert.ToDouble(val);
            }
            catch
            {
            }
            return null;
        }

        public static double ConvertToDouble(object val)
        {
            try
            {
                return Convert.ToDouble(val);
            }
            catch
            {
            }
            return 0d;
        }



        public static Guid ConvertToGuid(object val)
        {
            try
            {
                return Guid.Parse(val.ToString());
            }
            catch
            {
            }
            return Guid.Empty;
        }

        public static string ConvertToString(object val)
        {
            try
            {
                return val.ToString();
            }
            catch
            {
            }
            return string.Empty;
        }

        public static int ConvertToInt(object val)
        {
            try
            {
                return int.Parse(val.ToString());
            }
            catch
            {
            }
            return 0;
        }

        public static DateTime ConvertToDateTime(object val)
        {
            try
            {
                return DateTime.Parse(val.ToString());
            }
            catch
            {
            }
            return DateTime.MinValue;
        }
    }
}
