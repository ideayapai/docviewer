using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using Common.Logging;
using Documents;
using Documents.Converter;
using Documents.Enums;
using Documents.Utils;
using Infrasturcture.Errors;
using Infrasturcture.Store;
using Infrasturcture.Utils;
using Services.Contracts;
using Services.Models;

namespace Services.Documents
{
    public class DocumentConvertService
    {
        private readonly DocumenConverter _documentConverter;
        private readonly IStorePolicy _storePolicy;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public DocumentConvertService(DocumenConverter documentConverter, IStorePolicy storePolicy)
        {
            _documentConverter = documentConverter;
            _storePolicy = storePolicy;
        }

        public bool CanConvert(DocumentObject document)
        {
           
            //不是Office文档，不需要转换
            if (document.DocumentCategory != DocumentCategory.Office &&
                document.DocumentCategory != DocumentCategory.CAD &&
                document.DocumentCategory != DocumentCategory.Text)
            {
                return false;
            }

            // 转换后的路径不存在，需要转换
            if (string.IsNullOrEmpty(document.ConvertPath) || !File.Exists(document.ConvertPath))
            {
                return true;
            }

            return false;
            
        }

        public ConvertResult Convert(ref DocumentObject document, ConvertFileType convertType)
        {
            try
            {
                string path = DocumentSettings.GetStorePath(document.FileName);
                _storePolicy.Copy(path, document.StorePath);

                string tmpName = document.Id + convertType.ToSuffix();
                var convertPath = DocumentSettings.GetConvertPath(tmpName);
                int result = _documentConverter.Convert(path, convertPath, convertType);

                if (File.Exists(tmpName))
                {
                    File.Delete(tmpName);
                }

                return new ConvertResult{
                                            ErrorCode = result,
                                            SourcePath = document.StorePath,
                                            TargetPath = convertPath
                                        };

                
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw;
            }
           
        }
    }
}
