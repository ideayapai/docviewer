using System.IO;

namespace Documents.Converter
{
    public interface IConverter
    {
        /// <summary>
        /// 文档转换从一个文件转换到另一个文件
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        int Convert(string from, string to);

        /// <summary>
        /// 文档转换从一个流转换为另一个流
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        int Convert(Stream from, Stream to);
    }
}
