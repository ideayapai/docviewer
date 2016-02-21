using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using Common.Logging;
using Infrasturcture.DomainObjects;
using Services.Enums;
using Services.Models;
using Services.Spaces;

namespace WebAPI2.Controllers
{
    /// <summary>
    /// 文件夹管理API
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SpaceController : BaseApiController
    {
        private readonly SpaceService _spaceService;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="spaceService"></param>
        public SpaceController(SpaceService spaceService)
        {
            _spaceService = spaceService;
        }

        /// <summary>
        /// 添加文件夹
        /// </summary>
        /// <param name="parentId">文件夹Id,为空为根文件夹下创建</param>
        /// <param name="name">文件夹名称</param>
        /// <param name="userId">创建者用户Id</param>
        /// <param name="userName">创建者名称</param>
        /// <param name="depId">部门Id</param>
        /// <returns>成功返回文件夹对象</returns>
        [HttpPost]
        public SpaceContract Add(string parentId, string name, string userId, string userName, string depId)
        {
            _logger.InfoFormat("Add Space parentId:{0},name:{1}, userId:{2}, userName:{3}, depId:{4}", 
                parentId, name, userId, userName);

            try
            {
                return
                    _spaceService.Add(parentId, Guid.NewGuid().ToString(), name, userId, userName, depId, Visible.Public)
                        .ToObject<SpaceContract>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

        }

     
        /// <summary>
        /// 删除一个文件夹对象到回收站
        /// </summary>
        /// <param name="Id">文件夹Id</param>
        /// <returns>返回移动回收站的对象</returns>
        [HttpDelete]
        public SpaceContract Remove(string Id)
        {
            _logger.InfoFormat("Remove Space Id:{0}", Id);

            if (string.IsNullOrWhiteSpace(Id))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            try
            {
                var space = _spaceService.MoveToTrash(Id).ToObject<SpaceContract>();
                if (space == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                return space;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

    
        /// <summary>
        /// 返回子文件夹列表
        /// </summary>
        /// <param name="parentId">父文件夹Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="depId">部门Id</param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<SpaceContract> GetSpaces(string parentId, string userId, string depId)
        {
            _logger.InfoFormat("GetSpaces parentId:{0}", parentId);

            try
            {
                return _spaceService.GetChildren(parentId, userId, depId).ConvertAll(f => f.ToObject<SpaceContract>());
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// 根据文件夹Id返回文件夹对象
        /// </summary>
        /// <param name="id">文件夹Id</param>
        /// <returns>文件夹对象</returns>
        [HttpGet]
        public SpaceContract Get(string id)
        {
            _logger.InfoFormat("Get Space id:{0}", id);

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
             
            try
            {
                var space = _spaceService.GetSpace(id);
                if (space == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                return space.ToObject<SpaceContract>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

        }
    }
}
