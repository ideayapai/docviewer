using System;
using Common.Logging;
using Messages;
using Search;
using Services.Ioc;
using Services.Messages;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 创建空间的处理函数
    /// </summary>
    public class CreateSpaceMessageHandler: IMessageHandler<CreateSpaceMessage>
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        private readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();

        public void Handle(CreateSpaceMessage message)
        {
            _logger.Info("Handle CreateSpaceMessage");

            try
            {
                _searchProvider.Add(message.Content);

                _logger.Info("Handle CreateSpaceMessage Success");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }
    }
}
