using System;
using System.IO;
using System.Linq;
using System.Web;
using Infrasturcture.Store.Mongo;
using NUnit.Framework;

namespace Infrasturcture.Tests.Store
{
    [TestFixture]
    public class MongoPolicyTester
    {
        static readonly string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.docx";
        string localfile = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台local.docx";
         
        [Test]
        public void should_add_file_and_delete()
        {
            var id = Guid.NewGuid().ToString();

            var repository = new MongoPolicy();
            var result = repository.Add(file, id);

            Assert.IsTrue(repository.Exist(id));

            repository.Delete(result.fileName);
            Assert.IsFalse(repository.Exist(id));

        }


        [Test]
        public void should_add_stream_and_delete()
        {
            var id = Guid.NewGuid().ToString();

            var repository = new MongoPolicy();

            using (var stream = new FileStream(file, FileMode.Open))
            {
                string mimetype = MimeMapping.GetMimeMapping(file);
                var result = repository.AddStream(stream, mimetype, id);
                repository.Copy(localfile, result.fileName);
                repository.Delete(result.fileName);

                Assert.IsTrue(File.Exists(localfile));
                File.Delete(localfile);
            }
        }

        [Test]
        public void should_copy_to_localfile()
        {
            var id = Guid.NewGuid().ToString();

            var repository = new MongoPolicy();

            var result = repository.Add(file, id);
            repository.Copy(localfile, result.fileName);
            repository.Delete(result.fileName);

            Assert.IsTrue(File.Exists(localfile));
            File.Delete(localfile);
        }


        [Test]
        public void should_copy_to_localstream()
        {
            string id = Guid.NewGuid().ToString();

            var repository = new MongoPolicy();
            repository.Add(file, id);
            byte[] bytes = repository.GetBytes(id);
            repository.Delete(id);

            using (var fs = new FileStream(localfile, FileMode.Create))
            {
                fs.Read(bytes, 0, bytes.Count());
            }
            
            Assert.IsTrue(File.Exists(localfile));
            File.Delete(localfile);
        }
    }
}
