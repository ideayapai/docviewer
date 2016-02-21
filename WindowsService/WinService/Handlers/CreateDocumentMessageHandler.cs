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
    /// 创建文档的处理函数
    /// </summary>
    public class CreateDocumentMessageHandler: IMessageHandler<CreateDocumentMessage>
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        private readonly DocumentService _documentService = ServiceActivator.Get<DocumentService>();

        private readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();
        
        public void Handle(CreateDocumentMessage message)
        {
            _logger.Info("Handle CreateDocumentMessage");

            try
            {
                _logger.Info("创建文档索引");
                _searchProvider.Add(message.Content);

                _logger.Info("执行文档转换");
                _documentService.Convert(message.Content);

                _logger.Info("Handle CreateDocumentMessage Success");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }
    }
}
