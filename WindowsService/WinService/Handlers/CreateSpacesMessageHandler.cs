using System;
using Common.Logging;
using Messages;
using Search;
using Services.Ioc;
using Services.Messages;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 批量创建控件的处理函数
    /// </summary>
    public class CreateSpacesMessageHandler: IMessageHandler<CreateSpacesMessage>
    {
        readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(CreateSpacesMessage message)
        {
            _logger.Info("Handle CreateSpacesMessage");

            try
            {
                foreach (var content in message.Contents)
                {
                    _searchProvider.Add(content);
                }

                _logger.Info("Handle CreateSpacesMessage Success");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }

        }
    }
}
