using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Common.Logging;
using Documents.Enums;
using Infrasturcture.DomainObjects;
using Infrasturcture.Store;
using Infrasturcture.Web;
using Krystalware.SlickUpload;
using Services.Contracts;
using Services.Documents;
using Services.Enums;
using Services.Models;
using Services.Spaces;

namespace WebAPI2.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UploadController : Controller
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        private readonly DocumentService _documentService;
        private readonly SpaceService _spaceService;
        private readonly IHttpRequestProvider _requestProvider;

         /// <summary>
        /// DocumentController
        /// </summary>
        /// <param name="documentService"></param>
        /// <param name="spaceService"></param>
        public UploadController(DocumentService documentService, 
                                  SpaceService spaceService, 
                                  IHttpRequestProvider requestProvider,
                                  IStorePolicy storePolicy)
        {
      
            _documentService = documentService;
            _spaceService = spaceService;
             _requestProvider = requestProvider;

        }

        //
        // GET: /Upload/
        /// <summary>
        /// path数据结构
        /// 001_西山区|4567_虹桥立交|8889_日常巡检 解析为三个文件夹
        /// 西山区(001)
        ///   |
        ///   |---虹桥立交(4567)
        ///         |
        ///         |----日常巡检(8889)
        /// </summary>
        public JsonResult Add(UploadSession uploadSession, string userId, string spaceId, string userName, string depId, string visible, string path)
        {
            //var userId = _requestProvider["UserId"];
            //var spaceId = _requestProvider["SpaceId"];
            //var userName = _requestProvider["UserName"];
            //var depId = _requestProvider["DepId"];
            //var visible = _requestProvider["Visible"];
            //var path = _requestProvider["Path"];
            _logger.Info("DocumentController 添加文档.");

            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.Error("文档参数错误, 缺少 UserId 参数.");
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            _logger.InfoFormat("文档参数userId:{0}", userId);

            if (string.IsNullOrWhiteSpace(userName))
            {
                _logger.Error("文档参数错误, 缺少 userName 参数.");
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            _logger.InfoFormat("文档参数userName:{0}", userName);

            if (string.IsNullOrWhiteSpace(depId))
            {
                _logger.Error("文档参数缺少 depId 参数,默认设置为空");
                depId = string.Empty;
            }
            _logger.InfoFormat("文档参数depId:{0}", depId);

            var visiblity = Visible.Public;
            if (!string.IsNullOrWhiteSpace(visible))
            {

                visiblity = (Visible)Enum.Parse(typeof(Visible), visible);
                _logger.InfoFormat("文档参数为:{0}", visiblity);
            }
            else
            {
                _logger.Info("文档参数缺少 visible 参数,默认设置为Public");
            }

            SpaceObject space;
            if (string.IsNullOrWhiteSpace(path))
            {
                _logger.Info("文档参数缺少 path 参数,采用spaceId参数");
                space = string.IsNullOrWhiteSpace(spaceId) ?
                    _spaceService.GetDefaultSpace() : _spaceService.GetSpace(spaceId);
            }
            else
            {
                _logger.InfoFormat("文档参数path:{0}", path);
                space = _spaceService.MakeSpace(string.Empty, path, userId, userName, depId, visiblity);
            }

            if (uploadSession.UploadedFiles.Count == 0)
            {
                _logger.Error("文档参数错误, 上传文件失败.");
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            _logger.Info(string.Format("Add Document, spaceId:[{0}]；userId:[{1}]；userName:[{2}]。", spaceId, userId, userName));

            try
            {
                var file = uploadSession.UploadedFiles.First();
                var document = DocumentBuilder.Build(file,
                                                    space.Id.ToString(),
                                                    space.SpaceSeqNo,
                                                    space.SpaceName,
                                                    userId,
                                                    userName,
                                                    depId,
                                                    visiblity);
                var documentObj = _documentService.Add(document);
                if (documentObj.DocumentCategory != DocumentCategory.Image)
                {
                    documentObj.DisplayPath = documentObj.PreviewUrl;
                }

                return Json(documentObj.ToObject<DocumentContract>());
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