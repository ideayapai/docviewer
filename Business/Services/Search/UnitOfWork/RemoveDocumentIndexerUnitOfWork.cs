using Common.Logging;
using Search;
using Services.Documents;

namespace Services.Search.UnitOfWork
{
    public class RemoveDocumentIndexerxUnitOfWork: IUnitOfWork
    {
        private readonly DocumentService _documentService;
        private readonly ISearchProvider _searchProvider;

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        private RemoveDocumentIndexerxUnitOfWork(DocumentService documentService, ISearchProvider searchProvider)
        {
            _documentService = documentService;
            _searchProvider = searchProvider;
        }

        
        public void DoWork()
        {
            _logger.Info("删除回收站中文档的索引...");
            var documents = _documentService.GetAllTrashDocuments();
            if (documents != null && documents.Count > 0)
            {
                _searchProvider.DeleteList(documents);
            }

        }
    }
}
