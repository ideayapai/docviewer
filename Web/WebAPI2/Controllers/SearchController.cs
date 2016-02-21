using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using Infrasturcture.DomainObjects;
using Services.Models;
using Services.Search;

namespace WebAPI2.Controllers
{
    /// <summary>
    /// 搜索相关API
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SearchController : ApiController
    {
        private readonly SearchService _indexService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="indexService"></param>
        public SearchController(SearchService indexService)
        {
            _indexService = indexService;
        }

        /// <summary>
        /// 根据关键字搜索
        /// </summary>
        /// <param name="queryString"></param>
        /// <param name="documentType"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <param name="start"></param>
        /// <param name="takeSize"></param>
        /// <returns></returns>
        public List<SearchContract> Index(string queryString, 
                                          string documentType, 
                                          string begin, 
                                          string end, 
                                          int? start, 
                                          int? takeSize)
        {
          
            var list = _indexService.Query(queryString, documentType, null, null, start, takeSize);

            return list.ConvertAll(f=>f.ToObject<SearchContract>());
        }

    }
}
