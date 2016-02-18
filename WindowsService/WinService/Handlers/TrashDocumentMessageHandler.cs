using System;
using Common.Logging;
using Messages;
using Search;
using Services.Ioc;
using Services.Messages;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 将文档放到回收站的处理函数
    /// </summary>
    public class TrashDocumentMessageHandler: IMessageHandler<TrashDocumentMessage>
    {
        readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();
        
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(TrashDocumentMessage message)
        {
            _logger.Info("Handle TrashDocumentMessage");

            try
            {
                if (_searchProvider.Delete(message.Content))
                {
                    _logger.Info("Handle TrashDocumentMessage Success");
                }
                else
                {
                    _logger.Error("Handle TrashDocumentMessage Failed");
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
