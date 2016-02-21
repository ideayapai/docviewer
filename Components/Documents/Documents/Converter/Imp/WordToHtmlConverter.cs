using System;
using System.IO;
using Aspose.Words;
using Aspose.Words.Saving;
using Common.Logging;
using Documents.Exceptions;
using Infrasturcture.Errors;

namespace Documents.Converter.Imp
{
    /// <summary>
    /// 转换Word为HTML
    /// </summary>
    public class WordToHtmlConverter : IConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string from, string to)
        {
            _logger.DebugFormat("Word转换为HTML, sourcePath:{0},targetPath:{1}", from, to);

            try
            {
                var doc = new Document(from);
                doc.Save(to, SaveFormat.Html);
                return ErrorMessages.Success;
            }
           
            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.WordToHtmlFailed, ex);
            }
            
        }

        public int Convert(Stream @from, Stream to)
        {
            _logger.DebugFormat("Word转换为HTML");

            try
            {
                var doc = new Document(from);
                doc.Save(to, SaveFormat.Html);
                return ErrorMessages.Success;
            }

            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.WordToHtmlFailed, ex);
            }
        }
    }
}
