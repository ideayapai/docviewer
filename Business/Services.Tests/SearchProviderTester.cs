using System;
using System.Collections.Generic;
using System.Diagnostics;
using Documents.Enums;
using Infrasturcture.Store.Local;
using NUnit.Framework;
using Search;
using Search.QueryBuilder;
using Services.Contracts;
using Services.Ioc;
using Services.Search;

namespace Services.Tests
{
    [TestFixture]
    public class DocumentLueceneSearchProviderWriterTester
    {
        private static readonly IFileContentReader reader = new FileContentReader(ServiceActivator.Get<LocalPolicy>());
        private readonly DocumentLuceneBaseSearchProvider _provider = new DocumentLuceneBaseSearchProvider(reader);

        [SetUp]
        public void SetUp()
        {
            WriteData("attachment");
        }

        [Test]
        public void test_simple_write_and_query()
        {
            var results = _provider.Query<DocumentSearchResult>("智慧市政");
            Assert.IsTrue(results.Count>= 3);
        }

        [Test]
        public void test_query_write_with_parameters_start_and_takeSize()
        {
            var results = _provider.Query<DocumentSearchResult>("智慧市政", 0, 2);
            Assert.AreEqual(results.Count, 2);
        }

        [Test]
        public void test_query_write_with_parameters_start_and_takeSize_with_zero()
        {
            var results = _provider.Query<DocumentSearchResult>("智慧市政", 0, 0);
            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void test_query_write_with_parameters_start_and_takeSize_with_negative()
        {
            var results = _provider.Query<DocumentSearchResult>("智慧市政", -1, 0);
            Assert.AreEqual(results.Count, 0);

            results = _provider.Query<DocumentSearchResult>("智慧市政", -1, -1);
            Assert.AreEqual(results.Count, 0);

            results = _provider.Query<DocumentSearchResult>("智慧市政", 0, -1);
            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void test_query_write_with_parameters_start_and_takeSize_with_null()
        {
            var results = _provider.Query<DocumentSearchResult>("智慧市政", null, null);
            var results2 = _provider.Query<DocumentSearchResult>("智慧市政", 0, null);
            var results3 = _provider.Query<DocumentSearchResult>("智慧市政", null, 0);
            var final = _provider.Query<DocumentSearchResult>("智慧市政");
            Assert.AreEqual(final.Count, results.Count);
            Assert.AreEqual(final.Count, results2.Count);
            Assert.AreEqual(final.Count, results3.Count);
        }

        [Test]
        public void test_query_write_with_querybuilder()
        {
            var query = new StringQuery("Name", "智慧市政", OccurType.Should);
            var results = _provider.Query<DocumentSearchResult>(query);
            Assert.IsTrue(results.Count>= 3);
        }

        [Test]
        public void test_query_write_with_querybuilder_with_documentType()
        {
            var query = new StringQuery("Name", "智慧市政", OccurType.Should);
            query.Next(new StringQuery("DocumentType", DocumentType.Folder.ToString(), OccurType.Must));
            var results = _provider.Query<DocumentSearchResult>(query);
            Assert.AreEqual(results.Count, 1);
        }

        [Test]
        public void test_query_write_with_querybuilder_with_documentType_with_unknowName()
        {
            var query = new StringQuery("Name", "unknowNamemyff");
            var query2 = new StringQuery("Content", "unknowNamemyff");
            var orQuery = new OrQuery(query, query2, OccurType.Must);
            orQuery.Next(new StringQuery("DocumentType", DocumentType.Excel.ToString(), OccurType.Must));
            var results = _provider.Query<DocumentSearchResult>(orQuery);
            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void test_query_write_with_querybuilder_match_one_condition()
        {
            var query = new StringQuery("Name", "报价明细表");
            var query2 = new StringQuery("Content", "unknowNamemyff");
            var orQuery = new OrQuery(query, query2, OccurType.Must);
            orQuery.Next(new StringQuery("DocumentType", DocumentType.Excel.ToString(), OccurType.Must));
            var results = _provider.Query<DocumentSearchResult>(orQuery);
            Assert.AreEqual(results.Count, 1);
        }

        [Test]
        public void test_write_documents_with_query_and_fields()
        {
            var query = new StringQuery("Name", "报价明细表", OccurType.Should);
            query.Next(new StringQuery("DocumentType", DocumentType.Excel.ToString(), OccurType.Should));

            var results = _provider.Query<DocumentSearchResult>(query);
            Assert.AreEqual(results.Count, 1);
        }

        [Test]
        public void test_write_documents_with_query_condition_with_different_documentType()
        {
            var query = new StringQuery("Name", "报价明细表", OccurType.Should);
            var query2 = new StringQuery("Content", "报价明细表", OccurType.Should);
            var orQuery = new OrQuery(query, query2, OccurType.Must);
            orQuery.Next(new StringQuery("DocumentType", DocumentType.Word.ToString(), OccurType.Must));

            var results = _provider.Query<DocumentSearchResult>(orQuery);
            Assert.AreEqual(results.Count, 0);

        }

        [Test]
        public void test_write_documents_with_query_condition_with_documentType_null()
        {
            var query = new StringQuery("Name", "报价明细表", OccurType.Should);
            var query2 = new StringQuery("Content", "报价明细表", OccurType.Should);

            var orQuery = new OrQuery(query, query2, OccurType.Must);
            orQuery.Next(new StringQuery(null, null, OccurType.Must));

            var results = _provider.Query<DocumentSearchResult>(orQuery);
            Assert.AreEqual(results.Count, 1);
        }

        [Test]
        public void test_write_documents_with_query_condition_with_all_query_null()
        {
            var query = new StringQuery(null, null, OccurType.Should);
            var query2 = new StringQuery(null, null, OccurType.Should);

            var orQuery = new OrQuery(query, query2, OccurType.Must);
            orQuery.Next(new StringQuery(null, null, OccurType.Must));

            var results = _provider.Query<DocumentSearchResult>(orQuery);
            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void test_write_documents_with_query_condition_with_all_query_empty()
        {
            var query = new StringQuery(string.Empty, string.Empty, OccurType.Should);
            var query2 = new StringQuery(string.Empty, string.Empty, OccurType.Should);

            var orQuery = new OrQuery(query, query2, OccurType.Must);
            orQuery.Next(new StringQuery(string.Empty, string.Empty, OccurType.Must));

            var results = _provider.Query<DocumentSearchResult>(orQuery);
            Assert.AreEqual(results.Count, 0);
        }

        [Test]
        public void test_write_documents_with_query_condition_with_range_begin_end()
        {
            var query = new StringQuery("Name", "智慧市政", OccurType.Should);
            query.Next(new RangeQuery("UpdateTime", "20140721", "20141221", OccurType.Must));

            var results = _provider.Query<DocumentSearchResult>(query);
            Assert.AreEqual(results.Count, 2);
        }


        [Test]
        public void test_write_documents_with_query_condition_with_null_range_begin_end()
        {
            var query = new StringQuery("Name", "智慧市政", OccurType.Should);
            query.Next(new RangeQuery(null, null, null, OccurType.Should));

            var results = _provider.Query<DocumentSearchResult>(query);
            Assert.AreEqual(results.Count, 3);
        }

        [Test]
        public void test_write_documents_with_emptyString_condition_with_null_range_begin_end()
        {
            var query = new StringQuery("Name", "智慧市政", OccurType.Should);
            query.Next(new RangeQuery(string.Empty,"     ", string.Empty, OccurType.Should));

            var results = _provider.Query<DocumentSearchResult>(query);
            Assert.AreEqual(results.Count, 3);
        }

        [Test]
        public void test_write_documents_with_query_condition_with_range_begin_end_start_startIndex()
        {
            var query = new StringQuery("Name", "智慧市政", OccurType.Should);
            query.Next(new RangeQuery("UpdateTime", "20140721", "20141221", OccurType.Must));

            var results = _provider.Query<DocumentSearchResult>(query, 0, 1);
            Assert.AreEqual(results.Count, 1);

        }

        [Test]
        public void test_write_spaces_with_keyword_with_range_begin_end()
        {
            var query = new StringQuery("Name", "智慧市政", OccurType.Should);
            query.Next(new StringQuery("DocumentType", DocumentType.Folder.ToString(), OccurType.Must))
                 .Next(new RangeQuery("UpdateTime", "20140721", "20141221", OccurType.Must));

            var results = _provider.Query<DocumentSearchResult>(query);
            Assert.AreEqual(results.Count, 1);

        }

        private void WriteData(string folder)
        {
            _provider.IndexDirectory = folder;
            Stopwatch stopWatcher = new Stopwatch();

            var list = InitializeData();

            stopWatcher.Start();
            _provider.AddList(list, true);
            stopWatcher.Stop();
            Console.WriteLine("创建时间为<毫秒>:" + stopWatcher.ElapsedMilliseconds);

        }

        private static List<object> InitializeData()
        {
            List<object> list = new List<object>();

            list.Add(new DocumentObject
                         {
                             Id = Guid.NewGuid(),
                             DisplayPath = "http://localhost/view/1",
                     
                             CreateTime = DateTime.Parse("2014-08-21"),
                             FileSize = 1024,
                             UpdateTime = DateTime.Parse("2014-08-21"),
                             DocumentType = DocumentType.PPT,
                             StorePath = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台97format.ppt",
                             FileName = "智慧市政综合指挥平台97format.ppt"
                         });

            list.Add(new DocumentObject
                         {
                             Id = Guid.NewGuid(),
                             DisplayPath = "http://localhost/view/1",
                    
                             CreateTime = DateTime.Parse("2014-06-21"),
                             FileSize = 1024,
                             UpdateTime = DateTime.Parse("2014-06-21"),
                             DocumentType = DocumentType.PPT,
                             StorePath = Environment.CurrentDirectory + @"\TestFiles\Yapa   Tech.ppt",
                             FileName = "Yapa   Tech.ppt"
                         });

            list.Add(new DocumentObject
                         {
                             Id = Guid.NewGuid(),
                             DisplayPath = "http://localhost/view/1",
                  
                             CreateTime = DateTime.Now,
                             FileSize = 1024,
                             UpdateTime = DateTime.Now,
                             DocumentType = DocumentType.Excel,
                             StorePath = Environment.CurrentDirectory + @"\TestFiles\报价明细表97format.xls",
                             FileName = "圣诞报价明细表97format.xls"
                         });
            
            list.Add(new DocumentObject
                           {
                               Id = Guid.NewGuid(),
                               DisplayPath = "http://localhost/view/1",
                        
                               CreateTime = DateTime.Now,
                               FileSize = 1024,
                               UpdateTime = DateTime.Now,
                               DocumentType = DocumentType.Word,
                               StorePath = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台97format.doc",
                               FileName = "智慧市政综合指挥平台97format.doc"
                           });

            var space = new SpaceObject
                            {
                                Id = Guid.NewGuid(),
                                CreateUserId = "1",
                                SpaceName = "智慧市政",
                                UpdateTime = DateTime.Parse("2014-08-21"),
                                CreateTime = DateTime.Parse("2014-08-21")

                            };
            list.Add(space);
            return list;
        }
    }
}
