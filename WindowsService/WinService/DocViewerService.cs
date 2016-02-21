using System.ServiceProcess;
using Common.Logging;
using DocViewerWinService.Jobs;
using Messages;
using Services.Ioc;

namespace DocViewerWinService
{
    partial class DocViewerService : ServiceBase
    {
        private MessageListener _listener;
        private JobEngine _jobEngine;

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public DocViewerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _logger.Info("Starting DocViewerService");

            _listener = ServiceActivator.Get<MessageListener>();
            _listener.Start();

            _jobEngine = ServiceActivator.Get<JobEngine>();
            _jobEngine.Start();

            _logger.Info("DocViewerService is started.");
        }

        protected override void OnStop()
        {
            _logger.Info("Stoping DocViewerService");

            _listener.Stop();
            _jobEngine.Stop();

           _logger.Info("DocViewerService is stopped");
        }


    }
}
