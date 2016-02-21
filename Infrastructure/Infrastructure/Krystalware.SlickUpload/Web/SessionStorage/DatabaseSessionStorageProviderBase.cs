using System;
using System.Collections.Generic;
using System.Text;
using Krystalware.SlickUpload.Configuration;
using System.Configuration;
using System.Data;
using System.Reflection;

namespace Krystalware.SlickUpload.Web.SessionStorage
{
    /// <summary>
    /// Exposes an abstract base class for database <see cref="ISessionStorageProvider"/>s that use the System.Data interfaces.
    /// </summary>
    public abstract class DatabaseSessionStorageProviderBase : SessionStorageProviderBase
    {
        string _connectionString;
        string _table;
        string _sessionIdField;
        string _requestIdField;
        string _statusField;
        string _lastUpdatedField;
        
        string _sessionIdParameterName = "@sessionId";
        string _requestIdParameterName = "@requestId";
        string _statusParameterName = "@status";
        string _lastUpdatedParameterName = "@updated";

        /// <summary>
        /// Gets or sets the name to use for the session id parameter.
        /// </summary>
        protected string SessionIdParameterName
        {
            get { return _sessionIdParameterName; }
            set { _sessionIdParameterName = value; }
        }
        /// <summary>
        /// Gets or sets the name to use for the request id parameter.
        /// </summary>
        protected string RequestIdParameterName
        {
            get { return _requestIdParameterName; }
            set { _requestIdParameterName = value; }
        }
        /// <summary>
        /// Gets or sets the name to use for the status data parameter.
        /// </summary>
        protected string StatusParameterName
        {
            get { return _statusParameterName; }
            set { _statusParameterName = value; }
        }
        /// <summary>
        /// Gets or sets the name to use for the last updated parameter.
        /// </summary>
        protected string LastUpdatedParameterName
        {
            get { return _lastUpdatedParameterName; }
            set { _lastUpdatedParameterName = value; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="DatabaseSessionStorageProviderBase" /> class with the specified settings.
        /// </summary>
        /// <param name="settings">The <see cref="SessionStorageProviderElement" /> object that holds the configuration settings.</param>
        public DatabaseSessionStorageProviderBase(SessionStorageProviderElement settings)
            : base(settings)
        {
            string connectionStringName = settings.Parameters["connectionStringName"];

            if (!string.IsNullOrEmpty(connectionStringName))
                // TODO: also get type
                _connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            else
                _connectionString = settings.Parameters["connectionString"];

            if (string.IsNullOrEmpty(_connectionString))
                throw new Exception("sessionStorageProvider connectionString or connectionStringName attribute must be specified.");

            _table = settings.Parameters["table"];
            _sessionIdField = settings.Parameters["sessionIdField"];
            _requestIdField = settings.Parameters["requestIdField"];
            _statusField = settings.Parameters["statusField"];
            _lastUpdatedField = settings.Parameters["lastUpdatedField"];

            if (string.IsNullOrEmpty(_table))
                throw new Exception("sessionStorageProvider table attribute must be specified.");
            if (string.IsNullOrEmpty(_sessionIdField))
                throw new Exception("sessionStorageProvider sessionIdField attribute must be specified.");
            if (string.IsNullOrEmpty(_requestIdField))
                throw new Exception("sessionStorageProvider requestIdField attribute must be specified.");
            if (string.IsNullOrEmpty(_statusField))
                throw new Exception("sessionStorageProvider statusField attribute must be specified.");
            if (string.IsNullOrEmpty(_lastUpdatedField))
                throw new Exception("sessionStorageProvider lastUpdatedField attribute must be specified.");

        }

        /// <inheritdoc />
        public override void SaveSession(UploadSession session, bool isCreate)
        {
            InsertOrUpdate(session.UploadSessionId, null, session.Serialize(), DateTime.Now, isCreate);
        }

        /// <inheritdoc />
        public override UploadSession GetSession(string uploadSessionId)
        {
            string data = GetData(uploadSessionId, null);

            if (!string.IsNullOrEmpty(data))
                return UploadSession.Deserialize(data);
            else
                return null;
        }

        /// <inheritdoc />
        public override void RemoveSession(string uploadSessionId)
        {
            string sql;

            sql = "DELETE FROM " + _table + " WHERE " + _sessionIdField + "=" + _sessionIdParameterName;

            using (IDbConnection cn = CreateConnection(_connectionString))
            using (IDbCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = sql;

                AddParameter(cmd, _sessionIdParameterName, DbType.String, uploadSessionId);

                cn.Open();

                cmd.ExecuteNonQuery();
            }
        }

        /// <inheritdoc />
        public override void SaveRequest(UploadRequest request, bool isCreate)
        {
            InsertOrUpdate(request.UploadSessionId, request.UploadRequestId, request.Serialize(), DateTime.Now, isCreate);
        }

        /// <inheritdoc />
        public override UploadRequest GetRequest(string uploadSessionId, string uploadRequestId)
        {
            string data = GetData(uploadSessionId, uploadRequestId);

            if (!string.IsNullOrEmpty(data))
                return UploadRequest.Deserialize(data);
            else
                return null;
        }

        /// <inheritdoc />
        public override IEnumerable<UploadRequest> GetRequestsForSession(string uploadSessionId)
        {
            List<UploadRequest> requests = new List<UploadRequest>();

            string sql;

            sql = "SELECT " + _statusField + " FROM " + _table + " WHERE " + _sessionIdField + "=" + _sessionIdParameterName + " AND NOT " + _requestIdField + " IS NULL";

            using (IDbConnection cn = CreateConnection(_connectionString))
            using (IDbCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = sql;

                AddParameter(cmd, _sessionIdParameterName, DbType.String, uploadSessionId);

                cn.Open();

                using (IDataReader rd = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (rd.Read())
                        requests.Add(UploadRequest.Deserialize(rd.GetString(0)));
                }
            }

            return requests;
        }

        /// <inheritdoc />
        public override IEnumerable<UploadSession> GetStaleSessions(DateTime staleBefore)
        {
            List<UploadSession> sessions = new List<UploadSession>();

            string sql;

            sql = "SELECT " + _statusField + " FROM " + _table + " AS kw_sessions WHERE " + _requestIdField + " IS NULL AND (SELECT MAX(" + _lastUpdatedField + ") FROM " + _table + " AS kw_status WHERE kw_status." + _sessionIdField + "=kw_sessions." + _sessionIdField + ") < " + _lastUpdatedParameterName;

            using (IDbConnection cn = CreateConnection(_connectionString))
            using (IDbCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = sql;

                AddParameter(cmd, _lastUpdatedParameterName, DbType.DateTime, staleBefore);

                cn.Open();

                using (IDataReader rd = cmd.ExecuteReader(CommandBehavior.SingleResult))
                {
                    while (rd.Read())
                        sessions.Add(UploadSession.Deserialize(rd.GetString(0)));
                }
            }

            return sessions; 
        }

        /// <summary>
        /// Creates an <see cref="IDbConnection" /> using the settings used to construct this <see cref="DatabaseSessionStorageProviderBase" />.
        /// </summary>
        /// <returns>The <see cref="IDbConnection" />.</returns>
        protected abstract IDbConnection CreateConnection(string connectionString);

        /// <summary>
        /// Inserts or updates the specified serialized <see cref="UploadSession" /> or <see cref="UploadRequest" /> data.
        /// </summary>
        /// <param name="uploadSessionId">The upload session id.</param>
        /// <param name="uploadRequestId">The upload request id, or null to save only the session.</param>
        /// <param name="data">The upload session or upload request data.</param>
        /// <param name="lastUpdated">The last updated <see cref="DateTime" />.</param>
        /// <param name="isCreate">A boolean that specifies whether this is the first (create) call or a subsequent (update) call.</param>
        protected virtual void InsertOrUpdate(string uploadSessionId, string uploadRequestId, string data, DateTime lastUpdated, bool isCreate)
        {
            string sql;

            if (isCreate)
            {
                sql = "INSERT INTO " + _table + " (" + _statusField + "," + _lastUpdatedField + "," + _sessionIdField;

                if (!string.IsNullOrEmpty(uploadRequestId))
                    sql += "," + _requestIdField;

                sql += ") VALUES (" + _statusParameterName + "," + _lastUpdatedParameterName + "," + _sessionIdParameterName;

                if (!string.IsNullOrEmpty(uploadRequestId))
                    sql += "," + _requestIdParameterName;

                sql += ")";
            }
            else
            {
                sql = "UPDATE " + _table + " SET " + _statusField + "=" + _statusParameterName + ", " + _lastUpdatedField + "=" + _lastUpdatedParameterName + " WHERE " + _sessionIdField + "=" + _sessionIdParameterName;

                if (!string.IsNullOrEmpty(uploadRequestId))
                    sql += " AND " + _requestIdField + "=" + _requestIdParameterName;
                else
                    sql += " AND " + _requestIdField + " IS NULL";
            }

            using (IDbConnection cn = CreateConnection(_connectionString))
            using (IDbCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = sql;

                AddParameter(cmd, _statusParameterName, DbType.String, data);
                AddParameter(cmd, _lastUpdatedParameterName, DbType.DateTime, lastUpdated);
                AddParameter(cmd, _sessionIdParameterName, DbType.String, uploadSessionId);

                if (!string.IsNullOrEmpty(uploadRequestId))
                    AddParameter(cmd, _requestIdParameterName, DbType.String, uploadRequestId);

                cn.Open();

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Gets the serialized string for an <see cref="UploadSession" /> or <see cref="UploadRequest" />.
        /// </summary>
        /// <param name="uploadSessionId">The upload session id to retrieve.</param>
        /// <param name="uploadRequestId">The upload request id to retrieve, or null to retrieve only the upload session.</param>
        /// <returns>The serialized string, or null if none exists.</returns>
        protected virtual string GetData(string uploadSessionId, string uploadRequestId)
        {
            string sql;

            sql = "SELECT " + _statusField + " FROM " + _table + " WHERE " + _sessionIdField + "=" + _sessionIdParameterName;

            if (!string.IsNullOrEmpty(uploadRequestId))
                sql += " AND " + _requestIdField + "=" + _requestIdParameterName;

            using (IDbConnection cn = CreateConnection(_connectionString))
            using (IDbCommand cmd = cn.CreateCommand())
            {
                cmd.CommandText = sql;

                AddParameter(cmd, _sessionIdParameterName, DbType.String, uploadSessionId);

                if (!string.IsNullOrEmpty(uploadRequestId))
                    AddParameter(cmd, _requestIdParameterName, DbType.String, uploadRequestId);

                cn.Open();

                return cmd.ExecuteScalar() as string;
            }
        }

        /// <summary>
        /// Adds a parameter to the specified <see cref="IDbCommand" />.
        /// </summary>
        /// <param name="cmd">The <see cref="IDbCommand" /> to which to add the paramater.</param>
        /// <param name="name">The name of the paramater.</param>
        /// <param name="type">The <see cref="DbType" /> of the paramater.</param>
        /// <param name="value">The value for the parameter.</param>
        protected virtual void AddParameter(IDbCommand cmd, string name, DbType type, object value)
        {
            IDbDataParameter param = cmd.CreateParameter();

            if (name != "?")
                param.ParameterName = name;

            param.DbType = type;
            param.Value = value;

            cmd.Parameters.Add(param);
        }
    }
}