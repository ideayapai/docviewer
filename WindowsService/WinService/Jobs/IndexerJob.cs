using Common.Logging;
using Quartz;

namespace DocViewerWinService.Jobs
{
    /// <summary>
    /// 创建索引类
    /// </summary>
    public class IndexerJob : IJob
    {
        readonly IndexerJobExecutor _executor = new IndexerJobExecutor();
        
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        
        public void Execute(JobExecutionContext context)
        {
            if (JobStatus.TransformStatus == Status.Off)
            {
                JobStatus.TransformStatus = Status.On;

                _executor.Execute();
                
                JobStatus.TransformStatus = Status.Off;
            }
            else
            {
                _logger.Info("索引创建正在执行,系统将在下个运行时间运行!");
            }
        }
    }


}
