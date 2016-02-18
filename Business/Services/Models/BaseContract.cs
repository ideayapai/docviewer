namespace Services.Models
{
    public abstract class BaseContract
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}