using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Collections.Specialized;
using System.Web;

namespace Krystalware.SlickUpload.Web
{
    // TODO: implement
//#if DEPLOY
    internal static class MimeHelper
//#endif
//#if !DEPLOY
//    public static class MimeHelper
//#endif
    {
        /*public static string GetContentDispositionValue(string buffer, string key, int pos)
        {
            string prefix = key + "=\"";
            int start = CultureInfo.InvariantCulture.CompareInfo.IndexOf(buffer, prefix, pos, CompareOptions.IgnoreCase);

            if (start == -1)
                return null;

            start += prefix.Length;

            int end = buffer.IndexOf('"', start);

            if (end == -1)
                return null;
            else if (end == start)
                return string.Empty;
            else
                return buffer.Substring(start, end - start);
        }*/

#if DEPLOY
        internal
#else
        public
#endif
        static string[] GetParts(string str, char sep)
        {
            int pos = str.IndexOf(sep);

            if (pos > 0)
                return new string[] { str.Substring(0, pos), str.Substring(pos + 1) };
            else
                return new string[] { str };
        }

#if DEPLOY
        internal
#else
        public
#endif
        static string[] QuotedSemiSplit(string str)
        {
            List<string> strings = new List<string>();

            int start = 0;
            int end = 0;
            int end2 = 0;

            while (end < str.Length)
            {
                end = str.IndexOf('"', start);
                end2 = str.IndexOf(';', start);

                if (end == -1 || end2 < end)
                {
                    end = end2;
                }
                else
                {
                    end = str.IndexOf('"', end + 1);
                    end = str.IndexOf(';', end);
                }

                if (end == -1)
                    end = str.Length;

                strings.Add(str.Substring(start, end - start));

                start = end + 1;
            }

            return strings.ToArray();
        }

#if DEPLOY
        internal
#else
        public
#endif
        static void ParseQueryStringToDictionary(string data, Dictionary<string, string> dictionary)
        {
            NameValueCollection dataCollection = HttpUtility.ParseQueryString(data);

            foreach (string key in dataCollection.AllKeys)
                dictionary[key] = dataCollection[key];
        }
    }
}
