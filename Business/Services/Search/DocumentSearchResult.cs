using System;
using Search;

namespace Services.Search
{
    public class DocumentSearchResult
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [SearchIndex]
        public Guid Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [SearchIndex]
        public string Name { get; set; }

        /// <summary>
        /// 文件简介
        /// </summary>
        [SearchIndex]
        public string Content { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        [SearchIndex]
        public double Size { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SearchIndex]
        public string CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [SearchIndex]
        public string UpdateTime { get; set; }

        /// <summary>
        /// 创建用户Id
        /// </summary>
        [SearchIndex]
        public string CreateUserId { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>
        [SearchIndex]
        public string CreateUserName { get; set; }

        
        [SearchIndex]
        public string DepId { get; set; }

        [SearchIndex]
        public string UserId { get; set; }

        [SearchIndex]
        public int Visible { get; set; }

        /// <summary>
        /// 文档类型:CAD,Word,Excel等
        /// </summary>
        [SearchIndex]
        public string DocumentType { get; set; }

        /// <summary>
        /// 预览图
        /// </summary>
        [SearchIndex]
        public string ThumbUrl { get; set; }

        /// <summary>
        /// 预览图
        /// </summary>
        [SearchIndex]
        public string DisplayPath { get; set; }


        /// <summary>
        /// 下载地址
        /// </summary>
        [SearchIndex]
        public string DownloadPath { get; set; }
    }
}