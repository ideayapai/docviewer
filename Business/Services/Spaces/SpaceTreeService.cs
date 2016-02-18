using System;
using System.Collections.Generic;
using Common.Logging;
using Services.Context;
using Services.Contracts;

namespace Services.Spaces
{
    public class SpaceTreeService
    {
        private readonly SpaceService _spaceService;
        private readonly ContextService _contextService;

        private ILog _logger = LogManager.GetCurrentClassLogger();


        public SpaceTreeService(SpaceService spaceService, ContextService contextService)
        {
            _spaceService = spaceService;
            _contextService = contextService;

        }
        /// <summary>
        /// 需要重构
        /// </summary>
        /// <param name="userId"></param>
        public string GetOrSetSpaceTree(string userId)
        {
            string spaceTreeHtml = string.Empty;

            try
            {
                var spaceObjects = _spaceService.GetVisibleSpaces(userId, _contextService.DepId);
                var defaultSpace = spaceObjects.Find(f => f.ParentId == string.Empty);
                if (defaultSpace != null)
                {
                    _contextService.DefaultSpaceId = defaultSpace.Id.ToString();
                    spaceTreeHtml += SetChildrenTree(defaultSpace.Id.ToString(), spaceObjects);
                    spaceTreeHtml = string.Format("[{0}]", spaceTreeHtml.TrimEnd(','));
                }
            }
            catch (Exception ex)
            {
                
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
          

            return spaceTreeHtml;

        }

        private string SetChildrenTree(string id, List<SpaceObject> spaceNodes)
        {
            var result = string.Empty;
            var now = spaceNodes.Find(f => f.Id == Guid.Parse(id));
            result += "{\"name\":\"" + now.SpaceName + "\",\"xid\":\"" + now.Id + "\"," + "open:true,";
            var childrens = spaceNodes.FindAll(f => f.ParentId == id);
            if (childrens.Count > 0)
            {
                result += "\"children\": [";
                foreach (var child in childrens)
                {
                    result +=SetChildrenTree(child.Id.ToString(), spaceNodes);
                }
                result = result.TrimEnd(',') + "]},";
            }
            else
            {
                result += "\"iconSkin\":\"ind\"},";
            }

            return result;
        }
    }
}
