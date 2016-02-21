using System.IO;

namespace Documents.Reader
{
    internal interface IReader
    {
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        string Read(string filePath);

        /// <summary>
        /// 读取流
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        string Read(Stream inputStream);
    }
}
