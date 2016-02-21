using System;
using System.Web.Mvc;
using Common.Logging;
using Infrasturcture.Web;
using Services.Context;
using Services.Contracts;
using Services.Enums;
using Services.Spaces;
using WebSite2.Models;

namespace WebSite2.Controllers
{
 
    public class SpaceController : BaseController
    {
        private readonly SpaceService _spaceService;
        private readonly SpaceTreeService _spaceTreeService;
        private readonly ContextService _contextService;
        private readonly IHttpRequestProvider _requestProvider;

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public SpaceController(SpaceService spaceService, SpaceTreeService spaceTreeService, IHttpRequestProvider requestProvider, ContextService contextService)
        {
            _spaceService = spaceService;
            _spaceTreeService = spaceTreeService;
            _requestProvider = requestProvider;
            _contextService = contextService;
        }

        public ActionResult Index()
        {
            var allSpaces = _spaceService.GetAllSpaces();
            if (allSpaces == null || allSpaces.Count == 0)
            {
                return View("Empty");
            }
            return View(allSpaces);
        }

        public ActionResult Add(string Id)
        {
            var space = new SpaceObject();
            if (!string.IsNullOrEmpty(Id))
            {
                space.ParentId = Id;
            }
            return View(space);
        }

        [HttpPost]
        public ActionResult Add()
        {
            _logger.Info("Enter SpaceController Add");

            var userId = _contextService.UserId;
            var userName = _contextService.NickName;
            var depId = _contextService.DepId;
            var spaceName = _requestProvider["SpaceName"];
            var parentId = _requestProvider["ParentId"];
            Visible visiblity = Visible.Dep;

            //判断空间重名问题
            var children = _spaceService.GetChildren(parentId, userId, depId);
            var spanObject = children.Find(f => f.SpaceName == spaceName);
            if (spanObject != null)
            {
                return Json(spanObject);
            }
            try
            {
                _logger.Info(string.Format("Add Space, spacename:[{0}]；userId:[{1}]；userName:[{2}];parentId:[{3}]。", spaceName, userId, userName,parentId));

                var space = _spaceService.Add(parentId, Guid.NewGuid().ToString(), spaceName, userId, userName, depId, visiblity);
                if (space != null)
                {
                    _contextService.SpaceTreeHtml = _spaceTreeService.GetOrSetSpaceTree(_contextService.UserId);

                    return Json(new SpaceViewModel(space, _contextService.UserId));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
                _logger.Error(ex.Message);
            }

            return Json(new SpaceViewModel(new SpaceObject(), _contextService.UserId));
        }

        public ActionResult Delete(string Id)
        {
            _spaceService.Delete(Id);

            return RedirectToAction("Index");
        }

        public ActionResult Detail(string Id)
        {
            return RedirectToAction("Space", "Home", new {spaceid = Id});
        }

    }
}
