using System;
using Documents;
using Documents.Enums;
using Infrasturcture.Utils;
using Search;

namespace Services.Contracts
{
    [Serializable]
    public class SpaceObject : ContentObject
    {
        /// <summary>
        /// 空间编号
        /// </summary>
        [SearchIndex(Tokenized = true, IsDelete = true)]
        public Guid Id { get; set; }

        [SearchIndex]
        public string SpaceSeqNo { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        [SearchIndex(Analyzed = true, Name = "Name")]
        public string SpaceName { get; set; }

        /// <summary>
        /// 文件数量
        /// </summary>
        [SearchIndex]
        public int FileCount { get; set; }

        /// <summary>
        /// 父节点Id
        /// </summary>
        [SearchIndex]
        public string ParentId { get; set; }

        /// <summary>
        /// 空间大小
        /// </summary>
        [SearchIndex]
        public double SpaceSize { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [SearchIndex(Analyzed = true, Date = true)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [SearchIndex(Analyzed = true, Date = true)]
        public DateTime UpdateTime { get; set; }

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

        [SearchIndex(Analyzed = true)]
        public DocumentType DocumentType
        {
            get { return DocumentType.Folder; }
        }

        /// <summary>
        /// 是否是默认空间
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }

        [SearchIndex]
        public string DepId { get; set; }

        [SearchIndex]
        public int Visible { get; set; }

        /// <summary>
        /// 缩略图
        /// </summary>
        [SearchIndex]
        public string ThumbUrl { get; set; }

        public string DisplayFileSize
        {
            get { return StringUtils.GetDisplayFileSize(SpaceSize); }
        }
    }
}