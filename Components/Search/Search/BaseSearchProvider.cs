using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Common.Logging;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Search.Exceptions;
using Search.Imp;
using Search.QueryBuilder;
using Directory = System.IO.Directory;

namespace Search
{
   
    public class BaseSearchProvider: ISearchProvider 
    {
        
        private readonly Analyzer _analyzer = AnalyzerFactory.GetAnalyzer();
        private IFileContentReader _reader;
        private string _indexDirectory = ConfigurationManager.AppSettings["segmentPath"];

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        public void setFileReader(IFileContentReader reader)
        {
            _reader = reader;
        }

        public string IndexDirectory
        {
            get
            {
                return _indexDirectory;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    _indexDirectory = Path.Combine(ConfigurationManager.AppSettings["segmentPath"]);
                }
                else
                {
                    if (!Directory.Exists(value))
                    {
                        try
                        {
                            Directory.CreateDirectory(value);
                        }
                        catch(Exception ex)
                        {
                            _logger.Error(ex.Message);
                        }
                    }
                    _indexDirectory = value;
                }
            }
        }

    
        public virtual List<T> Query<T>(string queryString) where T: class, new()
        {
            return Query<T>(new MultiQuery<T>(queryString, _analyzer, OccurType.Should), null, null);
        }

        public virtual List<T> Query<T>(string queryString, int? start, int? takeSize) where T : class, new()
        {
            return Query<T>(new MultiQuery<T>(queryString, _analyzer, OccurType.Should), start, takeSize);
        }

        public virtual List<T> Query<T>(BaseQuery query) where T : class, new()
        {
            return Query<T>(query, null, null);
        }

        public List<T> Query<T>(BaseQuery query, int? start, int? takeSize) where T : class, new()
        {
            _logger.Debug("Query");

            List<T> results = new List<T>();
            if (!IndexReader.IndexExists(IndexDirectory))
            {
                _logger.Error(_indexDirectory + "Query 文件夹不存在");
                return results;
            }

            IndexSearcher searcher = new IndexSearcher(IndexDirectory);
            var hits = GetSearchResult(searcher, query.GetRule(), start, takeSize);
            List<T> hitsToContract = HitsToContract<T>(hits);
            if (hitsToContract != null)
            {
                results = hitsToContract.ToList();
            }

            searcher.Close();
            return results;
        }
       
        private SearchResult GetSearchResult(IndexSearcher searcher,
                                             Query booleanQuery,
                                             int? startPos,
                                             int? takeSize)
        {
            _logger.Debug("GetSearchResult");

            var docs = new List<Document>();
            
            if (startPos == null || takeSize == null )
            {
                Hits hits = searcher.Search(booleanQuery);
                for (int i = 0; i < hits.Length(); i++)
                {
                    Document doc = hits.Doc(i);
                    docs.Add(doc);
                }
                return new SearchResult(docs.Count, docs);
            }

            if (takeSize.Value <= 0)
            {
                return new SearchResult(docs.Count, docs);
            }
        
            TopDocs topDocs = searcher.Search(booleanQuery, startPos.Value + takeSize.Value);
            ScoreDoc[] scoreDocs = topDocs.scoreDocs;
            int endIndex = Math.Min(startPos.Value + takeSize.Value, scoreDocs.Length);
            for (int i = startPos.Value; i < endIndex; i++)
            {
                Document doc = searcher.Doc(scoreDocs[i].doc);
                docs.Add(doc);
            }

            return new SearchResult(docs.Count, docs);
        }

        /// <summary>
        /// 追加索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        public virtual void Add<T>(T source) where T : class, new()
        {    
            Add(source, false);
        }

        public void Add<T>(T source, bool rewrite) where T : class, new()
        {
            _logger.Debug("Add");

            List<object> errorItems = new List<object>();
            var writer = GetIndexWriter(false);

            try
            {
                var document = new Document();
                var fields = ObjToField(source);
                foreach (var field in fields)
                {
                    document.Add(field);
                }

                writer.AddDocument(document);

                _logger.Debug("Add Finished");
            }
            catch (Exception e)
            {
                _logger.Error(e.StackTrace);
                errorItems.Add(source);
            }
            writer.Optimize();
            writer.Close();

            if (errorItems.Count != 0)
            {
                throw new WriteIndexException(errorItems);
            }
        }

        public void AddList<T>(IList<T> source) where T : class, new()
        {
            AddList(source, false);
        }

