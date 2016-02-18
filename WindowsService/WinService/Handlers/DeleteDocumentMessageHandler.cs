using System;
using Common.Logging;
using Messages;
using Search;
using Services.Ioc;
using Services.Messages;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 删除文档的处理函数
    /// </summary>
    public class DeleteDocumentMessageHandler: IMessageHandler<DeleteDocumentMessage>
    {
        readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();
        
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(DeleteDocumentMessage message)
        {
            _logger.Info("Handle DeleteIndexMessage");

            try
            {
               
                if (_searchProvider.Delete(message.Content))
                {
                    _logger.Info("Handle DeleteDocumentMessage Success");
                }
                else
                {
                    _logger.Error("Handle DeleteDocumentMessage Failed");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
           
        }
    }
}
