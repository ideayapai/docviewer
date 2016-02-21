using System.IO;

namespace Infrasturcture.Store.Utils
{
    public static class FileUtils
    {
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
                    path += "\\" + pathes[i];
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }


        }
    }
}
