namespace WebAPI2.Models
{
    /// <summary>
    /// 通用返回类
    /// </summary>
    public abstract class BaseContract
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrorCode { get; set; }
        
        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}