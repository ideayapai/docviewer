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
    public class DeleteMutiIndexMessageHandler: IMessageHandler<DeleteMutiIndexMessage>
    {
        private readonly ISearchProvider _searchProvider = ControllerDependencyInjection.Kernel.Get<ISearchProvider>();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(DeleteMutiIndexMessage message)
        {
            _logger.Info("Handle DeleteMutiIndexMessageHandler");

            try
            {
                
                foreach(var content in message.Contents)
                {
                    if (_searchProvider.Delete(content))
                    {
                        _logger.Info("Handle DeleteMutiIndexMessageHandler Success");
                    }
                    else
                    {
                        _logger.Error("Handle DeleteMutiIndexMessageHandler Failed");
                    }
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
