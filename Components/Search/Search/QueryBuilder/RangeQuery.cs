using Lucene.Net.Search;

namespace Search.QueryBuilder
{
    public class RangeQuery: BaseQuery
    {
        private readonly string _range;
        private readonly string _start;
        private readonly string _end;

        public RangeQuery(string range, string start, string end, OccurType occurType):base(occurType)
        {
            _range = range;
            _start = start;
            _end = end;
        }

        public override Query GetQuery()
        {
            if (string.IsNullOrWhiteSpace(_range) || string.IsNullOrWhiteSpace(_start)
                || string.IsNullOrWhiteSpace(_end))
            {
                return new EmptyQuery();
            }

            return new TermRangeQuery(_range, _start, _end, true, true);
        }
    }
}
