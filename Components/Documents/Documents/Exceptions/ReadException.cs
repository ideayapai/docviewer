using System;
using Infrasturcture.Errors;

namespace Documents.Exceptions
{
    public class ReadException: Exception
    {
        public ReadException(int errorCode)
            : base(ErrorMessages.GetErrorMessages(errorCode))
        {

        }

        public ReadException(int errorCode, Exception innerException)
            : base(ErrorMessages.GetErrorMessages(errorCode), innerException)
        {

        }
        public ReadException(string message, Exception innerException)
            :base(message, innerException)
        {
            
        }
    }
}
