using System;
using System.Configuration;
using Common.Logging;
using Services.Ioc;
using Services.Search;

namespace DocViewerWinService.Jobs
{
    internal class IndexerJobExecutor
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Execute()
        {
            _logger.Debug("开始执行批量创建索引...");

            try
            {
          
                var searchService = ServiceActivator.Get<SearchService>();

                searchService.Update(ConfigurationManager.AppSettings["Segment"]);

                _logger.Debug("创建索引完毕!");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
                _logger.Error(ex.Message);
            }
        }
    }
}
