using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

using Krystalware.SlickUpload.Configuration;
using Krystalware.SlickUpload.Storage.Streams;
using System.Configuration;
using System.Data.SqlTypes;

namespace Krystalware.SlickUpload.Storage
{
	/// <summary>
	/// An <see cref="IUploadStreamProvider" /> that writes to SQL Server databases.
	/// </summary>
    public class SqlFileStreamUploadStreamProvider : DatabaseUploadStreamProviderBase
	{
		/// <summary>
		/// Creates a new instance of the <see cref="SqlClientUploadStreamProvider" /> class with the specified configuration settings.
		/// </summary>
        /// <param name="settings">The <see cref="SqlFileStreamUploadStreamProvider" /> object that holds the configuration settings.</param>
        public SqlFileStreamUploadStreamProvider(UploadStreamProviderElement settings) :
            base(settings)
		{
            UseInsertTransaction = true;        
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override IDbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override string InsertRecord(UploadedFile file, IDbConnection cn, IDbTransaction t)
        {
            using (SqlCommand cmd = (SqlCommand)cn.CreateCommand())
            {
                StringBuilder insertCommand = new StringBuilder();

                insertCommand.Append("INSERT INTO ");
                insertCommand.Append(Table);
                insertCommand.Append(" (");
                insertCommand.Append(DataField);

                if (FileNameField != null)
                {
                    insertCommand.Append(",");
                    insertCommand.Append(FileNameField);
                }

                insertCommand.Append(") OUTPUT INSERTED." + KeyField);
                insertCommand.Append(" VALUES (CAST('' AS varbinary(MAX))");

                if (FileNameField != null)
                {
                    insertCommand.Append(",@fileName");

                    SqlParameter fileNameParm = cmd.CreateParameter();

                    fileNameParm.ParameterName = "@fileName";
                    fileNameParm.DbType = DbType.String;
                    fileNameParm.Value = file.ClientName;

                    cmd.Parameters.Add(fileNameParm);
                }
                insertCommand.Append(");");

                cmd.CommandText = insertCommand.ToString();
                cmd.Transaction = t as SqlTransaction;

                try
                {
                    if (cn.State != ConnectionState.Open)
                        cn.Open();

                    return cmd.ExecuteScalar().ToString();
                }
                finally
                {
                    if (t == null)
                        cn.Close();
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override DatabaseStreamBase GetWriteDatabaseStream(UploadedFile file, IDbConnection cn, IDbTransaction t)
        {
            return new SqlFileStreamWriteStream(cn, t, Table, DataField, (IDbCommand cmd) => BuildWhereCriteria(file, cmd));
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override DatabaseStreamBase GetReadDatabaseStream(UploadedFile file, IDbConnection cn)
        {
            return new SqlFileStreamReadStream(cn, Table, DataField, (IDbCommand cmd) => BuildWhereCriteria(file, cmd));
        }
    }
}