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
    public class CreateMutiIndexMessageHandler : IMessageHandler<CreateMutiIndexMessage>
    {
        readonly ISearchProvider _searchProvider = ControllerDependencyInjection.Kernel.Get<ISearchProvider>();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(CreateMutiIndexMessage message)
        {
            _logger.Info("Handle CreateMutiIndexMessageHandler");

            try
            {
                
                foreach(var content in message.Contents)
                {
                    _searchProvider.Add(content);
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
