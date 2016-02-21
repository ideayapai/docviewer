using Infrasturcture.Errors;

namespace Services.Models
{
    public class ResponseMessage
    {
        public int ErrorCode { get; set; }

        public string Message { get; set; }

        BaseContract Contract { get; set; }

        public ResponseMessage(BaseContract content)
        {
            Contract = content;
            Message = ErrorMessages.GetErrorMessages(ErrorCode);
        }

        public ResponseMessage(int errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }

        public ResponseMessage(int errorCode, string message, BaseContract conent)
        {
            ErrorCode = errorCode;
            Message = message;
            Contract = conent;
        }
    }
}
