using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;

namespace Search.QueryBuilder
{
    public static class AnalyzerFactory
    {
        private static readonly Analyzer _analyzer = new StandardAnalyzer();

        public static Analyzer GetAnalyzer()
        {
            return _analyzer;
        }
    }
}
