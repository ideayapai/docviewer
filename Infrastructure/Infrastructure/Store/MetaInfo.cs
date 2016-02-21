using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrasturcture.Store
{
    [Serializable]
    public class MetaInfo
    {
        /// <summary>
        /// 文档Id
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// 文档内容MD5
        /// </summary>
        public string MD5 { get; set; }

        /// <summary>
        /// 文档MimeType
        /// </summary>
        public string MimeType { get; set; }
    }
}
