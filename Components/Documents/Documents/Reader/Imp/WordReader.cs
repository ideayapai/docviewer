using System;
using System.IO;
using Aspose.Words;
using Common.Logging;
using Documents.Exceptions;
using Infrasturcture.Errors;

namespace Documents.Reader.Imp
{
    /// <summary>
    /// 读取Word内容(包含RTF格式)
    /// </summary>
    public class WordReader: IReader
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
       
        public string Read(string filePath)
        {
            _logger.Debug("读取WordFile:" + filePath);

            try
            {
                var document = new Document(filePath);
                return document.GetText();
            }
            catch (Exception ex)
            {
                throw new ReadException(ErrorMessages.ReadTextFailed, ex);
            }
          
        }

        public string Read(Stream inputStream)
        {
            try
            {
                var document = new Document(inputStream);
                return document.GetText();
            }
            catch (Exception ex)
            {
                throw new ReadException(ErrorMessages.ReadTextFailed, ex);
            }
        }
    }
}
