using System.Collections.Generic;
using Services.Contracts;

namespace WebSite2.Models
{
    public class UploadViewModel
    {
        public UploadViewModel(int status, string errorMessage)
        {
            this.Status = status;
            this.ErrorMessage = errorMessage;
        }

        public UploadViewModel(int status, string errorMessage, List<DocumentObject> documents)
        {
            this.Documents = documents;
            this.Status = status;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }

        public DocumentObject Document { get; set; }

        /// <summary>
        /// 上传的文档模型
        /// </summary>
        public List<DocumentObject> Documents { get; set; }
    }
}