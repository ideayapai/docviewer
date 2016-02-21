using System;

namespace Services.Exceptions
{
    public class ItemNotExistException : ApplicationException
    {
        public ItemNotExistException() { }

        public ItemNotExistException(string message)
            : base(message)
        { }

        public ItemNotExistException(string message, Exception innerException)
            : base(message, innerException)
        { }

        
    }

    public class NullException : ApplicationException
    {
         public NullException() { }

        public NullException(string message)
            : base(message)
        { }

        public NullException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

}