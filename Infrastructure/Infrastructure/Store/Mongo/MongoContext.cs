using System;
using System.Collections.Generic;
using System.Configuration;
using Infrasturcture.Store.Configuration;
using MongoDB.Driver;

namespace Infrasturcture.Store.Mongo
{
    public class MongoContext
    {
        private static MongoDatabase _database;

        MongoClientSettings GetSettings()
        {
             if (String.IsNullOrEmpty(MongoSettings.ConnectionString))
             {
                 throw new ArgumentNullException("Connection string not found.");
             }

             if (String.IsNullOrEmpty(MongoSettings.Database))
             {
                 throw new ArgumentNullException("Database string not found.");
             }

             var ips = MongoSettings.ConnectionString.Split(';');
             var servicesList = new List<MongoServerAddress>();
 
             foreach (var ip in ips)
             {
                 var host = ip.Split(':')[0];
                 var port = Convert.ToInt32(ip.Split(':')[1]);
 
                 servicesList.Add(new MongoServerAddress(host, port));
             }
 
             var setting = new MongoClientSettings();
             setting.ReplicaSetName = MongoSettings.ReplicaSetName;
          
             //集群中的服务器列表
             setting.Servers = servicesList;
             
             return setting;
         }

        public MongoDatabase DataBase
        {
            get
            {
                if (_database == null)
                {
                    var settings = GetSettings();
                    MongoClient client = new MongoClient(settings);
                    MongoServer server = client.GetServer();
                    //var dataBaseSetting = new MongoDatabaseSettings(server, MongoSettings.Database)
                    //{
                    //    SlaveOk = true
                    //};

                    _database = server.GetDatabase(MongoSettings.Database);
                }

                return _database;
            }
        }


    }
}
