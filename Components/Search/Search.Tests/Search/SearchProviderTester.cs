using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Search.Imp;
using Services.Contracts;

namespace Search.Tests.Search
{
    [TestFixture]
    public class SearchProviderTester
    {
       
        [Test]
        public void should_get_queryStrings()
        {
           
            var queryStrings = QueryUtils.GetQueryStrings(typeof(SpaceObject));
            Assert.IsTrue(queryStrings.Contains("Name"));
        }

        [Test]
        public void should_query_with_null_query()
        {
            BaseSearchProvider lueceneBaseSearchProvider = new BaseSearchProvider();

            string query = null;
            var results = lueceneBaseSearchProvider.Query<SpaceObject>(query);
            Assert.AreEqual(results.Count, 0);

        }

        [Test]
        public void should_query_with_empty_string()
        {
            BaseSearchProvider lueceneBaseSearchProvider = new BaseSearchProvider();

            var results = lueceneBaseSearchProvider.Query<SpaceObject>(string.Empty);
            Assert.AreEqual(results.Count, 0);

            results = lueceneBaseSearchProvider.Query<SpaceObject>("      ");
            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void should_query_with_special_string()
        {
            BaseSearchProvider lueceneBaseSearchProvider = new BaseSearchProvider();

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
            lueceneBaseSearchProvider.Add(spaceOne);
         
            var results = lueceneBaseSearchProvider.Query<SpaceObject>("*****");
            Assert.AreEqual(results.Count, 0);

            results = lueceneBaseSearchProvider.Query<SpaceObject>("#@RF**__++||");
            Assert.AreEqual(results.Count, 0);

            results = lueceneBaseSearchProvider.Query<SpaceObject>(" || __    ");
            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void test_with_write_index_singleton()
        {
            BaseSearchProvider lueceneBaseSearchProvider = new BaseSearchProvider();

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

            lueceneBaseSearchProvider.IndexDirectory = "tempspace";
            lueceneBaseSearchProvider.Add(spaceOne);
            lueceneBaseSearchProvider.Add(spaceTwo);
          
            var results = lueceneBaseSearchProvider.Query<SpaceObject>("空间");
            Assert.IsTrue(results.Count>= 2);

            Console.WriteLine(results.Count);
        }


        [Test]
        public void test_with_write_and_delete_document()
        {
            var lueceneSearchProvider = new BaseSearchProvider();

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
