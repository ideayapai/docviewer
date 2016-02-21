using System;

namespace Infrasturcture.Utils
{
    public static class StringUtils
    {
        public const double Byte = 1;
        public const double KB = Byte * 1024;
        public const double MB = KB * 1024;
        public const double GB = MB * 1024;
        public const double TB = GB * 1024;
        public const double PB = TB * 1024;

        public static string GetDisplayFileSize(double fileSize)
        {
            if (fileSize < KB)
            {
                return String.Format("{0} 字节", fileSize);
            }
            if (fileSize >= KB && fileSize < MB)
            {
                return String.Format("{0} KB", (fileSize / KB).ToString("0.00"));
            }
            if (fileSize >= MB && fileSize < GB)
            {
                return String.Format("{0} MB", (fileSize / MB).ToString("0.00"));     
            }
            if (fileSize >= GB && fileSize < TB)
            {
                return String.Format("{0} GB", (fileSize / GB).ToString("0.00"));     
            }
            if (fileSize >= TB && fileSize < PB)
            {
                return String.Format("{0} TB", (fileSize / TB).ToString("0.00"));
            }
            if (fileSize >= PB )
            {
                return String.Format("{0} PB", (fileSize / PB).ToString("0.00"));
            }
            return string.Empty;
            
        }

        public static string RemoveAllEmpty(string savePath)
        {
            int preLength = savePath.Length;
            int nowLength = 0;
            for (; nowLength < preLength; )
            {
                preLength = savePath.Length;
                savePath = savePath.Replace(" ", "");
                nowLength = savePath.Length;
            }
            return savePath;
        }

        /// <summary>
        /// 截取子字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubString(this string str, int start, int length)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            if (str.Length < length)
            {
                return str.Substring(start, str.Length);
            }
            return str.Substring(start, length);
        }

        public static string GetWithoutBlankName(this string fileName, string extension)
        {
            return RemoveAllEmpty(fileName + extension);
        }
        
    }
}
