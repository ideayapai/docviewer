using System;
using Common.Logging;
using Search;
using Services.Ioc;
using Services.Messages;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 重命名空间处理函数
    /// </summary>
    public class UpdateSpaceMessageHandler: IMessageHandler<UpdateSpaceMessage>
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        private readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();

        public void Handle(UpdateSpaceMessage message)
        {
            _logger.Info("Handle RenameSpaceMessage");

            try
            {
                _logger.Debug("删除原有索引");
                _searchProvider.Delete(message.Content);

                _logger.Debug("添加原有索引");
                _searchProvider.Add(message.Content);

                _logger.Info("Handle RenameSpaceMessage Success");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }
    }
}
