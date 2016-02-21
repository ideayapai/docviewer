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
    /// 移动空间到回收站的消息
    /// </summary>
    public class TrashSpaceMessageHandler: IMessageHandler<TrashSpaceMessage>
    {
        private readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();
        private readonly DocumentService _documentService = ServiceActivator.Get<DocumentService>();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(TrashSpaceMessage message)
        {
            _logger.Info("Handle TrashSpaceMessage");

            var space = message.Content;

            if (space != null)
            {
                DeleteSearchIndex(space);
                
                MoveToTrash(space);
            }

        }


        private void DeleteSearchIndex(SpaceObject space)
        {
            _logger.Info("DeleteSearchIndex");

            try
            {
                if (_searchProvider.Delete(space))
                {
                    _logger.Info("Handle DeleteSearchIndex Success");
                }
                else
                {
                    _logger.Error("Handle DeleteSearchIndex Failed");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }

        private void MoveToTrash(SpaceObject space)
        {
            var ids = _documentService.GetDocuments(f => f.SpaceId == space.Id.ToString()).Select(f => f.Id.ToString());
            _documentService.MoveToTrash(ids.ToArray());
        }
    }
}
