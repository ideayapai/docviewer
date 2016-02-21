using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Common.Logging;
using Documents;
using Documents.Enums;
using Infrasturcture.DomainObjects;
using Infrasturcture.Store;
using Infrasturcture.Web;
using Krystalware.SlickUpload;
using Services.CacheService;
using Services.Contracts;
using Services.Documents;
using Services.Enums;
using Services.Models;
using Services.Spaces;
using WebAPI2.Models;

namespace WebAPI2.Controllers
{
    /// <summary>
    /// 文档管理API
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DocumentController : ApiController
    {
        private readonly DocumentService _documentService;
        private readonly SpaceService _spaceService;
        private readonly IHttpRequestProvider _requestProvider;
        private readonly IStorePolicy _storePolicy;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// DocumentController
        /// </summary>
        /// <param name="documentService"></param>
        /// <param name="spaceService"></param>
        public DocumentController(DocumentService documentService, 
                                  SpaceService spaceService, 
                                  IHttpRequestProvider requestProvider,
                                  IStorePolicy storePolicy)
        {
      
            _documentService = documentService;
            _spaceService = spaceService;
            _requestProvider = requestProvider;
            _storePolicy = storePolicy;

        }

        
        /// <summary>
        /// path数据结构
        /// 001_西山区|4567_虹桥立交|8889_日常巡检 解析为三个文件夹
        /// 西山区(001)
        ///   |
        ///   |---虹桥立交(4567)
        ///         |
        ///         |----日常巡检(8889)
        /// </summary>
        [System.Web.Http.HttpPost]
        public DocumentContract Add()
        {
            _logger.Info("DocumentController 添加文档.");

            var userId = _requestProvider["UserId"];
            var spaceId = _requestProvider["SpaceId"];
            var userName = _requestProvider["UserName"];
            var depId = _requestProvider["DepId"];
            var visible = _requestProvider["Visible"];
            var path = _requestProvider["Path"];

            var timeStamp = _requestProvider["TimeStamp"];

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
                space = string.IsNullOrWhiteSpace(spaceId) ? _spaceService.GetDefaultSpace() : _spaceService.GetSpace(spaceId);
            }
            else
            {
                _logger.InfoFormat("文档参数path:{0}", path);
                space = _spaceService.MakeSpace(string.Empty, path, userId, userName, depId, visiblity);
            }

            var fileData = _requestProvider.FileCollection[0];
            if (fileData == null)
            {
                _logger.Error("文档参数错误, fileData ContentLength为0.");
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            _logger.Info(string.Format("Add Document, spaceId:[{0}]；userId:[{1}]；userName:[{2}]。", spaceId, userId, userName));

            try
            {
                var document = DocumentBuilder.Build(fileData,
                                                       space.Id.ToString(),
                                                       space.SpaceSeqNo,
                                                       space.SpaceName,
                                                       userId,
                                                       userName,
                                                       depId,
                                                       visiblity);
                var mimeType = MimeMapping.GetMimeMapping(document.FileName);
                _storePolicy.AddStream(fileData.FileStream, mimeType, document.StorePath);

                var documentObj = _documentService.Add(document);
                if (documentObj.DocumentCategory != DocumentCategory.Image)
                {
                    documentObj.DisplayPath = documentObj.PreviewUrl;                    
                }

                var contract = documentObj.ToObject<DocumentContract>();

                if (!string.IsNullOrWhiteSpace(timeStamp))
                {
                    MemoryContainer.Push(timeStamp, contract);
                }

                return contract;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
            
        }


        /// <summary>
        /// Form方式上传文件提交之后根据时间戳获取文件
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public DocumentContract GetTemp(string timeStamp)
        {
            var document = MemoryContainer.Pop(timeStamp) as DocumentContract;
            if (document != null)
            {
                return document;
            }
            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        /// <summary>
        /// 返回该目录下某用户的所有文件
        /// </summary>
        /// <param name="spaceId">目录Id</param>
        /// <param name="userId">用户Id</param>
        /// <returns>文档列表</returns>
        [System.Web.Http.HttpGet]
        public IEnumerable<DocumentContract> GetDocuments(string spaceId, string userId)
        {
            _logger.InfoFormat("In GetDocuments spaceId:{0}, userId:{1}", spaceId, userId);

            try
            {
                var documents = _documentService.GetVisibleDocuments(userId, f => f.SpaceId == spaceId);
                return documents.ConvertAll(f => f.ToObject<DocumentContract>());
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
           
        }

        /// <summary>
        /// 返回目录下某用户某种类型的文件
        /// </summary>
        /// <param name="spaceId">目录Id</param>
        /// <param name="userId">用户Id</param>
        /// <param name="documentType">文档类型</param>
        /// <returns>文档列表</returns>
        [System.Web.Http.HttpGet]
        public IEnumerable<DocumentContract> GetDocuments(string spaceId, string userId, DocumentType documentType)
        {
            _logger.InfoFormat("GetDocuments spaceId:{0}, userId:{1}, DocumentType:{2}", spaceId, userId, documentType);
            
            try
            {
                var documents = _documentService.GetVisibleDocuments(userId, f=>f.SpaceId == spaceId && f.DocumentType == documentType);
                return documents.ConvertAll(f => f.ToObject<DocumentContract>());
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

       
        /// <summary>
        /// 返回某种类型的文档
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="documentType">文档类型</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public IEnumerable<DocumentContract> GetDocuments(string userId, DocumentType documentType)
        {
            _logger.InfoFormat("GetDocuments documentType:{0}", documentType);
            
            try
            {
                var documents = _documentService.GetVisibleDocuments(userId, f=>f.DocumentType == documentType);
                return documents.ConvertAll(f=>f.ToObject<DocumentContract>());
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// 根据文档Id返回一个文档对象
        /// </summary>
        /// <param name="id">文档的Id</param>
        /// <returns>成功返回文档对象,如果没有找到，则返回NotFound状态码</returns>
        [System.Web.Http.HttpGet]
        public DocumentContract Get(string id)
        {
            _logger.InfoFormat("Document Get id:{0}", id);

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            try
            {
                var document = _documentService.GetDocument(id);
                if (document == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                return document.ToObject<DocumentContract>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }
        
        /// <summary>
        /// 移除某个文档到回收站中
        /// </summary>
        /// <param name="fileId">文档Id</param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public DocumentContract Recover(string fileId)
        {
            _logger.InfoFormat("Document Recover fileId:{0}", fileId);

            if (string.IsNullOrWhiteSpace(fileId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            try
            {
                var document = _documentService.MoveToTrash(fileId);
                if (document == null)
                {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
                return document.ToObject<DocumentContract>();
            }
            catch (Exception ex)
            {

                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// 删除某个文档
        /// </summary>
        /// <param name="fileId">文件Id</param>
        /// <returns>返回True表示成功，返回False表示失败</returns>
        [System.Web.Http.HttpGet]
        public bool Delete(string fileId)
        {
            _logger.InfoFormat("Enter Document Delete fileId:{0}", fileId);

            if (string.IsNullOrWhiteSpace(fileId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            try
            {
                return _documentService.Delete(fileId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// 返回下载文件
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage Download(string Id)
        {
            try
            {
                
                var document = _documentService.GetDocument(Id);
                string mimeType = MimeMapping.GetMimeMapping(document.FileName);
                byte[] bytes = _storePolicy.GetBytes(document.StorePath);

                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(bytes)
                };

                string userAgent = _requestProvider.UserAgent;
                string fileName = document.FileName;

                //如果是IE浏览器
                if (userAgent.Contains("MSIE") || userAgent.Contains("Edge"))
                {
                    fileName = HttpUtility.UrlEncode(fileName);
                }
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileName
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
                return result;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            
        }

      

       
    }
}