        /// <summary>
        /// 生成索引接口
        /// </summary>
        /// <param name="result"></param>
        public virtual void AddList<T>(IList<T> sources, bool rewrite) where T : class, new()
        {
            _logger.Debug("AddList");

            var writer = GetIndexWriter(rewrite);
            var errorItems = new List<object>();
            foreach (var source in sources)
            {
                try
                {
                    var document = new Document();
                    var fields = ObjToField(source);
                    foreach (var field in fields)
                    {
                        document.Add(field);
                    }
                    writer.AddDocument(document);

                    _logger.Debug("AddList Finished");
                }
                catch (Exception e)
                {
                    _logger.Error(e.StackTrace);
                    errorItems.Add(source);
                }
            }

            writer.Optimize();
            writer.Close();

            if (errorItems.Count != 0)
            {
                throw new WriteIndexException(errorItems);
            }
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        public bool Delete<T>(T source) where T : class
        {
            _logger.Debug("Delete");

            bool result = false;
            var writer = GetIndexWriter(false);


            try
            {
                var term = ObjToTerm(source);
                writer.DeleteDocuments(term);
                writer.Optimize();
                writer.Commit();
                result = true;

                _logger.Debug("Delete Finished");
            }
            catch (Exception e)
            {
                _logger.Error(e.StackTrace);
            }

            writer.Close();

            return result;
        }

        /// <summary>
        /// 删除所有索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool DeleteList<T>(IList<T> source) where T : class
        {
            _logger.Debug("DeleteList");

            bool result = false;
            var terms = new List<Term>();
            foreach (var item in source)
            {
                var term = ObjToTerm(item);
                terms.Add(term);
            }

            try
            {
                var writer = GetIndexWriter(false);
                writer.DeleteDocuments(terms.ToArray());
                writer.Optimize();
                writer.Commit();
                result = true;
                writer.Close();

                _logger.Debug("DeleteList Finished.");
            }
            catch (Exception e)
            {
                _logger.Error(e.StackTrace);
            }
            
            return result;
        }


       

        private IndexWriter GetIndexWriter(bool rewrite)
        {
            IndexWriter writer;

            if (rewrite)
            {
                writer = new IndexWriter(IndexDirectory, _analyzer, true);
            }
            else
            {
                if (IndexReader.IndexExists(IndexDirectory))
                {
                    if (IndexWriter.IsLocked(IndexDirectory))
                    {
                        IndexWriter.Unlock(new SimpleFSDirectory(new DirectoryInfo(IndexDirectory)));
                    }
                    writer = new IndexWriter(IndexDirectory, _analyzer, false);
                }
                else
                {
                    writer = new IndexWriter(IndexDirectory, _analyzer, true);
                }
            }
            
            return writer;
        }

        protected virtual List<T> HitsToContract<T>(SearchResult hits) where T: class, new()
        {
            var result = new List<T>();
            var queryStrings = QueryUtils.GetQueryStrings(typeof(T));
            for (int i = 0; i < hits.TotalHits; i++)
            {
                Assembly assembly = Assembly.GetAssembly(typeof(T));
                T obj = assembly.CreateInstance(String.Join(".", new[] { typeof(T).Namespace, typeof(T).Name })) as T;
                foreach (PropertyInfo property in obj.GetType().GetProperties())
                {
                    if (queryStrings.Contains(property.Name) && property.CanWrite)
                    {
                        obj.SetValue(property, hits.Docs[i].Get(property.Name));
                    }
                }
                result.Add(obj);
            }

            return result;
        }

        /// <summary>
        /// 转换obj为Field接口
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public List<Field> ObjToField<T>(T obj)
        {
            var fields = new List<Field>();

            Type type = obj.GetType();
            foreach (var property in type.GetProperties())
            {
                foreach (var customAttribute in property.GetCustomAttributes(true))
                {
                    var attribute = customAttribute as SearchIndexAttribute;
                    if (attribute != null)
                    {
                        string fieldName = attribute.Name ?? property.Name;

                        string value = null;
                        if (!String.IsNullOrEmpty(attribute.FileName))
                        {
                            PropertyInfo propertyInfo = type.GetProperty(attribute.FileName);
                            var fileName = propertyInfo.GetValue(obj, null).ToString();
                            if (_reader != null)
                            {
                                value = _reader.Read(fileName);
                            }
                        }
                        else if (attribute.Date && property.PropertyType == typeof(DateTime))
                        {
                            value = DateTime.Parse(property.GetValue(obj, null).ToString()).ToString("yyyyMMdd");
                        }
                        else
                        {
                            value = CheckEmpty(property.GetValue(obj, null));
                        }

                        Field.Index index = Field.Index.NO;

                        if (attribute.Analyzed || attribute.Tokenized)
                        {
                            index = Field.Index.ANALYZED;
                        }

                        var field = new Field(fieldName, value, Field.Store.YES, index);
                        fields.Add(field);

                        break;
                    }
                }
            }
            return fields;
        }

        public static Term ObjToTerm<T>(T obj)
        {
            Type type = obj.GetType();
            foreach (var property in type.GetProperties())
            {
                foreach (var customAttribute in property.GetCustomAttributes(true))
                {
                    var attribute = customAttribute as SearchIndexAttribute;
                    if (attribute != null && attribute.IsDelete)
                    {
                        string value = CheckEmpty(property.GetValue(obj, null));
                        var term = new Term(property.Name, value);
                        return term;
                    }
                }
            }

            return null;
        }

        public static string CheckEmpty(object obj)
        {
            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
            {
                return String.Empty;
            }
            return obj.ToString();
        }
    }
}
