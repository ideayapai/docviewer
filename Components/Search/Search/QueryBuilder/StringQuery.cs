using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace Search.QueryBuilder
{
    public class StringQuery: BaseQuery
    {
        private readonly string _field;
        private readonly string _query;
        private readonly Analyzer _analyzer = AnalyzerFactory.GetAnalyzer();
        public StringQuery(string field, string query):
            this(field, query, OccurType.Should)
        {
            
        }

        public StringQuery(string field, string query, OccurType occurType)
            : base(occurType)
        {
            _field = field;
            _query = query;
        }

        public override Query GetQuery()
        {
            if (string.IsNullOrWhiteSpace(_query))
            {
                return new EmptyQuery();
            }

            var parser = new QueryParser(Version.LUCENE_29, _field, _analyzer);
            return parser.Parse(QueryParser.Escape(_query));
        }
    }
}

