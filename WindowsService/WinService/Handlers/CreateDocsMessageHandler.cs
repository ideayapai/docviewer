using System;
using Common.Logging;
using Messages;
using Search;
using Services.Documents;
using Services.Ioc;
using Services.Messages;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 批量创建文档的处理函数
    /// </summary>
    public class CreateDocsMessageHandler: IMessageHandler<CreateDocsMessage>
    {
        private readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();
        private readonly DocumentService _documentService = ServiceActivator.Get<DocumentService>();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(CreateDocsMessage message)
        {
            _logger.Info("Handle CreateDocsMessageHandler");

            try
            {
                foreach (var content in message.Contents)
                {
                    _searchProvider.Add(content);

                    _documentService.Convert(content);
                }

                _logger.Info("Handle CreateDocsMessageHandler Success");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }

        }
    }
}
