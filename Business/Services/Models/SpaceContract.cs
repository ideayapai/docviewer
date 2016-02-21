using System;
using Documents;
using Documents.Enums;
using Infrasturcture.Utils;

namespace Services.Models
{
    public class SpaceContract:BaseContract
    {
        /// <summary>
        /// 空间编号
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 空间顺序号,对应业务编号
        /// </summary>
        public string SpaceSeqNo { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string SpaceName { get; set; }

        /// <summary>
        /// 文件数量
        /// </summary>
        public int FileCount { get; set; }

        /// <summary>
        /// 父节点Id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 空间大小
        /// </summary>
        public double SpaceSize { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
     
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 创建用户Id
        /// </summary>
      
        public string CreateUserId { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>
    
        public string CreateUserName { get; set; }

   
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

        /// <summary>
        /// 缩略图
        /// </summary>
        public string ThumbUrl { get; set; }

        public string DisplayFileSize
        {
            get { return StringUtils.GetDisplayFileSize(SpaceSize); }
        }
    }
}