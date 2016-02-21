using System;
using System.IO;
using Common.Logging;
using Documents.Exceptions;
using Documents.Utils;
using Infrasturcture.Errors;

namespace Documents.Reader.Imp
{
    /// <summary>
    /// 读取Text内容
    /// </summary>
    public class TextReader: IReader
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public string Read(string filePath)
        {
            _logger.Debug("读取文本格式的文件:" + filePath);

            try
            {
                var encoding = EncodingUtils.GetFileEncode(filePath);
                return File.ReadAllText(filePath, encoding);
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
               
                var encoding = EncodingUtils.GetFileEncode(inputStream);
                inputStream.Position = 0;
                StreamReader reader = new StreamReader(inputStream, encoding, true);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new ReadException(ErrorMessages.ReadTextFailed, ex);
            }
        }
    }
}
