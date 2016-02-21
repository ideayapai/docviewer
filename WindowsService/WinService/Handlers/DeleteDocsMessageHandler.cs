using System;
using System.Collections.Generic;
using Common.Logging;
using Infrasturcture.Store;
using Messages;
using Search;
using Services.Contracts;
using Services.Ioc;
using Services.Messages;

namespace DocViewerWinService.Handlers
{
    /// <summary>
    /// 批量删除文档的处理函数
    /// </summary>
    public class DeleteDocsMessageHandler: IMessageHandler<DeleteDocsMessage>
    {
        private readonly ISearchProvider _searchProvider = ServiceActivator.Get<ISearchProvider>();
        private readonly IStorePolicy _storePolicy = ServiceActivator.Get<IStorePolicy>();
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Handle(DeleteDocsMessage message)
        {
            _logger.Info("Handle DeleteDocsMessage");

            try
            {
                _searchProvider.DeleteList(message.Contents);
               
                CleanUpDocuments(message.Contents);

                _logger.Info("Handle DeleteDocsMessage Finished");

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
           
        }

        private void CleanUpDocuments(List<DocumentObject> documents)
        {
            _logger.Info("Clean up Documents");

            foreach (var document in documents)
            {
                try
                {
                    _logger.InfoFormat("Start to delete StorePath:{0}, ConvertPath:{1}", document.StorePath, document.ConvertPath);

                    if (_storePolicy.Exist(document.StorePath))
                    {
                        _logger.InfoFormat("StorePath {0} delete", document.StorePath);
                        _storePolicy.Delete(document.StorePath);
                    }
                    else
                    {
                        _logger.InfoFormat("StorePath {0} is not exist", document.StorePath);
                    }

                    if (System.IO.File.Exists(document.ConvertPath))
                    {
                        _logger.InfoFormat("ConvertPath {0} delete", document.ConvertPath);
                        System.IO.File.Delete(document.ConvertPath);
                    }
                    else
                    {
                        _logger.InfoFormat("ConvertPath {0} is not exist", document.ConvertPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                    _logger.Error(ex.StackTrace);
                }
              
            }
        }
    }
}
