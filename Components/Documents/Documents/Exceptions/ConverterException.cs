using System;
using Infrasturcture.Errors;

namespace Documents.Exceptions
{
    public class ConverterException: Exception
    {
        public ConverterException(int errorCode)
            : base(ErrorMessages.GetErrorMessages(errorCode))
        {

        }

        public ConverterException(int errorCode, Exception innerException)
            : base(ErrorMessages.GetErrorMessages(errorCode), innerException)
        {

        }
        public ConverterException(string message, Exception innerException)
            :base(message, innerException)
        {
            
        }
    }
}
