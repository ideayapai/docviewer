using System.Collections.Generic;
using Lucene.Net.Documents;

namespace Search.Imp
{
    public class SearchResult
    {
        /// <summary>
        /// 文档次数
        /// </summary>
        public int TotalHits { get; set; }

        /// <summary>
        /// 搜索结果
        /// </summary>
        public List<Document> Docs { get; set; }
 
        /// <summary>
        /// 构造函数
        /// </summary>
        public SearchResult(){ }
 
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="totalHits"></param>
        /// <param name="docs"></param>
        public SearchResult(int totalHits, List<Document> docs)
        {
            this.TotalHits = totalHits;
            this.Docs = docs;
        }
 
    }
}
