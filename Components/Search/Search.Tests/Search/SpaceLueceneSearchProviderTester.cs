using System;
using System.Collections.Generic;
using System.Diagnostics;
using Documents.Enums;
using NUnit.Framework;
using Search.Imp;
using Search.QueryBuilder;
using Services.Contracts;
using Services.Ioc;
using Services.Search;

namespace Search.Tests.Search
{
    [TestFixture]
    public class SpaceLueceneSearchProviderWriterTester
    {
        private readonly DocumentLuceneSearchProvider _provider = ServiceActivator.Get<DocumentLuceneSearchProvider>();

        [Test]
        public void test_query_write_with_parameters_start_and_takeSize_with_zero()
        {
            WriteSpace("myspace");

            var results = _provider.Query<SearchResult>("空间", 0, 0);            
            Assert.AreEqual(results.Count, 0);
        }


        [Test]
        public void test_write_documents_with_query_and_fields()
        {
            WriteSpace("spaces");

            var query = new StringQuery("Name", "空间", OccurType.Should);
            query.Next(new StringQuery("DocumentType", DocumentType.Folder.ToString(), OccurType.Must));

            var results = _provider.Query<DocumentSearchResult>(query);
            Assert.AreEqual(results.Count, 2);
        }

        [Test]
        public void test_write_documents_with_query_condition_with_incorrect_name()
        {
            WriteSpace("spaces");

            var query = new StringQuery("Name", "unknow", OccurType.Should);
            var query2 = new StringQuery("Content", "unkonw", OccurType.Should);
            var orQuery = new OrQuery(query, query2, OccurType.Must);
            orQuery.Next(new StringQuery("DocumentType", DocumentType.Folder.ToString(), OccurType.Must));
            var results = _provider.Query<DocumentSearchResult>(orQuery);

            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void test_write_documents_with_query_condition_with_correct_name()
        {
            WriteSpace("spaces");

            var query = new StringQuery("Name", "空间", OccurType.Should);
            var query2 = new StringQuery("Content", "空间", OccurType.Should);
            var orQuery = new OrQuery(query, query2, OccurType.Must);
            orQuery.Next(new StringQuery("DocumentType", DocumentType.Folder.ToString(), OccurType.Must));
            var results = _provider.Query<DocumentSearchResult>(orQuery);

            Assert.AreEqual(results.Count, 2);
        }

        [Test]

        public void test_write_documents_with_query_condition_with_different_documentType()
        {
            WriteSpace("spaces");

            var query = new StringQuery("Name", "空间", OccurType.Should);
            query.Next(new StringQuery("DocumentType", DocumentType.Word.ToString(), OccurType.Must));
            var results = _provider.Query<SearchResult>(query);
            
            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void test_write_documents_with_query_condition_with_range_begin_end()
        {
            WriteSpace("spaces");

            var query = new StringQuery("Name", "空间", OccurType.Should);
            query.Next(new RangeQuery("UpdateTime", "20140521", "20140622", OccurType.Must));

            var results = _provider.Query<DocumentSearchResult>(query, 0, 1);
            Assert.AreEqual(results.Count, 1);
        }


        [Test]
        public void test_write_spaces_with_keyword_with_should_occurType()
        {
            WriteSpace("spaces");

            var query = new StringQuery("Name", "空间");
            query.Next(new StringQuery("Content", "空间", OccurType.Should));

            var results = _provider.Query<DocumentSearchResult>(query);
            Assert.AreEqual(results.Count, 2);
        }

        [Test]
        public void test_write_spaces_with_keyword_with_must_occurType()
        {
            WriteSpace("spaces");

            var query = new StringQuery("Name", "空间");
            query.Next(new StringQuery("Content", "空间", OccurType.Must));
            var results = _provider.Query<SearchResult>(query);
            Assert.AreEqual(results.Count, 0);

        }

        private void WriteSpace(string folder)
        {
            _provider.IndexDirectory = folder;

            Stopwatch stopWatcher = new Stopwatch();

            var list = InitialzeData();

            stopWatcher.Start();
            _provider.AddList(list, true);
            stopWatcher.Stop();
            Console.WriteLine("创建时间为<毫秒>:" + stopWatcher.ElapsedMilliseconds);

        }

        private static List<object> InitialzeData()
        {
            List<object> list = new List<object>();

            var space = new SpaceObject
            {
                Id = Guid.NewGuid(),
                CreateUserId = "1",
                SpaceName = "昆明空间",
                UpdateTime = DateTime.Parse("2014-06-21"),
                CreateTime = DateTime.Parse("2014-06-21")
            };
            var space2 = new SpaceObject
            {
                Id = Guid.NewGuid(),
                CreateUserId = "1",
                SpaceName = "技术空间",
                UpdateTime = DateTime.Parse("2014-06-21"),
                CreateTime = DateTime.Parse("2014-06-21")
            };
            list.Add(space);
            list.Add(space2);
            return list;
        }
    }
}
