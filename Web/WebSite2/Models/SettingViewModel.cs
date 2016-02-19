
using System.Configuration;

namespace WebSite2.Models
{
    public class SettingViewModel: BaseMenuViewModel
    {
        /// <summary>
        /// Mongo DB连接地址
        /// </summary>
        public string MongoConnection
        {
            get { return ConfigurationManager.AppSettings["connection"]; }
        }

        /// <summary>
        /// Mongo DB数据库地址
        /// </summary>
        public string MongoDatabase
        {
            get { return ConfigurationManager.AppSettings["database"]; }
        }

        /// <summary>
        /// 消息队列地址
        /// </summary>
        public string Msmq
        {
            get
            {
                return ConfigurationManager.AppSettings["msmq_address"];
            }
        }

        /// <summary>
        /// 索引存储地址
        /// </summary>
        public string SegmentPath
        {
            get { return ConfigurationManager.AppSettings["segmentPath"]; }
        }

        /// <summary>
        /// 下载地址
        /// </summary>
        public string DownloadUrl
        {
            get { return ConfigurationManager.AppSettings["DownloadUrl"]; }
        }

        /// <summary>
        /// 预览网络地址
        /// </summary>
        public string DisplayUrl
        {
            get { return ConfigurationManager.AppSettings["DisplayUrl"]; }
        }

        /// <summary>
        /// 预览物理存储路径
        /// </summary>
        public string DisplayDir
        {
            get { return ConfigurationManager.AppSettings["DisplayDir"]; }
        }

        /// <summary>
        /// 预览地址
        /// </summary>
        public string ImageDir
        {
            get { return ConfigurationManager.AppSettings["ImageStore.ImageDir"]; }
        }

        /// <summary>
        /// 图片压缩地址
        /// </summary>
        public string CompressImageDir
        {
            get { return ConfigurationManager.AppSettings["ImageStore.CompressImageDir"]; }
        }

        /// <summary>
        /// 缩略图地址
        /// </summary>
        public string ThumbImageDir
        {
            get { return ConfigurationManager.AppSettings["ImageStore.ThumbImageDir"]; }
        }
    }
}