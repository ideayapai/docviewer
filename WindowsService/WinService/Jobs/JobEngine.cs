using System;
using System.IO;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Xml;

namespace DocViewerWinService.Jobs
{
    public class JobEngine
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        private IScheduler _scheduledJobs;

        public void Start()
        {
            _logger.Debug("启动 JobEngine....");

            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "quartz_jobs.xml");
                var sf = new StdSchedulerFactory();
                var processor = new JobSchedulingDataProcessor(true, true);
                _scheduledJobs = sf.GetScheduler();
                processor.ProcessFileAndScheduleJobs(path, _scheduledJobs, false);
                _scheduledJobs.Start();

                _logger.Debug("启动 JobEngine 完成.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
           
        }

        public void Stop()
        {
            _logger.Debug("停止 JobEngine.....");

            _scheduledJobs.Shutdown(true);

            _logger.Debug("停止 JobEngine 完成.");
        }
    }
}
