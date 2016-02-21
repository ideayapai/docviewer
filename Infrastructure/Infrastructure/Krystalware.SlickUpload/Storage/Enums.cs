using System;
using System.Collections.Generic;
using System.Text;

namespace Krystalware.SlickUpload.Storage
{
    /// <summary>
    /// Enumeration of the possible data types to use for the file data column.
    /// </summary>
    public enum SqlColumnDataType
    {
        /// <summary>
        /// IMAGE data type.
        /// </summary>
        Image,
        /// <summary>
        /// VARBINARY(MAX) data type.
        /// </summary>
        VarBinaryMax,
        /// <summary>
        /// FILESTREAM data type
        /// </summary>
        FileStream
    }
}
