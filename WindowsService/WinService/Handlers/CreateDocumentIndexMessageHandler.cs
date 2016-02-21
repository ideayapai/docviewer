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
    /// 创建索引的处理函数
    /// </summary>
    public class CreateDocumentIndexMessageHandler: IMessageHandler<CreateDocumentIndexMessage>
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        private readonly ISearchProvider _searchProvider = ControllerDependencyInjection.Kernel.Get<ISearchProvider>();
        
        public void Handle(CreateDocumentIndexMessage message)
        {
            _logger.Info("Handle CreateIndexMessage");

            try
            {
                _searchProvider.Add(message.Content);
                _logger.Info("Handle CreateIndexMessage Success");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }
    }
}
