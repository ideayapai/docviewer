using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Logging;
using Documents;
using Documents.Enums;
using ImageStore.Services;
using Infrasturcture.DomainObjects;
using Infrasturcture.Errors;
using Infrasturcture.Store;
using Messages;
using Repository;
using Services.CacheService;
using Services.Contracts;
using Services.Enums;
using Services.Messages;

namespace Services.Documents
{
    /// <summary>
    /// 文档服务类
    /// </summary>
    public class DocumentService
    {
        private readonly IBaseRepository<Document> _documentRepository;
        private readonly DocumentCacheService _cacheService;
        private readonly DocumentConvertService _documentConvertService;
        private readonly IStorePolicy _storePolicy;
        private readonly MessageBus _bus;
        private readonly ImageService _imageService;

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public DocumentService(IBaseRepository<Document> documentRepository, 
                               DocumentCacheService cacheService,
                               DocumentConvertService documentConvertService, 
                               MessageBus bus,
                               ImageService imageService,
                               IStorePolicy storePolicy)
        {
            _documentRepository = documentRepository;
            _cacheService = cacheService;
            _documentConvertService = documentConvertService;
            _bus = bus;
            _imageService = imageService;
            _storePolicy = storePolicy;
        }

 
        

        /// <summary>
        /// 添加文档
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public DocumentObject Add(DocumentObject document)
        {
            _logger.Info("DocumentService 添加文档");

            GeneratePreviewImage(document);

            document =  _documentRepository.Add(document.ToEntity<Document>()).ToObject<DocumentObject>();
            if (document != null)
            {
                _cacheService.Add(document);
                _bus.Send(new CreateDocumentMessage { Content = document });
            }
            _logger.Debug("写入数据库成功");

            return document;
        }

        /// <summary>
        /// 批量添加文档
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        public List<DocumentObject> Add(List<DocumentObject> documents)
        {
            _logger.Info("DocumentService 批量添加文档");

            foreach (var document in documents)
            {
                GeneratePreviewImage(document);
            }

            _logger.Debug("正在批量写入数据库");
            var entities = documents.Select(dto => dto.ToEntity<Document>()).ToList();
            documents = _documentRepository.Add(entities).ConvertAll(f=>f.ToObject<DocumentObject>());
            if (documents.Count > 0)
            {
                _logger.Debug("正在批量写入数据库");
                _cacheService.Add(documents);
                _bus.Send(new CreateDocsMessage { Contents = documents });
            }

            _logger.Debug("写入批量数据库成功");

            return documents;
        }

        private void GeneratePreviewImage(DocumentObject document)
        {
            //图片需要可以预览
            if (document.DocumentCategory == DocumentCategory.Image)
            {
                _logger.Info("文档为图片,进行图片生成操作");

                byte[] bytes = _storePolicy.GetBytes(document.StorePath);
                Stream stream = new MemoryStream(bytes);
                var result = _imageService.Upload(stream, document.FileName);
                document.ThumbUrl = result.ThumbImageUrl;
                document.DisplayPath = result.CompressImageUrl;
                //document.DownloadPath = result.ImageUrl;

                _logger.InfoFormat("图片地址,ThumUrl:{0},DisplayPath:{1},DownloadPath:{2}", document.ThumbUrl, document.DisplayPath,
                    document.DownloadPath);
            }

            //CAD图也可以预览
            if (document.DocumentCategory == DocumentCategory.CAD)
            {
                var convertResult = _documentConvertService.Convert(ref document, ConvertFileType.CadToJpg);
                var result = _imageService.Upload(convertResult.TargetPath);
                document.ThumbUrl = result.ThumbImageUrl;
                convertResult = _documentConvertService.Convert(ref document, ConvertFileType.CadToSvg);
                document.DisplayPath = DocumentSettings.GetDisplayUrl(Path.GetFileName(document.ConvertPath));
            }
        }

        /// <summary>
        /// 更新某个文档
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public DocumentObject Update(DocumentObject document)
        {
            _logger.Info("DocumentService 更新文档");

            document = _documentRepository.Update(f => f.Id == document.Id,document.ToEntity<Document>()).ToObject<DocumentObject>();

            if (document != null)
            {
                _cacheService.Update(document);
                _bus.Send(new UpdateDocumentMessage { Content = document });
            }
           
            return document;
        }

        /// <summary>
        /// 重命名
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public DocumentObject ReName(string id, string name)
        {
            _logger.Info("DocumentService 重命名");

            var document = GetDocument(id);
            if (document != null)
            {
                document.FileName = name;
                return Update(document);
            }

            throw new Exception(ErrorMessages.GetErrorMessages(ErrorMessages.FileNotExist));
        }

