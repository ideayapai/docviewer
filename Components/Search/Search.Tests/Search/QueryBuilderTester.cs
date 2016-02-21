using NUnit.Framework;
using Search.QueryBuilder;

namespace Search.Tests.Search
{
    [TestFixture]
    public class QueryBuilderTester
    {
        [Test]
        public void should_construct_builder()
        {
          
            var query = new StringQuery("Name", "文档", OccurType.Should)
                .Next(new RangeQuery("UpdateTime", "2015-10-11", "2015-2-1", OccurType.Must))
                .Next(new StringQuery("Content", "文档", OccurType.Should));

            var rule = query.GetRule();

        }
    }
}
