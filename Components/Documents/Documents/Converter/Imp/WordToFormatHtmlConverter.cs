using System.IO;
using Common.Logging;
using Infrasturcture.Errors;
using Infrasturcture.Utils;

namespace Documents.Converter.Imp
{
    /// <summary>
    /// Office->PDF->SWF
    /// </summary>
    public class WordToFormatHtmlConverter : IConverter
    {
        private readonly WordToPdfConverter _wordToPdfConverter = new WordToPdfConverter();
        private readonly PdfToHtmlConverter _pdfToSwfConverter = new PdfToHtmlConverter();
        
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string from, string to)
        {
            _logger.DebugFormat("转换Word为SWF, {0} 到 {1}", from, to);

            string pdfPath = StringUtils.RemoveAllEmpty(from + ".pdf");

            int result = _wordToPdfConverter.Convert(from, pdfPath);
            if (result != ErrorMessages.Success)
            {
                return result;
            }

            return _pdfToSwfConverter.Convert(pdfPath, to);
        }

        public int Convert(Stream @from, Stream to)
        {
            throw new System.NotImplementedException();
        }
    }
}