        /// <summary>
        /// 设置权限
        /// </summary>
        /// <param name="id"></param>
        /// <param name="visible"></param>
        /// <returns></returns>
        //   public enum Visible
        //   {
        //      Public,0
        //      Dep,1
        //      Private,2
        //   }
        public DocumentObject SetVisiblity(string id, Visible visible)
        {
            _logger.Info("DocumentService 设置权限");

            var document = GetDocument(id);
            if (document != null)
            {
                document.Visible = (int)visible;
                return Update(document);
            }
            throw new Exception(ErrorMessages.GetErrorMessages(ErrorMessages.FileNotExist));
        }


        /// <summary>
        /// 转换某个文档
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public DocumentObject Convert(DocumentObject document)
        {
            _logger.Info("DocumentService 转换文档:");

            if (_documentConvertService.CanConvert(document))
            {
                var result = _documentConvertService.Convert(ref document, document.DocumentType.ToConvertType());
                _logger.Info(ErrorMessages.GetErrorMessages(result.ErrorCode));
            }

            return document;
        }

        /// <summary>
        /// 移动某个文档
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="spaceId"></param>
        /// <returns></returns>
        public DocumentObject Move(string documentId, string spaceId)
        {
            var document = GetDocument(documentId);
            if (document != null)
            {
                document.SpaceId = spaceId;
                return Update(document);
            }

            throw new Exception(ErrorMessages.GetErrorMessages(ErrorMessages.DocumentExist));
        }

        /// <summary>
        /// 移动单个文档到回收站
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public DocumentObject MoveToTrash(string Id)
        {
            _logger.Info("DocumentService 移动到回收站:" + Id);

            var document = _documentRepository.Update(
                f => f.Id == Guid.Parse(Id), f=>f.IsDelete = true).ToObject<DocumentObject>();
            if (document != null)
            {
                _cacheService.Update(document);
                _bus.Send(new TrashDocumentMessage { Content = document });
            }
           
            return document;
        }

        /// <summary>
        /// 批量移动文档到回收站
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<DocumentObject> MoveToTrash(string[] ids)
        {
            _logger.Info("DocumentService 批量移动到回收站:");

            var documents = _documentRepository.UpdateAll(
                f => ids.Contains(f.Id.ToString()), f => f.IsDelete = true).ConvertAll(f=>f.ToObject<DocumentObject>());

            if (documents.Count > 0)
            {
                _cacheService.Update(documents);
                _bus.Send(new TrashDocsMessage { Contents = new List<DocumentObject>(documents) });    
            }

            return documents;
        }


        /// <summary>
        /// 还原某个文档
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public DocumentObject Recovery(string Id)
        {
            _logger.Info("DocumentService 还原文档Id:" + Id);

            var document = _documentRepository.Update(f => f.Id == Guid.Parse(Id), f => f.IsDelete = false).ToObject<DocumentObject>();

            if (document != null)
            {
                _cacheService.Update(document);
                _bus.Send(new RecoveryDocumentMessage { Content = document });
            }
            
            return document;
        }


        /// <summary>
        /// 批量还原文档
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<DocumentObject> Recovery(string[] ids)
        {
            _logger.Info("DocumentService 批量还原文档:");

            var documents = _documentRepository.UpdateAll(f => ids.Contains(f.Id.ToString()), f => f.IsDelete = false).ConvertAll(f => f.ToObject<DocumentObject>());
            if (documents.Count > 0)
            {
                _cacheService.Update(documents);
                _bus.Send(new RecoveryDocsMessage { Contents = new List<DocumentObject>(documents) });
            }
        
            
            return documents;
        }

        /// <summary>
        /// 清空某个文件
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool Delete(string Id)
        {
            _logger.Info("DocumentService 彻底删除文档:" + Id);

            var result = _documentRepository.Delete(f=> f.Id == Guid.Parse(Id));
            if (result)
            {
                _cacheService.Delete(Id);    
            }
            return result;
        }

        /// <summary>
        /// 清空回收站
        /// </summary>
        /// <returns></returns>
        public bool DeleteAll(string[] ids)
        {
            _logger.Info("DocumentService 批量删除Id:");

            var documents = GetTrashDocuments(ids);
            var result = _documentRepository.Delete(f => ids.Contains(f.Id.ToString()));
            if (result)
            {
                _cacheService.Delete(ids);
                _bus.Send(new DeleteDocsMessage { Contents = documents });
            }
            return result;
        }

        /// <summary>
        /// 清空某个用户的所有回收站
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool DeleteAll(string userId)
        {
            _logger.Info("DocumentService 清空回收站 userId:" + userId);

            var documents = GetTrashDocuments(userId);
            var result = _documentRepository.Delete(f => f.CreateUserId == userId && f.IsDelete);
            if (result)
            {
                foreach (var document in documents)
                {
                    _cacheService.Delete(document.Id.ToString());
                }

                _bus.Send(new DeleteDocsMessage{ Contents = documents });
                
            }
            return result;
        }

