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
    /// 批量恢复空间
    /// </summary>
    public class RecoverySpacesMessageHandler: IMessageHandler<RecoverSpacesMessage>
    {
        private readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();
        private readonly DocumentService _documentService = ServiceActivator.Get<DocumentService>();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(RecoverSpacesMessage message)
        {
            _logger.Info("Handle RecoverSpacesMessage");
            var spaces = message.Contents;
            if (spaces != null && spaces.Count > 0)
            {
                AddIndex(message.Contents);
                Recovery(message.Contents);
            }
            
        }

        private void AddIndex(List<SpaceObject> spaces)
        {
            try
            {
                foreach (var content in spaces)
                {
                    _searchProvider.Add(content);
                }

                _logger.Info("Handle RecoverSpacesMessage Success");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }

        private void Recovery(List<SpaceObject> spaces)
        {
            foreach (var space in spaces)
            {
                var ids = _documentService.GetDocuments(f => f.SpaceId == space.Id.ToString()).Select(f => f.Id.ToString());
                _documentService.Recovery(ids.ToArray());
            }

        }
    }
}
