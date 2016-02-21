using Lucene.Net.Search;

namespace Search.QueryBuilder
{
    public abstract class BaseQuery
    {
        private BaseQuery _next;
        protected OccurType _occurType;

        protected BaseQuery()
        {
            _occurType = OccurType.Should;
        }

        protected BaseQuery(OccurType occurType)
        {
            _occurType = occurType;
        }

        public BaseQuery Next(BaseQuery next)
        {
            _next = next;
            return _next;
        }

        public Query GetRule()
        {
            BooleanQuery query = new BooleanQuery();
            return GetRule(query);
        }

        private Query GetRule(BooleanQuery booleanQuery)
        {
            var query = GetQuery();
            if (!(query is EmptyQuery))
            {
                booleanQuery.Add(query, _occurType.ToOccur());
            }

            if (_next != null)
            {
                return _next.GetRule(booleanQuery);
            }
            return booleanQuery;
        }

        public abstract Query GetQuery();

       
    }
}
