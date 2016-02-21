using System;
using Common.Logging;
using Messages;
using Ninject;
using Search;
using Services.Ioc;
using Services.Messages.Message;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 删除索引的消息处理
    /// </summary>
    public class DeleteIndexMessageHandler: IMessageHandler<DeleteIndexMessage>
    {
        readonly ISearchProvider _searchProvider = ControllerDependencyInjection.Kernel.Get<ISearchProvider>();
        
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(DeleteIndexMessage message)
        {
            _logger.Info("Handle DeleteIndexMessage");

            try
            {
               
                if (_searchProvider.Delete(message.Content))
                {
                    _logger.Info("Handle DeleteIndexMessage Success");
                }
                else
                {
                    _logger.Error("Handle DeleteIndexMessage Failed");
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
