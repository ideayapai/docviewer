using Common.Logging;
using Search;
using Services.Documents;

namespace Services.Search.UnitOfWork
{
    public class AddDocumentIndexerxUnitOfWork: IUnitOfWork
    {
        private readonly DocumentService _documentService;
        private readonly ISearchProvider _searchProvider;

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public AddDocumentIndexerxUnitOfWork(DocumentService documentService, ISearchProvider searchProvider)
        {
            _documentService = documentService;
            _searchProvider = searchProvider;
        }

        public void DoWork()
        {
            _logger.Info("更新文档索引...");
         
            var documents = _documentService.GetExistsDocuments();
            if (documents != null && documents.Count > 0)
            {
                _searchProvider.AddList(documents, false);
            }

        }
    }
}
