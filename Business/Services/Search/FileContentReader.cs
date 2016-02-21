using System;
using System.IO;
using Common.Logging;
using Documents.Enums;
using Documents.Reader;
using Infrasturcture.Store;
using Search;

namespace Services.Search
{
    public class FileContentReader: IFileContentReader
    {
        private readonly DocumentReader _reader = new DocumentReader();
        private readonly IStorePolicy _storePolicy;
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public FileContentReader(IStorePolicy storePolicy)
        {
            _storePolicy = storePolicy;
        }

        /// <summary>
        /// 这里是否需要try catch呢
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string Read(string fileName)
        {
            _logger.InfoFormat("读取文档 {0} 的内容", fileName);

            try
            {
                if (!_storePolicy.Exist(fileName))
                {
                    _logger.ErrorFormat("文档 {0} 不存在", fileName);
                    return string.Empty;
                }

                var bytes = _storePolicy.GetBytes(fileName);
                using (Stream stream = new MemoryStream(bytes))
                {
                    return _reader.Read(stream, fileName.ToDocumentType());
                }   
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }

            return string.Empty;
        }
    }
}
