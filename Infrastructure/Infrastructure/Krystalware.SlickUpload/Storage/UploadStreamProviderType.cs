using System;
using System.Collections.Generic;
using System.Text;

namespace Krystalware.SlickUpload.Storage
{
    /// <summary>
    /// Enumeration of the possible upload stream providers.
    /// </summary>
    public enum UploadStreamProviderType
    {
        /// <summary>
        /// The built-in <see cref="FileUploadStreamProvider" /> that writes to the file system.
        /// </summary>
        File,
        /*
        /// <summary>
        /// The built-in <see cref="OracleBlobUploadStreamProvider" /> that writes to an Oracle database using OracleBlob.
        /// </summary>
        OracleBlob,*/
        /// <summary>
        /// The built-in <see cref="SqlClientUploadStreamProvider" /> that writes to a SQL Server database.
        /// </summary>
        SqlClient,
        /// <summary>
        /// The built-in <see cref="SqlFileStreamUploadStreamProvider" /> that writes to a SQL Server database using FILESTREAM.
        /// </summary>
        SqlFileStream,
        /// <summary>
        /// A custom provider.
        /// </summary>
        Custom
    }
}
