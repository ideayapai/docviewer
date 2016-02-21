using System;
using System.IO;
using Aspose.Words;
using Common.Logging;
using Documents.Exceptions;
using Infrasturcture.Errors;

namespace Documents.Converter.Imp
{
    /// <summary>
    /// 转换Word为PDF
    /// </summary>
    public class WordToPdfConverter : IConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string from, string to)
        {
            _logger.DebugFormat("Word转换为Pdf, {0} 到 {1}", from, to);

            try
            {
                var doc = new Document(from);
                doc.Save(to, SaveFormat.Pdf);
                return ErrorMessages.Success;
            }
            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.WordToPdfFailed, ex);
            }
           
        }

        public int Convert(Stream @from, Stream to)
        {
            _logger.DebugFormat("Word转换为Pdf, {0} 到 {1}", from, to);

            try
            {
                var doc = new Document(from);
                doc.Save(to, SaveFormat.Pdf);
                return ErrorMessages.Success;
            }
            catch (Exception ex)
            {
                throw new ConverterException(ErrorMessages.WordToPdfFailed, ex);
            }
        }
    }
}
