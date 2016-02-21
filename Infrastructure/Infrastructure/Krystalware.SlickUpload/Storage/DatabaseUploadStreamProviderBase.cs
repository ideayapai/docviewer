using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using Krystalware.SlickUpload.Configuration;
using System.Configuration;
using Krystalware.SlickUpload.Storage.Streams;

namespace Krystalware.SlickUpload.Storage
{
    /// <summary>
    /// An action that generates where criteria for the specified command. It may also add parameters to the command to support a parameterized where criteria.
    /// </summary>
    /// <param name="cmd">The <see cref="IDbCommand" /> for which to generate criteria, and optionally add paramaters.</param>
    /// <returns>The where criteria string.</returns>
    public delegate string BuildWhereCriteriaAction(IDbCommand cmd);

    /// <summary>
    /// Exposes an abstract base class for database <see cref="IUploadStreamProvider"/>s that use the System.Data interfaces.
    /// </summary>
    public abstract class DatabaseUploadStreamProviderBase : UploadStreamProviderBase
    {        
        string _connectionString;
		string _table;
		string _keyField;
		string _dataField;
		string _fileNameField;

        bool _useInsertTransaction;
        IsolationLevel? _isolationLevel;

        /// <summary>
        /// Gets the configured connection string.
        /// </summary>
        protected string ConnectionString { get { return _connectionString; } }
        /// <summary>
        /// Gets the configured table name.
        /// </summary>
        protected string Table { get { return _table; } }
        /// <summary>
        /// Gets the configured key field name.
        /// </summary>
        protected string KeyField { get { return _keyField; } }
        /// <summary>
        /// Gets the configured data field name.
        /// </summary>
        protected string DataField { get { return _dataField; } }
        /// <summary>
        /// Gets the configured file name field.
        /// </summary>
        protected string FileNameField { get { return _fileNameField; } }

        /// <summary>
        /// Gets or sets a boolean that specifies whether to use a transaction.
        /// </summary>
        protected bool UseInsertTransaction
        {
            get { return _useInsertTransaction; }
            set { _useInsertTransaction = value; }
        }

        /// <summary>
        /// Gets the configured <see cref="IsolationLevel"/>.
        /// </summary>
        protected IsolationLevel? IsolationLevel { get { return _isolationLevel; } }

        /// <summary>
        /// Creates an <see cref="IDbConnection" /> using the settings used to construct this <see cref="DatabaseUploadStreamProviderBase" />.
        /// </summary>
        /// <returns>The <see cref="IDbConnection" />.</returns>
        public abstract IDbConnection CreateConnection();

        /// <summary>
        /// Inserts an initial record for the specified <see cref="UploadedFile" />.
        /// </summary>
        /// <param name="file">The <see cref="UploadedFile" /> for which to insert a record.</param>
        /// <param name="connection">The <see cref="IDbConnection" /> to use.</param>
        /// <param name="transaction">The <see cref="IDbTransaction" /> to use.</param>
        /// <returns>The id of the inserted record.</returns>
        public abstract string InsertRecord(UploadedFile file, IDbConnection connection, IDbTransaction transaction);

        /// <summary>
        /// Returns a write stream for the specified <see cref="UploadedFile" />.
        /// </summary>
        /// <param name="file">The <see cref="UploadedFile" /> for which to get a write stream.</param>
        /// <param name="connection">The <see cref="IDbConnection" /> to use.</param>
        /// <param name="transaction">The <see cref="IDbTransaction" /> to use.</param>
        /// <returns>The write <see cref="DatabaseStreamBase" /> for the specified <see cref="UploadedFile" />.</returns>
        public abstract DatabaseStreamBase GetWriteDatabaseStream(UploadedFile file, IDbConnection connection, IDbTransaction transaction);

        /// <summary>
        /// Returns a read stream for the specified <see cref="UploadedFile" />.
        /// </summary>
        /// <param name="file">The <see cref="UploadedFile" /> for which to get a read stream.</param>
        /// <param name="connection">The <see cref="IDbConnection" /> to use.</param>
        /// <returns>The read <see cref="DatabaseStreamBase" /> for the specified <see cref="UploadedFile" />.</returns>
        public abstract DatabaseStreamBase GetReadDatabaseStream(UploadedFile file, IDbConnection connection);

