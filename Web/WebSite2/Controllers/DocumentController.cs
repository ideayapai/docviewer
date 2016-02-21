using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common.Logging;
using Documents.Enums;
using Infrasturcture.Errors;
using Infrasturcture.Store;
using Infrasturcture.Web;
using Krystalware.SlickUpload;
using Services.CacheService;
using Services.Context;
using Services.Contracts;
using Services.Documents;
using Services.Enums;
using Services.Spaces;
using WebSite2.Models;

namespace WebSite2.Controllers
{
    /// <summary>
    /// 文档控制Controller
    /// </summary>
    public class DocumentController : BaseController
    {
        private readonly DocumentService _documentService;
        private readonly SpaceService _spaceService;
        private readonly SpaceTreeService _spaceTreeService;
        private readonly IHttpRequestProvider _requestProvider;
        private readonly IStorePolicy _storePolicy;
        private readonly ContextService _contextService;

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public DocumentController(DocumentService documentService,
                                  SpaceService spaceService, 
                                  SpaceTreeService spaceTreeService, 
                                  IStorePolicy storePolicy,
                                  IHttpRequestProvider requestProvider,
                                  ContextService contextService)
        {
            _documentService = documentService;
            _spaceService = spaceService;
            _spaceTreeService = spaceTreeService;
            _requestProvider = requestProvider;
            _contextService = contextService;
            _storePolicy = storePolicy;
        }

        /// <summary>
        /// 添加一个或多个文档
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Add()
        {
            _logger.Info("DocumentController 添加文档");

            try
            {
                var fileCollection = _requestProvider.FileCollection;

                var userId = _contextService.UserId;
                var userName = _contextService.NickName;
                var depId = _contextService.DepId;
                var timeStamp = _requestProvider["TimeStamp"];
                var spaceId = _requestProvider["SpaceId"];
                
                var space = string.IsNullOrWhiteSpace(spaceId) ? _spaceService.GetDefaultSpace() : _spaceService.GetSpace(spaceId);

                _logger.Info(string.Format("Add Document, spaceId:[{0}]；spaceSeqNo:[{1}], spaceName:[{2}], userId:[{3}]；userName:[{4}]。",
                    space.Id, space.SpaceSeqNo, space.SpaceName, userId, userName));



                var documents = new List<DocumentObject>();

                for (int i = 0; i < fileCollection.Count; ++i)
                {
                    var document = DocumentBuilder.Build(fileCollection[i], space.Id.ToString(), space.SpaceSeqNo, space.SpaceName, userId, userName, depId, Visible.Public);
                    documents.Add(document);
                    var mimeType = MimeMapping.GetMimeMapping(document.FileName);
                    _storePolicy.AddStream(fileCollection[i].FileStream, mimeType, document.StorePath);
                }
               

                var contracts = _documentService.Add(documents);
                if (!string.IsNullOrWhiteSpace(timeStamp))
                {
                    MemoryContainer.Push(timeStamp, documents);
                }

                return Json(new UploadViewModel(ErrorMessages.Success,
                                                ErrorMessages.GetErrorMessages(ErrorMessages.Success),
                                                contracts));
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
                _logger.Error(ex.Message);
            }

            _logger.Error(ErrorMessages.GetErrorMessages(ErrorMessages.UploadFailed));
            return Json(new UploadViewModel(ErrorMessages.UploadFailed,
                                            ErrorMessages.GetErrorMessages(ErrorMessages.UploadFailed)));
        }

