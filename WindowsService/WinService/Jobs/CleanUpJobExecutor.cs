using System;
using System.Configuration;
using Common.Logging;
using Services.Search;

namespace DocViewerWinService.Jobs
{
    internal class CleanUpJobExecutor
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Execute()
        {
            _logger.Debug("开始执行批量删除数据中没有的文件...");

            try
            {
          
                //var searchService = ServiceActivator.Get<SearchService>();

                //searchService.Update(ConfigurationManager.AppSettings["Segment"]);

                _logger.Debug("批量删除完毕!");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
                _logger.Error(ex.Message);
            }
        }
    }
}
