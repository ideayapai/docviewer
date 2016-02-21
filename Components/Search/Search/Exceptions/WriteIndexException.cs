using System;
using System.Collections.Generic;

namespace Search.Exceptions
{
    public class WriteIndexException: Exception
    {
        public List<object> ErrorItems { get; private set; }

        public WriteIndexException(List<object> errorItems)
        {
            ErrorItems = errorItems;
        }
    }
}