        public ActionResult Upload(UploadSession session, string spaceId)
        {
            _logger.Info("DocumentController 添加文档");

            try
            {
                var files = session.UploadedFiles;

                var userId = _contextService.UserId;
                var userName = _contextService.NickName;
                var depId = _contextService.DepId;
                var space = _spaceService.GetSpace(spaceId);

                _logger.Info(string.Format("Add Document, spaceId:[{0}]；spaceSeqNo:[{1}], spaceName:[{2}], userId:[{3}]；userName:[{4}]。",
                    space.Id, space.SpaceSeqNo, space.SpaceName, userId, userName));

                var documents = new List<DocumentObject>();
                foreach (var file in files)
                {
                    var document = DocumentBuilder.Build(file, space.Id.ToString(), space.SpaceSeqNo, space.SpaceName, userId, userName, depId, Visible.Public);
                    documents.Add(document);
                }

                var contracts = _documentService.Add(documents);

                return Json(new UploadViewModel(ErrorMessages.Success,
                                                ErrorMessages.GetErrorMessages(ErrorMessages.Success), contracts));

            }
            catch (Exception ex)
            {
                _logger.Error(ex.StackTrace);
                _logger.Error(ex.Message);
                return Json(new UploadViewModel(ErrorMessages.UploadFailed, ex.Message));
            }
        }

        /// <summary>
        /// 最近的文档
        /// </summary>
        /// <returns></returns>
        public ActionResult Recent()
        {
            _logger.Info("最近的文档");

            var model = new DocumentIndexViewModel
            {
                DocumentModels = new DocumentViewModelCollection(_documentService.GetVisibleDocuments(_contextService.UserId,
                                                                      _contextService.DepId),_contextService.UserId),
                ActiveMenuType = MenuType.Recent,
            };
            CurrentMenu = MenuType.Recent;
            return View(model);
        }


        /// <summary>
        /// Office文档
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Office(int? page)
        {
            _logger.Info("Office文档列表");

            if (page == null || page == 0)
            {
                page = 1;
            }
                
            var documents = _documentService.GetVisibleDocuments(_contextService.UserId,
                                                                 _contextService.DepId,
                                                                 f=>f.DocumentCategory == DocumentCategory.Office);
            var model = new DocumentIndexViewModel
            {
                DocumentModels = new DocumentViewModelCollection(
                    documents.Take(page.Value * PageService.PageShowCount).Skip((page.Value - 1) * PageService.PageShowCount).ToList(),
                    _contextService.UserId),

                ActiveMenuType = MenuType.Office,
                PageCode = PageService.GetPageCode(documents.Count, page.Value, "Document/Office")
            };
            CurrentMenu = MenuType.Office;
            return View(model);
        }

        /// <summary>
        /// 图片
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Image(int? page)
        {
            _logger.Info("图片列表");

            if (page == null || page == 0)
                page = 1;
            var documents = _documentService.GetVisibleDocuments(_contextService.UserId,
                                                                 _contextService.DepId,
                                                                 f=>f.DocumentCategory == DocumentCategory.Image);
            var model = new DocumentIndexViewModel
            {
                DocumentModels = new  DocumentViewModelCollection(
                    documents, _contextService.UserId),
                ActiveMenuType = MenuType.Image,
                PageCode = PageService.GetPageCode(documents.Count, page.Value, "Document/Image")
            };
            CurrentMenu = MenuType.Image;
            return View(model);
        }

        /// <summary>
        /// 工程图
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult CAD(int? page)
        {
            _logger.Info("工程图列表");

            if (page == null || page == 0)
                page = 1;
            var documents = _documentService.GetVisibleDocuments(_contextService.UserId,
                                                                 _contextService.DepId,
                                                                 f=>f.DocumentCategory == DocumentCategory.CAD);
            var model = new DocumentIndexViewModel
            {
                DocumentModels = new DocumentViewModelCollection(
                    documents,
                    _contextService.UserId),

                ActiveMenuType = MenuType.Cad,
                PageCode = PageService.GetPageCode(documents.Count, page.Value, "Document/CAD")
            };
            CurrentMenu = MenuType.Cad;
            return View(model);
        }


