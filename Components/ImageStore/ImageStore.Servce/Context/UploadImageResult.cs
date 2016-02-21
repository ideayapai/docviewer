namespace ImageStore.Services.Context
{
    /// <summary>
    /// 这是新的上传方式返回的结果
    /// </summary>
    public class UploadImageResult
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string ImageName { get; set; }

        /// <summary>
        /// 原始图片网络路径
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 等比例压缩后的网络路径
        /// </summary>
        public string CompressImageUrl { get; set; }

        /// <summary>
        /// 生成小图后的网络路径
        /// </summary>
        public string ThumbImageUrl { get; set; }

        /// <summary>
        /// 返回的状态码
        /// </summary>
        public int ReturnCode { get; set; }
        
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "ImageName:" + ImageName + " " +
                   "ImageUrl:" + ImageUrl + " " +
                   "ThumbImageUrl:" + ThumbImageUrl + " " +
                   "ReturnCode:" + ReturnCode + " " +
                   "Success:" + Success;
        }
    }
}
