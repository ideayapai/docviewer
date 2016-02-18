using System;

namespace Services.Exceptions
{
    public class SettingException : ApplicationException
    {
        public SettingException() { }

        public SettingException(string message)
            : base(message)
        { }


        public SettingException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
