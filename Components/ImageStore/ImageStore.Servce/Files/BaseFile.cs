using System.IO;

namespace ImageStore.Services.Files
{
    public interface BaseFile
    {
        /// <summary>
        /// 文件流
        /// </summary>
        Stream Stream { get; }

        /// <summary>
        /// 文件名
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// 文件长度
        /// </summary>
        long ContentLength { get; }

        /// <summary>
        /// 关闭文件
        /// </summary>
        void Close();

    }
}
