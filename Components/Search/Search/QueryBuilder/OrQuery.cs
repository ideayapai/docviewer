using Lucene.Net.Search;

namespace Search.QueryBuilder
{
    /// <summary>
    /// 满足或关系
    /// </summary>
    public class OrQuery : BaseQuery
    {
        private readonly BaseQuery _fQuery;
        private readonly BaseQuery _f2Query;

        public OrQuery(BaseQuery fQuery, BaseQuery f2Query, OccurType occurType)
            :base(occurType)
        {
            _fQuery = fQuery;
            _f2Query = f2Query;
        }

        public override Query GetQuery()
        {
            if (_fQuery == null || _f2Query == null)
            {
                return new EmptyQuery();
            }

            if ((_fQuery.GetQuery() is EmptyQuery)||  _f2Query.GetQuery() is EmptyQuery)
            {
                return new EmptyQuery();
            }

            BooleanQuery booleanQuery = new BooleanQuery();
            booleanQuery.Add(_fQuery.GetQuery(), BooleanClause.Occur.SHOULD);
            booleanQuery.Add(_f2Query.GetQuery(), BooleanClause.Occur.SHOULD);
            return booleanQuery;
        }
    }
}
