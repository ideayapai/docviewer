using System.IO;
using Common.Logging;
using Infrasturcture.Errors;
using Infrasturcture.Utils;

namespace Documents.Converter.Imp
{
    /// <summary>
    /// EXCEL->PDF->SWF
    /// </summary>
    public class ExcelToSwfConverter : IConverter
    {
        private readonly ExcelToPdfConverter _wordToPdfConverter = new ExcelToPdfConverter();
        private readonly PdfToSwfConverter _pdfToSwfConverter = new PdfToSwfConverter();
        
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string from, string to)
        {
            _logger.DebugFormat("转换Excel为SWF, {0} 到 {1}", from, to);

            string pdfPath = StringUtils.RemoveAllEmpty(from + ".pdf");

            int result = _wordToPdfConverter.Convert(from, pdfPath);
            if (result != ErrorMessages.Success)
            {
                return result;
            }

            return _pdfToSwfConverter.Convert(pdfPath, to);
        }

    }
}