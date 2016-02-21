using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrasturcture.Store;
using NUnit.Framework;

namespace Infrasturcture.Tests.Store
{
    [TestFixture]
    public class MongoDBFileRepositoryTester
    {
        private const string connectionString = @"mongodb://192.168.1.29:27017";
        private const string database = "docviewer";

        [Test]
        public void should_add_file_and_delete()
        {
           
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.docx";
            var repository = new MongoDBFileRepository(connectionString, database);
            repository.Init();
            repository.Add(file);
            //repository.Delete(Path.GetFileName(file));
            
        }

        [Test]
        public void should_add_file_and_delete_by_id()
        {
            
            string id = Guid.NewGuid().ToString();
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.docx";
            var repository = new MongoDBFileRepository(connectionString, database);
            repository.Init();
            repository.Add(id, file);
            repository.DeleteById(id);
            
        }

        [Test]
        public void should_add_file_and_download_delete_by_id()
        {

            string id = Guid.NewGuid().ToString();
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.docx";
            string localfile = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台local.docx";
            var repository = new MongoDBFileRepository(connectionString, database);
            repository.Init();
            repository.Add(id, file);
            repository.GetById(localfile, id);
            repository.DeleteById(id);

            Assert.IsTrue(File.Exists(localfile));
            File.Delete(localfile);
        }


        [Test]
        public void should_add_file_and_download_delete_by_name()
        {

            string id = Guid.NewGuid().ToString();
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.docx";
            string localfile = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台local.docx";
            var repository = new MongoDBFileRepository(connectionString, database);
            repository.Init();
            repository.Add(file);
            repository.Get(localfile, Path.GetFileName(file));
            repository.Delete(Path.GetFileName(file));

            Assert.IsTrue(File.Exists(localfile));
            File.Delete(localfile);
        }
    }
}
