using System;

namespace Search
{
    public class SearchIndexAttribute: Attribute
    {
        /// <summary>
        /// 索引的名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否是删除主键
        /// </summary>
        public bool IsDelete { get; set; }

        /// <summary>
        /// 是否分词
        /// </summary>
        public bool Analyzed { get; set; }

        /// <summary>
        /// 是否索引
        /// </summary>
        public bool Tokenized { get; set; }

        /// <summary>
        /// 是否关联文件
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 是否为日期
        /// </summary>
        public bool Date { get; set; }

    }
}
