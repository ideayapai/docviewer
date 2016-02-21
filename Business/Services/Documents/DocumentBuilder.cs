using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Common.Logging;
using Documents;
using Documents.Enums;
using Documents.Utils;
using Infrasturcture.Store;
using Infrasturcture.Store.Files;
using Infrasturcture.Utils;
using Infrasturcture.Web;
using Krystalware.SlickUpload;
using Services.Contracts;
using Services.Enums;

namespace Services.Documents
{
    public class DocumentBuilder
    {
       
        private static readonly ILog _logger = LogManager.GetCurrentClassLogger();
     

      

        public static DocumentObject Build(BaseFile fileData, 
                                           string spaceId, 
                                           string spaceSeqNo,
                                           string spaceName, 
                                           string userId, 
                                           string userName,
                                           string depId,
                                           Visible visible)
                {
            var fileName = fileData.FileName;

            _logger.Debug("存储原始文件...");
            var storePath = DocumentSettings.GetStorePath(fileName);

            _logger.Debug("存储原始文件完成");
            var document = new DocumentObject
            {
                Id = fileData.Id,
                FileName = fileData.FileName,
                StorePath = storePath,
                FileSize = fileData.ContentLength,
                CreateTime = DateTime.Now,
                DocumentType = fileName.ToDocumentType(),
                UpdateTime = DateTime.Now,
                CreateUserId = userId,
                CreateUserName = userName,
                SpaceId = spaceId,
                SpaceSeqNo = spaceSeqNo,
                SpaceName = spaceName,
                UpdateUserId = userId,
                UpdateUserName = userName,
                IsConvert = false,
                Visible = (int)visible,
                DepId = depId,
            };


            document.DisplayPath = DocumentSettings.GetDisplayUrl(Path.GetFileName(document.ConvertPath));
           
           
            return document;
        }

        public static DocumentObject Build(UploadedFile fileData,
                                           string spaceId,
                                           string spaceSeqNo,
                                           string spaceName,
                                           string userId,
                                           string userName,
                                           string depId,
                                           Visible visible)
        {
            var fileName = fileData.ClientName;

            _logger.Debug("存储原始文件...");
          

            Guid fileId = Guid.NewGuid();
            _logger.Debug("存储原始文件完成");
            var document = new DocumentObject
            {
                Id = fileId,
                FileName = fileName,
                StorePath = fileData.ServerLocation,
                FileSize = fileData.ContentLength,
                CreateTime = DateTime.Now,
                DocumentType = fileName.ToDocumentType(),
                UpdateTime = DateTime.Now,
                CreateUserId = userId,
                CreateUserName = userName,
                SpaceId = spaceId,
                SpaceSeqNo = spaceSeqNo,
                SpaceName = spaceName,
                UpdateUserId = userId,
                UpdateUserName = userName,
                IsConvert = false,
                Visible = (int)visible,
                DepId = depId,
            };


            document.DisplayPath = DocumentSettings.GetDisplayUrl(Path.GetFileName(document.ConvertPath));


            return document;
        }
    }
}
