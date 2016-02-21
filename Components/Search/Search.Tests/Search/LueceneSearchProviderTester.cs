using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Search.Imp;
using Services.Contracts;

namespace Search.Tests.Search
{
    [TestFixture]
    public class LueceneSearchProviderTester
    {
       

        [Test]
        public void should_get_queryStrings()
        {
           
            var queryStrings = LuceneHelper.GetQueryStrings(typeof(SpaceObject));
            Assert.IsTrue(queryStrings.Contains("Name"));
        }

        [Test]
        public void should_query_with_null_query()
        {
            LuceneSearchProvider lueceneSearchProvider = new LuceneSearchProvider();

            string query = null;
            var results = lueceneSearchProvider.Query<SpaceObject>(query);
            Assert.AreEqual(results.Count, 0);

        }

        [Test]
        public void should_query_with_empty_string()
        {
            LuceneSearchProvider lueceneSearchProvider = new LuceneSearchProvider();

            var results = lueceneSearchProvider.Query<SpaceObject>(string.Empty);
            Assert.AreEqual(results.Count, 0);

            results = lueceneSearchProvider.Query<SpaceObject>("      ");
            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void should_query_with_special_string()
        {
            LuceneSearchProvider lueceneSearchProvider = new LuceneSearchProvider();

            var spaceOne = new SpaceObject
            {
                Id = Guid.NewGuid(),
                SpaceName = "技术空间",
                FileCount = 1,
                ParentId = Guid.NewGuid().ToString(),
                SpaceSize = 1024,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                CreateUserId = "5",
                CreateUserName = "kane",

            };
            lueceneSearchProvider.Add(spaceOne);
         
            var results = lueceneSearchProvider.Query<SpaceObject>("*****");
            Assert.AreEqual(results.Count, 0);

            results = lueceneSearchProvider.Query<SpaceObject>("#@RF**__++||");
            Assert.AreEqual(results.Count, 0);

            results = lueceneSearchProvider.Query<SpaceObject>(" || __    ");
            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void test_with_write_index_singleton()
        {
            LuceneSearchProvider lueceneSearchProvider = new LuceneSearchProvider();

            var spaceOne = new SpaceObject
            {
                Id = Guid.NewGuid(),
                SpaceName = "技术空间",
                FileCount = 1,
                ParentId = Guid.NewGuid().ToString(),
                SpaceSize = 1024,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                CreateUserId = "5",
                CreateUserName = "kane",

            };

            var spaceTwo = new SpaceObject
            {
                Id = Guid.NewGuid(),
                SpaceName = "生存空间",
                FileCount = 1,
                ParentId = Guid.NewGuid().ToString(),
                SpaceSize = 1024,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                CreateUserId = "5",
                CreateUserName = "kane",

            };

            Stopwatch stopWatcher = new Stopwatch();
            stopWatcher.Start();

            lueceneSearchProvider.IndexDirectory = "tempspace";
            lueceneSearchProvider.Add(spaceOne);
            lueceneSearchProvider.Add(spaceTwo);
          
            var results = lueceneSearchProvider.Query<SpaceObject>("空间");
            Assert.IsTrue(results.Count>= 2);

            Console.WriteLine(results.Count);
        }


        [Test]
        public void test_with_write_and_delete_document()
        {
            var lueceneSearchProvider = new LuceneSearchProvider();

            var spaceOne = new SpaceObject
            {
                Id = Guid.NewGuid(),
                SpaceName = "技术空间",
                FileCount = 1,
                ParentId = Guid.NewGuid().ToString(),
                SpaceSize = 1024,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                CreateUserId = "5",
                CreateUserName = "kane",
            };
           

            Stopwatch stopWatcher = new Stopwatch();
            stopWatcher.Start();

            lueceneSearchProvider.IndexDirectory = "write_delete_space";
            lueceneSearchProvider.Add(spaceOne);

            var results = lueceneSearchProvider.Query<SpaceObject>("空间");
            Assert.IsTrue(results.Count>= 1);

            lueceneSearchProvider.Delete(spaceOne);
            results = lueceneSearchProvider.Query<SpaceObject>("空间");
            Assert.AreEqual(results.Count, 0);

            Console.WriteLine(results.Count);
        }
    }
}
