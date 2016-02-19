using System;
using System.Diagnostics;
using System.Web.Mvc;
using Common.Logging;
using Infrasturcture.Web;
using Services.Context;
using Services.Search;
using WebSite2.Models;

namespace WebSite2.Controllers
{

    public class SearchController : BaseController
    {
        private readonly SearchService _searchService;
        private readonly ContextService _contextService;
        private readonly IHttpRequestProvider _requestProvider;

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public SearchController(SearchService searchService, IHttpRequestProvider requestProvider, ContextService contextService)
        {
            _searchService = searchService;
            _requestProvider = requestProvider;
            _contextService = contextService;

        }

        [HttpPost]
        public ActionResult Index()
        {
            _logger.InfoFormat("搜索,查询条件为:QueryString:{0},DocumentType{1}",
                _requestProvider["QueryString"], _requestProvider["documentType"]);

            _logger.InfoFormat("索引所在目录为:{0}", IndexDirectory);

            var model = new SearchViewModel();

            Stopwatch watch = new Stopwatch();
            watch.Start();

            try
            {

                _searchService.IndexDirectory = IndexDirectory;
                var queryModel = new SearchQueryViewModel(_requestProvider["QueryString"],
                                                          _requestProvider["documentType"],
                                                          _requestProvider["time"],
                                                          _requestProvider["count"]);

                var list = _searchService.Query(queryModel.QueryString,
                                                queryModel.DocumentType,
                                                _contextService.UserId,
                                                _contextService.DepId);

                model.SearchQueryViewModel = queryModel;
                model.SearchResultViewModel = list.ToList();

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }

            watch.Stop();
            model.ElapseTime = watch.ElapsedMilliseconds;

            _logger.InfoFormat("搜索耗时:{0}", watch.ElapsedMilliseconds);

            return View(model);
        }

    }
}
