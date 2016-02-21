using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Krystalware.SlickUpload.Web
{
    // TODO: implement
//#if DEPLOY
    internal static class JsonHelper
//#endif
//#if !DEPLOY
//    public static class JsonHelper
//#endif
    {
#if DEPLOY
        internal
#else
        public
#endif
        static string Serialize(Dictionary<string, string> data)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append('{');

            foreach (KeyValuePair<string, string> pair in data)
                AppendJson(sb, pair.Key, pair.Value);

            sb.Append('}');

            return sb.ToString();
        }

#if DEPLOY
        internal
#else
        public
#endif
        static void AppendJson(StringBuilder sb, string key, string value)
        {
            if (sb.Length > 1)
                sb.Append(",");

            if (value != null)
                value = value.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\\", "\\\\").Replace("\"", "\\\"");

            sb.Append(key + " : \"" + value + "\"");
        }

#if DEPLOY
        internal
#else
        public
#endif
        static void AppendJson(StringBuilder sb, string key, float value)
        {
            if (sb.Length > 1)
                sb.Append(",");

            sb.Append(key + " : " + value.ToString("##0.00", CultureInfo.InvariantCulture));
        }

#if DEPLOY
        internal
#else
        public
#endif
        static void AppendJson(StringBuilder sb, string key, long value)
        {
            if (sb.Length > 1)
                sb.Append(",");

            sb.Append(key + " : " + value.ToString());
        }

    }
}
