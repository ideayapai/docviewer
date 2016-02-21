using System;
using System.Configuration;
using System.IO;
using Common.Logging;
using Infrasturcture.Utils;
using Services.Exceptions;

namespace Services.Documents
{
    /// <summary>
    /// 设置文档的下载，显示，转换，存储地址
    /// 需要根据网站的实际地址进行配置
    /// </summary>
    public static class DocumentSettings
    {
        private static readonly ILog _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 获取文档下载路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetDownloadUrl(string fileName)
        {
            string downloadUrl = GetConfigValue("DownloadUrl");

            if (string.IsNullOrWhiteSpace(downloadUrl))
            {
                _logger.Error("DownloadUrl 没有配置");
                throw new SettingException("DownloadUrl 没有配置");
            }

            return downloadUrl + fileName;
        }


        /// <summary>
        /// 获取文档显示路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetDisplayUrl(string fileName)
        {
            string displayUrl = GetConfigValue("DisplayUrl");

            if (string.IsNullOrWhiteSpace(displayUrl))
            {
                _logger.Error("DisplayUrl 没有配置");
                throw new SettingException("DisplayUrl 没有配置");
            }

            return displayUrl + fileName;
        }

        /// <summary>
        /// 获取临时文件存储路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetDownloadTempDir()
        {
            string dir = GetConfigValue("DownloadTempDir");

            if (string.IsNullOrWhiteSpace(dir))
            {
                _logger.Error("DownloadTempDir 没有配置");
                throw new SettingException("DownloadTempDir 没有配置");
            }

            return dir;
        }

        /// <summary>
        /// 获取文档显示路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetPreviewUrl(string fileName)
        {
            string previewUrl = GetConfigValue("PreviewUrl");

            if (string.IsNullOrWhiteSpace(previewUrl))
            {
                _logger.Error("PreviewUrl 没有配置");
                throw new SettingException("PreviewUrl 没有配置");
            }

            return previewUrl + fileName;
        }

        /// <summary>
        /// 获取文档存储路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetStorePath(string fileName)
        {
            fileName = DateTime.Now.ToString("yyyy_mm_dd_hh_mm_ssss") + "_" + Path.GetFileName(fileName);
            fileName = StringUtils.RemoveAllEmpty(fileName);
            string storedir = GetConfigValue("StoreDir");

            if (string.IsNullOrWhiteSpace(storedir))
            {
                _logger.Error("StoreDir 没有配置");
                throw new SettingException("StoreDir 没有配置");
            }

            if (!storedir.EndsWith("\\"))
            {
                storedir += "\\";
            }

            return storedir + fileName;
        }

        /// <summary>
        /// 获取文档转换地址
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetConvertPath(string fileName)
        {
            string displaydir = GetConfigValue("DisplayDir");

            if (string.IsNullOrWhiteSpace(displaydir))
            {
                _logger.Error("DisplayDir 没有配置");
                throw new SettingException("DisplayDir 没有配置");
            }
            if (!displaydir.EndsWith("\\"))
            {
                displaydir += "\\";
            }
            return displaydir + fileName;
        }

        private static string GetConfigValue(string configSettingName)
        {
            return ConfigurationManager.AppSettings[configSettingName];
        }


       
    }
}
