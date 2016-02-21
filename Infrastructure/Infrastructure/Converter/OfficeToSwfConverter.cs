using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using Infrasturcture.Errors;
using Infrasturcture.Utils;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;

namespace Infrasturcture.Converter
{
    /// <summary>
    /// Office->PDF->SWF
    /// </summary>
    public class OfficeToSwfConverter
    {
        private OfficeToPdfConverter _officeToPdfConverter = new OfficeToPdfConverter();
        private PdfToSwfConverter _pdfToSwfConverter = new PdfToSwfConverter();
     
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public int Convert(string sourcePath, string targetPath)
        {
            _logger.Debug("OfficeToSWF convert, convert " + sourcePath + " to: " + targetPath);

            string pdfPath = StringUtils.RemoveAllEmpty(sourcePath + ".pdf");

            int result = _officeToPdfConverter.Convert(sourcePath, pdfPath);
            if (result != ErrorMessages.ConvertSuccess)
            {
                return result;
            }

            return _pdfToSwfConverter.Convert(pdfPath, targetPath);
        }

       

    }
}
