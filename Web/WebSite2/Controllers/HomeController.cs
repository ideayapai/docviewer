using System;
using System.Web.Mvc;
using Common.Logging;
using Services.Context;
using Services.Contracts;
using Services.Documents;
using Services.Spaces;
using WebSite2.Models;

namespace WebSite2.Controllers
{
    public class HomeController : BaseController
    {
        private readonly DocumentService _documentService;
        private readonly SpaceService _spaceService;
        private readonly SpaceTreeService _spaceTreeService;
        private readonly ContextService _contextService;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public HomeController(DocumentService documentService, 
                              SpaceService spaceService,
                              SpaceTreeService spaceTreeService,
                              ContextService contextService)
        {
            _documentService = documentService;
            _spaceService = spaceService;
            _spaceTreeService = spaceTreeService;
            _contextService = contextService;
        }

        public ActionResult Index(int? page)
        {
            _logger.Info("HomeIndex,进入首页");


            var currentSpace = _spaceService.GetOrAddDefaultSpace(_contextService.NickName,
                                                                  _contextService.UserId,
                                                                  _contextService.DepId);
            if (page == 0 || page == null)
            {
                return View(BuildHomeViewModel(currentSpace, 1, "Home/Index"));     
            }

            return View(BuildHomeViewModel(currentSpace, page.Value, "Home/Index"));
        }

        public ActionResult Space(string spaceid,int? page)
        {
            _logger.InfoFormat("HomeIndex,进入空间:{0}", spaceid);

            //如果Guid为空或者Guid格式不对,返回首页
            Guid result;
            if (string.IsNullOrEmpty(spaceid) || !Guid.TryParse(spaceid, out result))
            {
                spaceid = _contextService.DefaultSpaceId;
            }

            var currentSpace = _spaceService.GetSpace(spaceid);
            if (currentSpace == null)
            {
                return View("NotFound", new HomeViewModel());
            }

            if (page == 0 || page == null)
            {
                return View(BuildHomeViewModel(currentSpace, 1, string.Format("Home/Space/{0}/page", spaceid)));     
            }

            return View(BuildHomeViewModel(currentSpace, page.Value, string.Format("Home/Space/{0}/page", spaceid)));
        }


        /**
         * 测试上传
         * 
         */
        public ActionResult UploadTest()
        {
            return View();
        }

        private HomeViewModel BuildHomeViewModel(SpaceObject space, int page, string pageCodeType)
        {
            var documents = _documentService.GetVisibleDocuments( _contextService.UserId,
                                                                  _contextService.DepId,
                                                                  f=>f.SpaceId == space.Id.ToString());

            var childSpaces = _spaceService.GetChildren(space.Id.ToString(), _contextService.UserId, _contextService.DepId);
            var parentSpaces = _spaceService.GetParentsChain(space.Id.ToString());

            var homeViewModels = new HomeViewModel
            {
                CurrentSpace = new SpaceViewModel(space, _contextService.UserId),
                Documents = new DocumentViewModelCollection(documents, _contextService.UserId),

                ChildSpaces = new SpaceViewModelCollection(childSpaces, _contextService.UserId),
                ParentSpaces = new SpaceViewModelCollection(parentSpaces, _contextService.UserId),
                ActiveMenuType = MenuType.All,
                PageCode = PageService.GetPageCode(documents.Count, page, pageCodeType),
            };

            CurrentMenu = MenuType.All;
            _contextService.CurrentSpace = homeViewModels.CurrentSpace.SpaceObject;

            if (string.IsNullOrEmpty(_contextService.SpaceTreeHtml))
            {
                _contextService.SpaceTreeHtml = _spaceTreeService.GetOrSetSpaceTree(_contextService.UserId);
            }
                
            return homeViewModels;
        }
    }



}
