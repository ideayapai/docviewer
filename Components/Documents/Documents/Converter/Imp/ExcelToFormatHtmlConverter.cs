using System.IO;
using Common.Logging;
using Infrasturcture.Errors;
using Infrasturcture.Utils;

namespace Documents.Converter.Imp
{
    /// <summary>
    /// EXCEL->PDF->SWF
    /// </summary>
    public class ExcelToFormatHtmlConverter : IConverter
    {
        private readonly ExcelToPdfConverter _wordToPdfConverter = new ExcelToPdfConverter();
        private readonly PdfToHtmlConverter _pdfToSwfConverter = new PdfToHtmlConverter();
        
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string from, string to)
        {
            _logger.DebugFormat("转换Excel为Format, {0} 到 {1}", from, to);

            string pdfPath = StringUtils.RemoveAllEmpty(from + ".pdf");

            int result = _wordToPdfConverter.Convert(from, pdfPath);
            if (result != ErrorMessages.Success)
            {
                return result;
            }

            result = _pdfToSwfConverter.Convert(pdfPath, to);
            if (File.Exists(pdfPath))
            {
                File.Delete(pdfPath);
            }
            return result;
        }

        public int Convert(Stream @from, Stream to)
        {
            throw new System.NotImplementedException();
        }
    }
}