using System;
using System.Web.Mvc;
using Common.Logging;
using Documents.Enums;
using Infrasturcture.Store;
using Services.Context;
using Services.Contracts;
using Services.Documents;
using Services.Spaces;
using WebSite2.Models;

namespace WebSite2.Controllers
{
    /// <summary>
    /// 显示文档的Controller
    /// </summary>
    public class ViewController : BaseController
    {
        private readonly DocumentService _documentService;
        private readonly SpaceService _spaceService;
        private readonly IStorePolicy _storePolicy;
        private readonly ContextService _contextService;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public ViewController(DocumentService documentService,
                              SpaceService spaceService, 
                              IStorePolicy storePolicy,
                              ContextService contextService)
        {
            _documentService = documentService;
            _spaceService = spaceService;
            _contextService = contextService;
            _storePolicy = storePolicy;
            _storePolicy = storePolicy;
        }

        public ActionResult Index(string Id)
        {
            _logger.InfoFormat("进入浏览界面 {0}", Id);

            Guid result;
            if (string.IsNullOrWhiteSpace(Id) || !Guid.TryParse(Id, out result))
            {
                return View("NotFound", new DisplayViewModel());
                
            }

            var document = _documentService.GetDocument(Id);
            if (document == null || document.IsDelete)
            {
                return View("NotFound", new DisplayViewModel());
            }

            if (!_storePolicy.Exist(document.StorePath))
            {
                return View("NotFound", new DisplayViewModel());
            }

            var parentSpaces = _spaceService.GetParentsChain(document.SpaceId);
            var space = _spaceService.GetSpace(document.SpaceId);
            _contextService.CurrentSpace = space;
            return View(new DisplayViewModel
                        {
                            Document = document,
                            CurrentSpace = space,
                            ParentSpaces = parentSpaces,
                        });
        }
     

        public ActionResult Preview(string Id)
        {
            _logger.InfoFormat("进入浏览界面 {0}", Id);

            Guid result;
            if (string.IsNullOrWhiteSpace(Id) || !Guid.TryParse(Id, out result))
            {
                return View("NotFound", new DisplayViewModel());

            }

            var document = _documentService.GetDocument(Id);
            if (document == null || document.IsDelete)
            {
                return View("NotFound", new DisplayViewModel());

            }

            if (!_storePolicy.Exist(document.StorePath))
            {
                return View("NotFound", new DisplayViewModel());
            }

            try
            {
                return GetDisplayView(document);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                return View("Unknow", new DisplayViewModel
                    {
                        Document = document,
                        ActiveMenuType = CurrentMenu,
                    });
            }
            

        }

        private ActionResult GetDisplayView(DocumentObject document)
        {
            switch (document.DocumentCategory)
            {
                case DocumentCategory.Office:
                case DocumentCategory.Text:
                    document = _documentService.Convert(document);

                    return View("Html", new DisplayViewModel
                    {
                        Document = document,
                    });

                case DocumentCategory.CAD:
                    document = _documentService.Convert(document);

                    return View("Svg", new DisplayViewModel
                    {
                        Document = document,
                    });
                case DocumentCategory.Image:
                    return View("Image", new DisplayViewModel
                    {
                        Document = document,
                        ActiveMenuType = CurrentMenu,
                    });

                default:
                    return View("Unknow", new DisplayViewModel
                    {
                        Document = document,
                        ActiveMenuType = CurrentMenu,
                    });
            }
        }
    }
}