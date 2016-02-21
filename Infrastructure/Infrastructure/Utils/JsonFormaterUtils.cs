using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Infrasturcture.Utils
{
    /// <summary>
    /// Json formater helper class
    /// </summary>
    public static class JsonFormaterUtils
    {
        /// <summary>
        /// Serialize a object to json data
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object instance</param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
        {
            return Serialize(obj, Encoding.UTF8);


        }

        /// <summary>
        /// Serialize a object to json data
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="obj">Object instance</param>
        /// <param name="encoding">Specific encoding</param>
        /// <returns></returns>
        public static string Serialize<T>(T obj, Encoding encoding)
        {
            var jsonFormater = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                jsonFormater.WriteObject(ms, obj);
                ms.Position = 0;
                using (var sr = new StreamReader(ms, encoding))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Deserialize json data to a object using default encoding
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="jsonDatas">Json data</param>
        /// <returns></returns>
        public static T Deserialize<T>(string jsonDatas)
        {
            return Deserialize<T>(jsonDatas, Encoding.UTF8);
        }

        /// <summary>
        /// Deserialize json data to a object using specific encoding
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="jsonDatas">Json data</param>
        /// <param name="encoding">Specific encoding</param>
        /// <returns></returns>
        public static T Deserialize<T>(string jsonDatas, Encoding encoding)
        {
            var jsonFormater = new DataContractJsonSerializer(typeof(T));
            byte[] buffer = encoding.GetBytes(jsonDatas);
            using (var ms = new MemoryStream(buffer))
            {
                ms.Position = 0;
                return (T)jsonFormater.ReadObject(ms);
            }
        }
    }
}


