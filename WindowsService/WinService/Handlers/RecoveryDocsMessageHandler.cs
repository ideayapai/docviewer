using System;
using Common.Logging;
using Messages;
using Search;
using Services.Ioc;
using Services.Messages;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 创建索引的处理函数
    /// </summary>
    public class RecoveryDocsMessageHandler : IMessageHandler<RecoveryDocsMessage>
    {
        readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(RecoveryDocsMessage message)
        {
            _logger.Info("Handle RecoveryDocsMessage");

            try
            {

                foreach (var content in message.Contents)
                {
                    _searchProvider.Add(content);
                }

                _logger.Info("Handle RecoveryDocsMessage Success");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }

        }
    }
}
