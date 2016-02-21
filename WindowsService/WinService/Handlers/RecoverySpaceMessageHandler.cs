using System;
using System.Linq;
using Common.Logging;
using Messages;
using Search;
using Services.Contracts;
using Services.Documents;
using Services.Ioc;
using Services.Messages;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 恢复空间处理消息
    /// </summary>
    public class RecoverySpaceMessageHandler: IMessageHandler<RecoverySpaceMessage>
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        private readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();
        private readonly DocumentService _documentService = ServiceActivator.Get<DocumentService>();
        public void Handle(RecoverySpaceMessage message)
        {
            _logger.Info("Handle RecoverySpaceMessage");

            var space = message.Content;

            if (space != null)
            {
                AddSearchIndex(space);
                Recovery(space);
            }
        }

        private void AddSearchIndex(SpaceObject space)
        {
            _logger.Info("Handle AddSearchIndex");

            try
            {
                _searchProvider.Add(space);

                _logger.Info("Handle AddSearchIndex Success");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }

        private void Recovery(SpaceObject space)
        {
            var ids = _documentService.GetDocuments(f => f.SpaceId == space.Id.ToString()).Select(f => f.Id.ToString());
            _documentService.Recovery(ids.ToArray());
        }
    }
}
