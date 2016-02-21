//using Microsoft.Office.Interop.Excel;

using System;
using System.IO;
using System.Text.RegularExpressions;
using Aspose.Pdf;
using Aspose.Pdf.Text;
using Common.Logging;
using Documents.Exceptions;
using Infrasturcture.Errors;

namespace Documents.Reader.Imp
{
    /// <summary>
    /// 读取PDF内容
    /// </summary>
    public class PDFReader : IReader
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public string Read(string filePath)
        {
            _logger.Debug("读取 PDF 文件:" + filePath);

            try
            {
                Document pdfDocument = new Document(filePath);
                return GetContent(pdfDocument);
            }
            catch (Exception ex)
            {
                throw new ReadException(ErrorMessages.ReadExcelFailed, ex);
            }
        }

        public string Read(Stream inputStream)
        {
            try
            {
                Document pdfDocument = new Document(inputStream);
                return GetContent(pdfDocument);
            }
            catch (Exception ex)
            {
                throw new ReadException(ErrorMessages.ReadExcelFailed, ex);
            }
        }

        private static string GetContent(Document pdfDocument)
        {
            TextAbsorber textAbsorber = new TextAbsorber();

            //accept the absorber for all the pages
            pdfDocument.Pages.Accept(textAbsorber);

            //get the extracted text
            var content = textAbsorber.Text;
            return Regex.Replace(content, @"\s", "");
        }
    }
}