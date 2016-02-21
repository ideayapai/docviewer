using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Common.Logging;

namespace Infrasturcture.Utils
{
    public class SerializerUtils
    {
        private static readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public static string Serialize<T>(T obj)
        {

            using (var ms = new MemoryStream())
            {
                try
                {
                    var settings = new XmlWriterSettings
                    {
                        Encoding = Encoding.UTF8
                    };

                    using (XmlWriter writer = XmlWriter.Create(ms, settings))
                    {
                        var serialize = new XmlSerializer(obj.GetType());

                        serialize.Serialize(writer, obj);
                        ms.Seek(0, SeekOrigin.Begin);
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                }
                return string.Empty;
            }
        }

        public static string Serialize<T>(T obj, XmlSerializerNamespaces ns)
        {
            using (var ms = new MemoryStream())
            {
                try
                {
                    var settings = new XmlWriterSettings
                    {
                        Encoding = Encoding.UTF8
                    };
                    using (XmlWriter writer = XmlWriter.Create(ms, settings))
                    {
                        var serialize = new XmlSerializer(obj.GetType());

                        serialize.Serialize(writer, obj, ns);
                        ms.Seek(0, SeekOrigin.Begin);
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e.Message);
                }
                return string.Empty;
            }
        }

        public static T Deserialize<T>(Stream stream) where T : class
        {
            using (var reader = new StreamReader(stream))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var obj = reader.ReadToEnd();
                return DoDeserialize<T>(obj.Replace("\r", "").Replace("\n", ""));
            }
        }

        public static T Deserialize<T>(string obj) where T : class
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return DoDeserialize<T>(obj.Replace("\r", "").Replace("\n", ""));
            }
            return null;
        }

        private static T DoDeserialize<T>(string str) where T : class
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var writer = new StreamWriter(ms, Encoding.UTF8))
                    {
                        writer.Write(str);
                        writer.Flush();
                        ms.Seek(0, SeekOrigin.Begin);
                        var serializer = new XmlSerializer(typeof(T), "");
                        return serializer.Deserialize(ms) as T;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return null;
            }
        }
    }
}


