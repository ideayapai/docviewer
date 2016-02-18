using Common.Logging;
using Quartz;

namespace DocViewerWinService.Jobs
{
    class CachedJob: IJob
    {
        readonly CachedJobExecutor _executor = new CachedJobExecutor();
        
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        
        public void Execute(JobExecutionContext context)
        {
            _logger.Info("缓存重启创建GO!");

            _executor.Execute();
             
                
            
        }
    }
   
}
