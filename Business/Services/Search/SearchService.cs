using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Logging;
using Search;
using Search.Imp;
using Services.Enums;
using Services.Search.UnitOfWork;

namespace Services.Search
{
    /// <summary>
    /// 索引服务
    /// </summary>
    public class SearchService
    {
        private readonly ISearchProvider _searchProvider;
        private readonly UnitWorkContainer _unitWorkContainer;
     
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public SearchService(ISearchProvider searchProvider,
                             UnitWorkContainer unitWorkContainer)
        {
            _searchProvider = searchProvider;
            _unitWorkContainer = unitWorkContainer;
        }

        public string IndexDirectory
        {
            get
            {
                return _searchProvider.IndexDirectory;
            }

            set
            {
                _searchProvider.IndexDirectory = value;
            }
        }

        public List<DocumentSearchResult> Query(string queryString)
        {
            return _searchProvider.Query<DocumentSearchResult>(queryString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryString">关键字</param>
        /// <param name="documentType">文档类型</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        public List<DocumentSearchResult> Query(string queryString, 
                                                string documentType, 
                                                string userId,
                                                string depId,
                                                int? start = null,
                                                int? takeSize = null)
        {
            _logger.InfoFormat("SearchService 查询条件:{0},{1}", queryString, documentType);

            if (string.IsNullOrWhiteSpace(queryString))
            {
                return new List<DocumentSearchResult>();
            }

            var results = _searchProvider.Query<DocumentSearchResult>(queryString.Trim(), start, takeSize);

            if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(depId))
            {
                results = results.FindAll(f => f.CreateUserId == userId || (f.DepId == depId && f.Visible == (int)Visible.Dep) ||
                                       f.Visible == (int)Visible.Public);
            }
            

            if (!string.IsNullOrWhiteSpace(documentType))
            {
                results = results.FindAll(f => f.DocumentType == documentType);
            }

            foreach (var result in results)
            {
                result.Name = SplitContent.HightLightTitle(queryString, result.Name);
                result.Content = SplitContent.HightLight(queryString, result.Content);
            }
            return results;

        }

        /// <summary>
        /// 更新索引文件，添加未添加的索引，已经删除的空间和文档，从索引中删除
        /// </summary>
        /// <param name="directory"></param>
        public void Update(string directory)
        {
            _logger.Info("写入索引到目录:" + directory);

            if (Directory.Exists(_searchProvider.IndexDirectory))
            {
                _searchProvider.IndexDirectory = directory;
            }

            foreach (var work in _unitWorkContainer)
            {
                try
                {
                    work.DoWork();
                }
                catch (Exception e)
                {
                    _logger.Error("写索引出错" + work.GetType(), e);
                }

            }
        }
    }
}
