using System.Collections.Generic;
using Search;
using Search.Imp;

namespace Services.Search
{
    public class DocumentLuceneBaseSearchProvider : BaseSearchProvider
    {
        public DocumentLuceneBaseSearchProvider(IFileContentReader fileContentReader)
        {
            setFileReader(fileContentReader);
        }

        /// <summary>
        /// SearchResultBean转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hits"></param>
        /// <returns></returns>
        protected override List<T> HitsToContract<T>(SearchResult hits)
        {
            var result = new List<DocumentSearchResult>();

            for (int i = 0; i < hits.TotalHits; i++)
            {
                var document = hits.Docs[i];

                var searchResultObject = new DocumentSearchResult
                                             {
                                                 Id = CheckEmpty(document.Get("Id")).ConvertToGuid(),
                                                 CreateUserId = CheckEmpty(document.Get("CreateUserId")),
                                                 Name = CheckEmpty(document.Get("Name")),
                                                 UpdateTime = CheckEmpty(document.Get("UpdateTime")),
                                                 CreateTime = CheckEmpty(document.Get("CreateTime")),
                                                 CreateUserName = CheckEmpty(document.Get("CreateUserName")),
                                                 DocumentType = CheckEmpty(document.Get("DocumentType")),
                                                 Size = CheckEmpty(document.Get("Size")).ConvertStringToInt(),
                                                 Content = CheckEmpty(document.Get("Content")),
                                                 Visible = CheckEmpty(document.Get("Visible")).ConvertStringToInt(),
                                                 DepId = CheckEmpty(document.Get("DepId")),
                                                 UserId = CheckEmpty(document.Get("CreateUserId")),
                                                 DownloadPath = CheckEmpty(document.Get("DownloadPath")),
                                                 ThumbUrl = CheckEmpty(document.Get("ThumbUrl")),
                                                 DisplayPath = CheckEmpty(document.Get("DisplayPath"))
                                             };
                result.Add(searchResultObject);
            }
            return result as List<T>;
        }
    }
}
