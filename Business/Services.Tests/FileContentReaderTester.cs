using System;
using Infrasturcture.Store;
using Infrasturcture.Store.Local;
using Infrasturcture.Store.Mongo;
using NUnit.Framework;
using Services.Ioc;
using Services.Search;

namespace Services.Tests
{
    [TestFixture]
    public class FileContentReaderTester
    {
        static readonly string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.docx";
       
        [Test]
        public void should_read_file_content_from_localfile()
        {
           IStorePolicy policy = ServiceActivator.Get<LocalPolicy>();

            FileContentReader reader = new FileContentReader(policy);
            var content = reader.Read(file);
            Console.WriteLine(content);
        }

        [Test]
        public void should_read_file_content_from_mongo_db()
        {
            IStorePolicy policy = ServiceActivator.Get<MongoPolicy>();
            string remoteFile = "sample.docx";
            policy.Add(file, remoteFile);
            FileContentReader reader = new FileContentReader(policy);
            var content = reader.Read(remoteFile);
            Console.WriteLine(content);
            policy.Delete(remoteFile);
        }
    }
}
