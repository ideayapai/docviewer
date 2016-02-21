using Lucene.Net.Search;

namespace Search.QueryBuilder
{
    public static class OccurTypeConversion
    {
        public static BooleanClause.Occur ToOccur(this OccurType occurType)
        {
            BooleanClause.Occur clause = BooleanClause.Occur.SHOULD;
            switch (occurType)
            {
                case OccurType.Must:
                    clause = BooleanClause.Occur.MUST;
                    break;
                case OccurType.MustNot:
                    clause = BooleanClause.Occur.MUST_NOT;
                    break;
                case OccurType.Should:
                    clause = BooleanClause.Occur.SHOULD;
                    break;
            }
            return clause;
        }
    }
}