        /// <summary>
        /// 回收站
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Trash(int? page)
        {
            _logger.Info("DocumentController 回收站");

            if (page == null || page == 0)
                page = 1;

            var trashSpaces = _spaceService.GetTrashSpaces(_contextService.UserId);
            var documents = _documentService.GetTrashDocuments(_contextService.UserId);

            var model = new TrashViewModel
            {
                SpaceModels = trashSpaces,
                DocumentModels = new DocumentViewModelCollection(
                    documents,
                    _contextService.UserId),
                ActiveMenuType = MenuType.Trash,
                PageCode = PageService.GetPageCode(documents.Count, page.Value, "Document/Trash")
            };

            CurrentMenu = MenuType.Trash;
            return View(model);
        }

        /// <summary>
        /// 从回收站还原文档
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Recovery(string Id)
        {
            _logger.Info("还原文档");

            _documentService.Recovery(Id);
            CurrentMenu = MenuType.Trash;
            return RedirectToAction("Trash", "Document");
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReName(string id, string type, string name)
        {
            _logger.InfoFormat("重命名:{0},{1},{2}", id, type, name);

            try
            {
                if (type == "file")
                {
                    var document = _documentService.ReName(id, name);

                    return  Json(new UpdateViewModel(ErrorMessages.Success,
                                                     ErrorMessages.GetErrorMessages(ErrorMessages.Success),
                                                     "file"));

                }

                if (type == "folder")
                {
                    var space = _spaceService.ReName(id, name);

                    _contextService.SpaceTreeHtml = _spaceTreeService.GetOrSetSpaceTree(_contextService.UserId);
                    return Json(new UpdateViewModel(ErrorMessages.Success,
                                                    ErrorMessages.GetErrorMessages(ErrorMessages.Success),
                                                    "folder"));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }

            return Json(new UpdateViewModel(ErrorMessages.ReNameFailed,
                ErrorMessages.GetErrorMessages(ErrorMessages.ReNameFailed), "file"));

        }

        /// <summary>
        /// 权限设置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SetVisiblity(string id, string type, int visiblity)
        {
            _logger.InfoFormat("设置权限:{0},{1},{2}", id, type, visiblity);

            try
            {
                if (type == "file")
                {
                    var document = _documentService.SetVisiblity(id, (Visible)visiblity);
                    return Json(new UpdateViewModel(ErrorMessages.Success,
                                                    ErrorMessages.GetErrorMessages(ErrorMessages.Success),
                                                    "file"));

                }

                if (type == "folder")
                {
                    var space = _spaceService.SetVisiblity(id, (Visible)visiblity);
                    _contextService.SpaceTreeHtml = _spaceTreeService.GetOrSetSpaceTree(_contextService.UserId);
                    return Json(new UpdateViewModel(ErrorMessages.Success,
                        ErrorMessages.GetErrorMessages(ErrorMessages.Success),
                        "folder"));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }

            return Json(new UpdateViewModel(ErrorMessages.ReNameFailed,
                ErrorMessages.GetErrorMessages(ErrorMessages.ReNameFailed), "file"));

        }

        /// <summary>
        /// 移动文件夹
        /// </summary>
        /// <param name="id"></param>
        /// <param name="spaceid"></param>
        /// <returns></returns>
         [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Move(string id, string type, string spaceid)
        {
            _logger.InfoFormat("移动文件夹:{0},{1}", id, spaceid);

            try
            {
                if (type == "file")
                {
                    var document = _documentService.Move(id, spaceid);
                    return Json(new UpdateViewModel(ErrorMessages.Success,
                        ErrorMessages.GetErrorMessages(ErrorMessages.Success), "file"));
                }
                 else if (type == "folder")
                 {
                     var space = _spaceService.Move(id, spaceid);
                     _contextService.SpaceTreeHtml = _spaceTreeService.GetOrSetSpaceTree(_contextService.UserId);
                     return Json(new UpdateViewModel(ErrorMessages.Success,
                            ErrorMessages.GetErrorMessages(ErrorMessages.Success),
                            "folder"));
                 }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
            return Json(new UpdateViewModel(ErrorMessages.ReNameFailed,
             ErrorMessages.GetErrorMessages(ErrorMessages.ReNameFailed), "file"));
            
        }

       

        /// <summary>
        /// 还原回收站多个文档
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecoveryList()
        {
            _logger.Info("批量还原");

            var documentIds = new List<string>();
            var spaceIds = new List<string>();
            var ids = _requestProvider["ids"];
            var types = _requestProvider["types"];

            if (ids != null && types != null)
            {
                GetIds(ids, types, ref documentIds, ref spaceIds);
            }

            if (documentIds.Count > 0)
            {
                _documentService.Recovery(documentIds.ToArray());
            }

            if (spaceIds.Count > 0)
            {
                _spaceService.Recovery(spaceIds.ToArray());
                _contextService.SpaceTreeHtml = _spaceTreeService.GetOrSetSpaceTree(_contextService.UserId);
            }

            return Json(ids);
        }

        /// <summary>
        /// 移入回收站单个(理论删除)
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(string id)
        {
            _logger.InfoFormat("移动到回收站 {0}", id);

            var document = _documentService.MoveToTrash(id);
            return Json(document);
        }

        /// <summary>
        /// 移入回收站多个(理论删除)
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DeleteList()
        {
            _logger.Info("批量移动到回收站");

            var documentIds = new List<string>();
            var spaceIds = new List<string>();
            var ids = _requestProvider["ids"];
            var types = _requestProvider["types"];
            
            if (ids != null&& types != null)
            {
                GetIds(ids, types, ref documentIds, ref spaceIds);
            }

            if (documentIds.Count > 0)
            {
                _documentService.MoveToTrash(documentIds.ToArray());    
            }

            if (spaceIds.Count > 0)
            {
                _spaceService.MoveToTrash(spaceIds.ToArray());
                _contextService.SpaceTreeHtml = _spaceTreeService.GetOrSetSpaceTree(_contextService.UserId);
            }

            return Json(ids);
        }

        public ActionResult Download(string Id)
        {
            try
            {
                var document = _documentService.GetDocument(Id);
                string mimeType = MimeMapping.GetMimeMapping(document.FileName);
                byte[] bytes = _storePolicy.GetBytes(document.StorePath);
                return File(bytes, mimeType, document.FileName);
               
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }

            return View("NotFound", new DisplayViewModel());
        }

     

        /// <summary>
        /// 物理删除
        /// ids格式:   001|003|004
        /// types格式:file|file|file
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Clear()
        {
            _logger.Info("清空回收站");

            var ids = _requestProvider["ids"];
            var types = _requestProvider["types"];

            if (ids != null && types != null)
            {
                var documentIds = new List<string>();
                var spaceIds = new List<string>();         
                
               
                GetIds(ids, types, ref documentIds, ref spaceIds);
               
                if (documentIds.Count > 0)
                {
                    _documentService.DeleteAll(documentIds.ToArray());
                }

                if (spaceIds.Count > 0)
                {
                    _spaceService.DeleteAll(spaceIds.ToArray());
                }

                return Json(true);
            }
            else
            {
                _documentService.DeleteAll(_contextService.UserId);
                _spaceService.DeleteAll(_contextService.UserId);
                return Json(true);
            }
        }

        private void GetIds(string ids, string types, ref List<string> documentIds, ref List<string> spaceIds)
        {
            if (ids != null && types != null)
            {
                var idGroup = ids.TrimEnd('|').Split('|');
                var typeGrounp = types.TrimEnd('|').Split('|');

                if (idGroup.Length != typeGrounp.Length)
                {
                    return;
                }

                for (int i = 0; i < typeGrounp.Length; ++i)
                {
                    if (typeGrounp[i] == "file")
                    {
                        documentIds.Add(idGroup[i]);
                    }
                    else if (typeGrounp[i] == "folder")
                    {
                        spaceIds.Add(idGroup[i]);
                    }
                }

            }
        }

        
    }
}