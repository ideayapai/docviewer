using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Krystalware.SlickUpload.Configuration;

namespace Krystalware.SlickUpload.Web.SessionStorage
{
    /// <summary>
    /// An <see cref="ISessionStorageProvider" /> that stores upload session state in a SQL Server database.
    /// </summary>
    public class SqlClientSessionStorageProvider : DatabaseSessionStorageProviderBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="SqlClientSessionStorageProvider" /> class with the specified settings.
        /// </summary>
        /// <param name="settings">The <see cref="SessionStorageProviderElement" /> object that holds the configuration settings.</param>
        public SqlClientSessionStorageProvider(SessionStorageProviderElement settings) :
            base(settings)
        { }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
