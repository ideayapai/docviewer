using System;
using Common.Logging;
using Messages;
using Services.Documents;
using Services.Ioc;
using Services.Messages;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 转换文档处理函数
    /// </summary>
    public class ConvertDocumentMessageHandler : IMessageHandler<ConvertDocumentMessage>
    {
        private readonly DocumentService _documentService = ServiceActivator.Get<DocumentService>();
        
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(ConvertDocumentMessage message)
        {
            _logger.Debug("ConvertDocument Handle Message");

            try
            {
                _documentService.Convert(message.Document);

                _logger.Debug("ConvertDocument Handle Message Complete.");
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }
    }
}
