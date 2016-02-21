using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Search.Imp;

namespace Search.QueryBuilder
{
    public class MultiQuery<T>: BaseQuery
    {
        private readonly Analyzer _analyzer = AnalyzerFactory.GetAnalyzer();
        private readonly string _queryString;

        public MultiQuery(string queryString, Analyzer analyzer, OccurType occrType)
            : base(occrType)
        {
            _queryString = queryString;
        }

        public override Query GetQuery()
        {
            if (string.IsNullOrWhiteSpace(_queryString))
            {
                return new EmptyQuery();
            }

            var parser = new MultiFieldQueryParser(QueryUtils.GetQueryStrings(typeof(T)), _analyzer);
            var queryString = QueryParser.Escape(_queryString);
            return parser.Parse(queryString);
        }
    }
}
