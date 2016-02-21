using System;
using Common.Logging;
using Messages;
using Search;
using Services.Ioc;
using Services.Messages;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 删除空间的处理函数
    /// </summary>
    public class DeleteSpaceMessageHandler: IMessageHandler<DeleteSpaceMessage>
    {
        private readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(DeleteSpaceMessage message)
        {
            _logger.Info("Handle DeleteSpaceMessageHandler");

            var space = message.Content;
            if (space != null)
            {
                 DeleteSearchIndex(message);
                
            }
           
        }

        private void DeleteSearchIndex(DeleteSpaceMessage message)
        {
            _logger.Info("DeleteSearchIndex");

            try
            {
                if (_searchProvider.Delete(message.Content))
                {
                    _logger.Info("Handle DeleteSpaceMessageHandler Success");
                }
                else
                {
                    _logger.Error("Handle DeleteSpaceMessageHandler Failed");
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
