using System;

namespace Services.Models
{
    public class SearchContract : BaseContract
    {
        /// <summary>
        /// 空间编号
        /// </summary>
    
        public Guid Id { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
    
        public string Name { get; set; }

        /// <summary>
        /// 文件简介
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
     
        public double Size { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdateTime { get; set; }

        /// <summary>
        /// 创建用户Id
        /// </summary>
        public int CreateUserId { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>
        public string CreateUserName { get; set; }

        public string DocumentType { get; set; }
    }
}
