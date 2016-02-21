using System;
using System.Drawing;
using System.IO;
using Common.Logging;

namespace ImageStore.Services.Utils
{
    public static class FileUtils
    {
        private static readonly ILog _logger = LogManager.GetCurrentClassLogger();

       
        /// <summary>
        /// 生成日历格式的图片名称
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static string GenerateImageName(string ext)
        {
            return DateTime.Now.ToString("yyyyMMdd") + Guid.NewGuid().GetShortNumber() + ext;
        }

        /// <summary>
        /// 保存图片并返回生成的图片名称
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="ext"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string SaveImage(Stream stream, string ext, string directory)
        {
            _logger.InfoFormat("Saveing Image ext:{0}, directory:{1}", ext, directory);

            using (Image img = Image.FromStream(stream))
            {
                string fileName = GenerateImageName(ext);

                CreateFolderIfNotExist(directory);
                string filePath = Path.Combine(directory, fileName);

                img.Save(filePath);
                return fileName;
            }
        }
       
        /// <summary>
        /// 创建文件如果不存在
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static void CreateFolderIfNotExist(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                return;
            }

            string dirpath = string.Empty;

            if (folderPath.EndsWith("\\"))
            {
                dirpath = folderPath.Substring(0, folderPath.LastIndexOf('\\'));
            }
            else
            {
                dirpath = folderPath;
            }
            
            string[] pathes = dirpath.Split('\\');
            if (pathes.Length > 1)
            {
                string path = pathes[0];
                for (int i = 1; i < pathes.Length; i++)
                {
                    if (!path.StartsWith("\\"))
                    {
                        path += "\\" + pathes[i];
                    }
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }


        }
    }
    

}
