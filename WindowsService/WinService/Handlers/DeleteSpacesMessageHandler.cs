using System;
using Common.Logging;
using Messages;
using Search;
using Services.Ioc;
using Services.Messages;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 批量删除空间的处理函数
    /// </summary>
    public class DeleteSpacesMessageHandler: IMessageHandler<DeleteSpacesMessage>
    {
        private readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(DeleteSpacesMessage message)
        {
            _logger.Info("Handle DeleteSpacesMessage");

            try
            {
                if (_searchProvider.DeleteList(message.Contents))
                {
                    _logger.Info("Handle DeleteSpacesMessage Success");
                }
                else
                {
                    _logger.Error("Handle DeleteSpacesMessage Failed");
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
