using System.Configuration;

namespace Infrasturcture.Store.Configuration
{
    /// <summary>
    /// MongoDB节点配置
    /// </summary>
    public static class MongoSettings
    {
        public static string ConnectionString
        {
            get
            {
                var connection = ConfigurationManager.AppSettings["connection"];
                return connection;
            }
        }

        /// <summary>
        /// database字符串
        /// </summary>
        public static string Database
        {
            get
            {
                var database = ConfigurationManager.AppSettings["database"];
                return database;
            }
        }

        /// <summary>
        /// database字符串
        /// </summary>
        public static string ReplicaSetName
        {
            get
            {
                var database = ConfigurationManager.AppSettings["ReplicaSetName"];
                return database;
            }
        }
    }

}
