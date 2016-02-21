using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

using Krystalware.SlickUpload.Configuration;
using Krystalware.SlickUpload.Storage.Streams;
using System.Configuration;

namespace Krystalware.SlickUpload.Storage
{
	/// <summary>
	/// An <see cref="IUploadStreamProvider" /> that writes to SQL Server IMAGE columns.
	/// </summary>
    public class SqlClientUploadStreamProvider : DatabaseUploadStreamProviderBase
	{
        SqlColumnDataType _dataType;

        /// <summary>
        /// Gets the configured data type for the file data field.
        /// </summary>
        protected SqlColumnDataType DataType { get { return _dataType; } }
        
        /// <summary>
		/// Creates a new instance of the <see cref="SqlClientUploadStreamProvider" /> class with the specified configuration settings.
		/// </summary>
        /// <param name="settings">The <see cref="UploadStreamProviderElement" /> object that holds the configuration settings.</param>
        public SqlClientUploadStreamProvider(UploadStreamProviderElement settings) :
            base(settings)
		{
            string dataTypeString = Settings.Parameters["dataType"];

            if (!string.IsNullOrEmpty(dataTypeString))
                _dataType = (SqlColumnDataType)Enum.Parse(typeof(SqlColumnDataType), dataTypeString, true);
            else
                _dataType = SqlColumnDataType.Image;

            if (_dataType == SqlColumnDataType.FileStream)
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

                if (_dataType != SqlColumnDataType.FileStream)
                {
                    insertCommand.Append(") VALUES (NULL");
                }
                else
                {
                    insertCommand.Append(") OUTPUT INSERTED." + KeyField);
                    insertCommand.Append(" VALUES (CAST('' AS varbinary(MAX))");
                }

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

                if (_dataType != SqlColumnDataType.FileStream)
                    insertCommand.Append("SELECT SCOPE_IDENTITY();");

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
            if (_dataType != SqlColumnDataType.FileStream)
                return new SqlClientWriteStream(cn, t, Table, DataField, (IDbCommand cmd) => BuildWhereCriteria(file, cmd), _dataType);
            else
                return new SqlFileStreamWriteStream(cn, t, Table, DataField, (IDbCommand cmd) => BuildWhereCriteria(file, cmd));
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override DatabaseStreamBase GetReadDatabaseStream(UploadedFile file, IDbConnection cn)
        {
            if (_dataType != SqlColumnDataType.FileStream)
                return new SqlClientReadStream(cn, Table, DataField, (IDbCommand cmd) => BuildWhereCriteria(file, cmd), _dataType);
            else
                return new SqlFileStreamReadStream(cn, Table, DataField, (IDbCommand cmd) => BuildWhereCriteria(file, cmd));
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override void CloseWriteStream(UploadedFile file, Stream stream, bool isComplete)
        {
            if (_dataType == SqlColumnDataType.FileStream)
            {
                ((SqlFileStreamWriteStream)stream).CompleteAndClose(isComplete);
            }
            else
            {
                base.CloseWriteStream(file, stream, isComplete);
            }
        }
    }
}