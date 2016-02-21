namespace WebSite2.Models
{
    public class UpdateViewModel
    {
        public UpdateViewModel(int status, string errorMessage, string fileType)
        {
            Status = status;
            ErrorMessage = errorMessage;
            FileType = fileType;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 文档类型
        /// </summary>
        public string FileType { get; set; }

    }
}