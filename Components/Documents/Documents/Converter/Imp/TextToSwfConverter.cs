using System;
using System.IO;
using System.Text;
using Common.Logging;
using Documents.Utils;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Infrasturcture.Errors;
using StringUtils = Infrasturcture.Utils.StringUtils;

namespace Documents.Converter.Imp
{
    /// <summary>
    /// Text->PDF->SWF
    /// </summary>
    public class TextToSwfConverter : IConverter
    {
        private readonly TextToPdfConverter _wordToPdfConverter = new TextToPdfConverter();
        private readonly PdfToHtmlConverter _pdfToSwfConverter = new PdfToHtmlConverter();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string from, string to)
        {
            _logger.DebugFormat("转换Text为SWF, {0} 到 {1}", from, to);

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
            throw new NotImplementedException();
        }
    }
}
