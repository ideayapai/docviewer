using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using Search.Imp;
using Services.Contracts;

namespace Search.Tests.Search
{
    [TestFixture]
    public class LueceneSearchProviderWriterPerformanceTester
    {
        [Test]
        public void performance_test_with_writeindex()
        {
            LuceneSearchProvider _lueceneSearchProvider = new LuceneSearchProvider();

            List<SpaceObject> list = new List<SpaceObject>();
            for (int i = 0; i < 100000; i++)
            {
                var space = new SpaceObject
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
                list.Add(space);
            }
            
            Stopwatch stopWatcher = new Stopwatch();
            stopWatcher.Start();

            _lueceneSearchProvider.AddList(list, true);

            stopWatcher.Stop();
            Console.WriteLine("创建时间为<毫秒>:" + stopWatcher.ElapsedMilliseconds);
        }
    }
}
