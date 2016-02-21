using System;
using System.Collections.Generic;
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
    /// 批量移动空间到回收站的处理函数
    /// </summary>
    public class TrashSpacesMessageHandler: IMessageHandler<TrashSpacesMessage>
    {
        private readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();
        private readonly DocumentService _documentService = ServiceActivator.Get<DocumentService>();
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(TrashSpacesMessage message)
        {
            _logger.Info("Handle TrashSpacesMessageHandler");

            var spaces = message.Contents;

            if (spaces != null && spaces.Count > 0)
            {
                DeleteIndex(spaces);

                MoveToTrash(spaces);
            }
        }

        private void MoveToTrash(List<SpaceObject> spaces)
        {
            foreach (var space in spaces)
            {
                var ids = _documentService.GetDocuments(f => f.SpaceId == space.Id.ToString()).Select(f => f.Id.ToString());
                _documentService.MoveToTrash(ids.ToArray());
            }
          
        }

        private void DeleteIndex(List<SpaceObject> spaces)
        {
            try
            {
                if (_searchProvider.DeleteList(spaces))
                {
                    _logger.Info("Handle TrashSpacesMessageHandler Success");
                }
                else
                {
                    _logger.Error("Handle TrashSpacesMessageHandler Failed");
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
