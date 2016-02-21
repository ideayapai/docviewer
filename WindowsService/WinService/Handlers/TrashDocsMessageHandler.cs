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
    public class TrashDocsMessageHandler: IMessageHandler<TrashDocsMessage>
    {
        private readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(TrashDocsMessage message)
        {
            _logger.Info("Handle TrashDocsMessage");

            try
            {
                if (_searchProvider.DeleteList(message.Contents))
                {
                    _logger.Info("Handle TrashDocsMessage Success");
                }
                else
                {
                    _logger.Error("Handle TrashDocsMessage Failed");
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
