using System;
using Common.Logging;
using Messages;
using Search;
using Services.Ioc;
using Services.Messages;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 恢复文档消息处理函数
    /// </summary>
    public class RecoveryDocumentMessageHandler: IMessageHandler<RecoveryDocumentMessage>
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        private readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();

        public void Handle(RecoveryDocumentMessage message)
        {
            _logger.Info("Handle RecoveryDocumentMessage");

            try
            {
                _searchProvider.Add(message.Content);

                _logger.Info("Handle RecoveryDocumentMessage Success");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }
    }
}
