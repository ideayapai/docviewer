using System;
using Common.Logging;
using Infrasturcture.Cache;
using Services.Ioc;

namespace DocViewerWinService.Jobs
{
    internal class CachedJobExecutor
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Execute()
        {
            _logger.Debug("开始执行缓存重启...");

            try
            {
                var cachedService = ServiceActivator.Get<ICachePolicy>();

                cachedService.FlushAll();

                _logger.Debug("缓存重启完毕!");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
                _logger.Error(ex.Message);
            }
        }
    }
}
