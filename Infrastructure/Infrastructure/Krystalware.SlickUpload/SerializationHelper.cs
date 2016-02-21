using System;
using System.Collections.Generic;
using System.Text;

namespace Krystalware.SlickUpload
{
    internal class SerializationHelper
    {
        internal static Dictionary<string, string> DeserializeDictionary(object value)
        {
            string[][] values = value as string[][];

            Dictionary<string, string> dict = new Dictionary<string,string>();

            if (values != null && values.Length > 0)
            {
                for (var i = 0; i < values.Length; i++)
                    dict[values[i][0]] = values[i][1];
            }

            return dict;
        }

        internal static object SerializeDictionary(Dictionary<string, string> dict)
        {
            if (dict != null && dict.Count > 0)
            {
                string[][] values = new string[dict.Count][];
                int i = 0;

                foreach (KeyValuePair<string, string> pair in dict)
                    values[i++] = new string[] { pair.Key, pair.Value };

                return values;
            }
            else
            {
                return null;
            }
        }

        // TODO: perf test
        /*string SerializeDictionary(Dictionary<string, string> d)
        {
            if (d != null && d.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (KeyValuePair<string, string> pair in d)
                {
                    if (sb.Length > 0)
                        sb.Append("&");

                    sb.Append(pair.Key + "=");

                    if (!string.IsNullOrEmpty(pair.Value))
                        sb.Append(HttpUtility.UrlEncode(pair.Value));
                }

                return sb.ToString();
            }
            else
            {
                return null;
            }
        }

        static Dictionary<string, string> DeserializeDictionary(string d)
        {
            if (!string.IsNullOrEmpty(d))
            {
                Dictionary<string, string> statusDictionary = new Dictionary<string, string>();
                NameValueCollection dCollection = HttpUtility.ParseQueryString(d);

                foreach (string key in dCollection.AllKeys)
                    statusDictionary[key] = dCollection[key];

                return statusDictionary;
            }
            else
                return null;
        }*/
    }
}
