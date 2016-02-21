using Lucene.Net.Search;

namespace Search.QueryBuilder
{
    class EmptyQuery: Query
    {
        public override string ToString(string field)
        {
            return string.Empty;
        }
    }
}