        /// <summary>
        /// Generates a where criteria string for the specified <see cref="UploadedFile" />, adding parameters to the specified <see cref="IDbCommand" />.
        /// </summary>
        /// <param name="file">The <see cref="UploadedFile" /> for which to generate criteria.</param>
        /// <param name="cmd">The <see cref="IDbCommand" /> to which to add criteria parameters.</param>
        /// <returns>The generated where criteria string.</returns>
        public virtual string BuildWhereCriteria(UploadedFile file, IDbCommand cmd)
        {
            IDbDataParameter param = cmd.CreateParameter();

            param.ParameterName = "@keyValue";
            param.DbType = DbType.String;
            param.Value = file.ServerLocation;

            cmd.Parameters.Add(param);

            return _keyField + "=@keyValue";
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override Stream GetWriteStream(UploadedFile file)
        {
            IDbConnection cn = null;
            IDbTransaction t = null;

            try
            {
                cn = CreateConnection();
                cn.Open();

                if (_useInsertTransaction)
                {
                    if (_isolationLevel != null)
                        t = cn.BeginTransaction(_isolationLevel.Value);
                    else
                        t = cn.BeginTransaction();
                }

                file.ServerLocation = InsertRecord(file, cn, t);

                return GetWriteDatabaseStream(file, cn, t);
            }
            catch
            {
                if (t != null)
                    t.Dispose();
                else
                    RemoveOutput(file);

                if (cn != null)
                {
                    cn.Dispose();

                    cn = null;
                }

                file.ServerLocation = null;

                throw;
            }
            /*finally
            {
                if (t == null && cn != null)
                    cn.Dispose();
            }*/
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override Stream GetReadStream(UploadedFile file)
        {
            IDbConnection cn = null;

            try
            {
                cn = CreateConnection();

                return GetReadDatabaseStream(file, cn);
            }
            catch
            {
                if (cn != null)
                    cn.Dispose();

                throw;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override void RemoveOutput(UploadedFile file)
        {
            using (IDbConnection cn = CreateConnection())
            using (IDbCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM " + _table + " WHERE " + BuildWhereCriteria(file, cmd);

                cn.Open();

                cmd.ExecuteNonQuery();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override void CloseWriteStream(UploadedFile file, Stream stream, bool isComplete)
        {
            DatabaseStreamBase databaseStream = stream as DatabaseStreamBase;

            if (databaseStream != null && databaseStream.Transaction != null)
            {
                if (isComplete)
                    databaseStream.Transaction.Commit();
            }

            base.CloseWriteStream(file, stream, isComplete);
        }

        /// <summary>
        /// Creates a new instance of <see cref="DatabaseUploadStreamProviderBase" /> class with the specified settings.
        /// </summary>
        /// <param name="settings">The <see cref="UploadStreamProviderElement" /> object that holds the configuration settings.</param>
        protected DatabaseUploadStreamProviderBase(UploadStreamProviderElement settings)
            : base(settings)
        {
            string connectionStringName = Settings.Parameters["connectionStringName"];

            if (!string.IsNullOrEmpty(connectionStringName))
                _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            else
                _connectionString = Settings.Parameters["connectionString"];

            if (string.IsNullOrEmpty(_connectionString))
                throw new Exception("uploadStreamProvider connectionString or connectionStringName attribute must be specified.");
                
            _table = Settings.Parameters["table"];
            _keyField = Settings.Parameters["keyField"];
            _dataField = Settings.Parameters["dataField"];
            _fileNameField = Settings.Parameters["fileNameField"];

            if (string.IsNullOrEmpty(_table))
                throw new Exception("uploadStreamProvider table attribute must be specified.");
            if (string.IsNullOrEmpty(_keyField))
                throw new Exception("uploadStreamProvider keyField attribute must be specified.");
            if (string.IsNullOrEmpty(_dataField))
                throw new Exception("uploadStreamProvider dataField attribute must be specified.");

            // TODO: throw exception for missing parameters

            bool.TryParse(Settings.Parameters["useInsertTransaction"], out _useInsertTransaction);

            string isolationLevelString = Settings.Parameters["useInsertTransaction"];

            if (!string.IsNullOrEmpty(isolationLevelString))
                _isolationLevel = (IsolationLevel)Enum.Parse(typeof(IsolationLevel), isolationLevelString, true);
        }
    }
}
