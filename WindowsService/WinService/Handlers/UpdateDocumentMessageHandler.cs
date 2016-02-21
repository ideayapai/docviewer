using System;
using Common.Logging;
using Messages;
using Search;
using Services.Ioc;
using Services.Messages;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 重命名文件
    /// </summary>
    public class UpdateDocumentMessageHandler: IMessageHandler<UpdateDocumentMessage>
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        private readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();

        public void Handle(UpdateDocumentMessage message)
        {
            _logger.Info("Handle RenameDocumentMessage");

            try
            {
                _logger.Debug("删除原有索引");
                _searchProvider.Delete(message.Content);

                _logger.Debug("添加原有索引");
                _searchProvider.Add(message.Content);

                _logger.Info("Handle RenameDocumentMessage Success");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }
    }
}