        /// <summary>
        /// 获取所有文档
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<DocumentObject> GetDocuments(Predicate<DocumentObject> predicate)
        {
            return GetAllDocuments().FindAll(predicate).OrderByDescending(f => f.UpdateTime).ToList();
        }

        /// <summary>
        /// 获取所有未删除的所有文档
        /// </summary>
        /// <returns></returns>
        public List<DocumentObject> GetAllUnDeleteDocuments()
        {
            return GetAllDocuments().FindAll(f => !f.IsDelete).ToList();
        }

        /// <summary>
        /// 获取所有未删除文档
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<DocumentObject> GetUnDeleteDocuments(Predicate<DocumentObject> predicate)
        {
            return GetAllUnDeleteDocuments().FindAll(predicate).OrderByDescending(f => f.UpdateTime).ToList();
        }
        
        /// <summary>
        /// 返回该用户的文档和公有文档
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<DocumentObject> GetVisibleDocuments(string userId)
        {
            return GetAllUnDeleteDocuments()
                    .FindAll(f => f.CreateUserId == userId || f.Visible == (int)Visible.Public)
                    .OrderByDescending(f => f.UpdateTime).ToList();
        }

        /// <summary>
        /// 返回该用户的文档和公有文档
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<DocumentObject> GetPrivateDocuments(string userId)
        {
            return GetAllUnDeleteDocuments()
                    .FindAll(f => f.CreateUserId == userId)
                    .OrderByDescending(f => f.UpdateTime).ToList();
        }

        /// <summary>
        /// 返回该用户的私人文档
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<DocumentObject> GetVisibleDocuments(string userId, Predicate<DocumentObject> predicate)
        {
            return GetAllUnDeleteDocuments()
                    .FindAll(f => f.CreateUserId == userId)
                    .FindAll(predicate)
                    .OrderByDescending(f => f.UpdateTime).ToList();
        }

        /// <summary>
        /// 返回该用户，同部门或者公开的所有文档
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="depId"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<DocumentObject> GetVisibleDocuments(string userId, string depId)
        {
            return GetAllUnDeleteDocuments()
                    .FindAll(f => f.CreateUserId == userId || (f.DepId == depId && f.Visible == (int)Visible.Dep) || f.Visible == (int)Visible.Public)
                    .OrderByDescending(f => f.UpdateTime).ToList();
        }


        /// <summary>
        /// 返回该用户，同部门或者公开的所有文档
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="depId"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public List<DocumentObject> GetVisibleDocuments(string userId, string depId, Predicate<DocumentObject> predicate)
        {
            return GetAllUnDeleteDocuments()
                    .FindAll(f => f.CreateUserId == userId || (f.DepId == depId && f.Visible == (int)Visible.Dep) || f.Visible == (int)Visible.Public)
                    .FindAll(predicate)
                    .OrderByDescending(f => f.UpdateTime).ToList();
        }


        /// <summary>
        /// 获取所有未删除并且文档存在的索引
        /// </summary>
        /// <returns></returns>
        public List<DocumentObject> GetExistsDocuments()
        {
            return GetAllUnDeleteDocuments().OrderByDescending(f => f.UpdateTime).ToList();
        }

        public List<DocumentObject> GetTrashDocuments(string[] documentIds)
        {
            var trashDocuments = GetAllTrashDocuments();
            return documentIds.Select(documentId => trashDocuments.Find(f => f.Id.ToString() == documentId)).ToList();
        }

        /// <summary>
        /// 获取回收站的文档
        /// </summary>
        /// <returns></returns>
        public List<DocumentObject> GetTrashDocuments(string userId)
        {
            return GetAllTrashDocuments().FindAll(f => f.CreateUserId == userId).OrderByDescending(f => f.UpdateTime).ToList();
        }

        /// <summary>
        /// 获取所有回收站的文档
        /// </summary>
        /// <returns></returns>
        public List<DocumentObject> GetAllTrashDocuments()
        {
            return GetAllDocuments().FindAll(f => f.IsDelete).OrderByDescending(f => f.UpdateTime).ToList();
        }

        /// <summary>
        /// 获取所有文档
        /// </summary>
        /// <returns></returns>
        public List<DocumentObject> GetAllDocuments()
        {
            return _cacheService.GetOrAdd(() => _documentRepository.GetAllByExp(f => true)
                             .ToList().ConvertAll(f => f.ToObject<DocumentObject>()));
        }

        /// <summary>
        /// 获取单个文档
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public DocumentObject GetDocument(string Id)
        {
            return _cacheService.GetOrAdd(Id, 
                ()=>_documentRepository.Get(f => f.Id == Guid.Parse(Id)).ToObject<DocumentObject>());
        }


       

    }
}