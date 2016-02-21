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
	/// An <see cref="IUploadStreamProvider" /> that writes to SQL Server databases.
	/// </summary>
    public sealed class OracleBlobUploadStreamProvider : UploadStreamProviderBase
	{
		/// <summary>
		/// Enumeration of the methods to use when generating criteria.
		/// </summary>
		public enum CriteriaMethod
		{
            /// <summary>
            /// Get the next value from the specified sequence and use that to insert.
            /// </summary>
            Sequence,
            /// <summary>
			/// Assume the key field has a trigger that autopopulates from a sequence and just insert.
			/// </summary>
			SequenceTrigger,
            /// <summary>
            /// Use a <see cref="ICriteriaGenerator" /> to generate the criteria.
            /// </summary>
            Custom
		}

		string _connectionString;
		string _table;
		string _keyField;
		string _dataField;
		string _fileNameField;
        string _sequenceName;
        internal static readonly string _odpAssembly = GetOdpAssembly();

        CriteriaMethod _criteriaMethod;
        ICriteriaGenerator _criteriaGenerator;

        /// <summary>
        /// The <see cref="UploadedFile.LocationInfo" /> key used to store the where criteria value.
        /// </summary>
        public const string WhereCriteriaKey = "whereCriteria";

        /// <summary>
        /// The <see cref="UploadedFile.LocationInfo" /> key used to store the sequence value.
        /// </summary>
        public const string SequenceValueKey = "sequenceValue";

		/// <summary>
		/// Creates a new instance of a <see cref="SqlClientUploadStreamProvider" /> with the specified configuration settings.
		/// </summary>
        /// <param name="configuration">The <see cref="NameValueConfigurationSection" /> object that holds the configuration settings.</param>
        public OracleBlobUploadStreamProvider(UploadStreamProviderElement configuration)
		{
            string criteriaMethodString = configuration["criteriaMethod"];

            if (criteriaMethodString != null && criteriaMethodString.Length != 0)
                _criteriaMethod = (CriteriaMethod)Enum.Parse(typeof(CriteriaMethod), criteriaMethodString, true);
            else
                _criteriaMethod = CriteriaMethod.SequenceTrigger;

            if (_criteriaMethod == CriteriaMethod.Custom)
            {
                _criteriaGenerator = TypeCache.GetInstance(configuration["criteriaGenerator"], configuration) as ICriteriaGenerator;

                if (_criteriaGenerator == null)
                    throw new ApplicationException("Could not instantiate criteria generator.");
            }

#if NET2
            string connectionStringName = configuration["connectionStringName"];

            if (!string.IsNullOrEmpty(connectionStringName))
                _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            else
#endif
                _connectionString = configuration["connectionString"];

            _table = configuration["table"];
            _keyField = configuration["keyField"];
            _dataField = configuration["dataField"];
            _fileNameField = configuration["fileNameField"];
            _sequenceName = configuration["sequenceName"];
		}

        /// <summary>
        /// Returns a <see cref="SqlClientInputStream" /> to which to write the file as it is uploaded.
        /// </summary>
        /// <param name="file">The <see cref="UploadedFile" /> for which to get a <see cref="Stream" />.</param>
        /// <returns>A <see cref="SqlClientInputStream" /> to which to write the <see cref="UploadedFile" />.</returns>
        public override Stream GetOutputStream(UploadedFile file)
        {
            string whereCriteria;

            if (_criteriaMethod == CriteriaMethod.Sequence || _criteriaMethod == CriteriaMethod.SequenceTrigger)
            {
                using (IDbConnection cn = CreateConnection(_connectionString))
                using (IDbCommand cmd = cn.CreateCommand())
                {
                    StringBuilder insertCommand = new StringBuilder();

                    insertCommand.Append("INSERT INTO ");
                    insertCommand.Append(_table);
                    insertCommand.Append(" (");
                    insertCommand.Append(_dataField);

                    if (_criteriaMethod == CriteriaMethod.Sequence)
                    {
                        insertCommand.Append(",");
                        insertCommand.Append(_keyField);
                    }

                    if (_fileNameField != null)
                    {
                        insertCommand.Append(",");
                        insertCommand.Append(_fileNameField);
                    }

                    insertCommand.Append(") VALUES (NULL");

                    if (_criteriaMethod == CriteriaMethod.Sequence)
                    {
                        insertCommand.Append(",\"");
                        insertCommand.Append(_sequenceName);
                        insertCommand.Append("\".nextval");
                    }

                    if (_fileNameField != null)
                    {
                        insertCommand.Append(",:fileName");

                        IDataParameter fileNameParm = cmd.CreateParameter();

                        fileNameParm.ParameterName = ":fileName";
                        fileNameParm.DbType = DbType.String;
                        fileNameParm.Value = file.ClientName;

                        cmd.Parameters.Add(fileNameParm);
                    }

                    IDataParameter returnedIdParm = cmd.CreateParameter();

                    returnedIdParm.ParameterName = ":returnedId";
                    returnedIdParm.DbType = DbType.Decimal;
                    returnedIdParm.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(returnedIdParm);

                    insertCommand.Append(") RETURNING " + _keyField + " INTO :returnedId");

                    cmd.CommandText = insertCommand.ToString();

                    cn.Open();

                    cmd.ExecuteNonQuery();

                    string id = returnedIdParm.Value.ToString();

                    file.LocationInfo[SequenceValueKey] = id;

                    whereCriteria = _keyField + "=" + id;
                }
            }
            else
            {
                whereCriteria = _criteriaGenerator.GenerateCriteria(file);
            }

            try
            {
                file.LocationInfo[WhereCriteriaKey] = whereCriteria;

                return new OracleBlobInputStream(_connectionString, _table, _dataField, whereCriteria);
            }
            catch
            {
                RemoveOutput(file);

                file.LocationInfo.Clear();

                throw;
            }
        }

        /// <summary>
        /// Removes the output record for the specified upload.
        /// </summary>
        /// <param name="file">The <see cref="UploadedFile" /> for which to remove the output.</param>
        public override void RemoveOutput(UploadedFile file)
        {
            // If it's a valid upload
            if (file.LocationInfo.Count > 0)
            {
                using (IDbConnection cn = CreateConnection(_connectionString))
                using (IDbCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM " + _table + " WHERE " + file.LocationInfo[WhereCriteriaKey];

                    cn.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="SqlClientOutputStream" /> of the file data that was uploaded for the specified location info.
        /// </summary>
        /// <param name="file">The <see cref="UploadedFile" /> for which to get a <see cref="SqlClientOutputStream" />.</param>
        /// <returns>A <see cref="SqlClientOutputStream" /> of the file data that was uploaded.</returns>
        public override Stream GetInputStream(UploadedFile file)
        {
            return new OracleBlobOutputStream(_connectionString, _table, _dataField, file.LocationInfo[WhereCriteriaKey]);
        }

        internal static IDbConnection CreateConnection(string connectionString)
        {
            return (IDbConnection)TypeCache.CreateInstance("Oracle.DataAccess.Client.OracleConnection, " + _odpAssembly, new object[] { connectionString });
        }

        static string GetOdpAssembly()
        {
            string configuredAssembly = SlickUploadConfiguration.UploadStreamProvider["odpAssembly"];

            if (!string.IsNullOrEmpty(configuredAssembly))
                return configuredAssembly;
            else
                return "Oracle.DataAccess, Version=10.2.0.100, Culture=neutral, PublicKeyToken=89b483f429c47342";
        }
    }
}