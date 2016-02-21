using System;
using System.IO;
using Common.Logging;
using Infrasturcture.Errors;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Infrasturcture.Converter
{
    /// <summary>
    /// 转换Word为PDF
    /// </summary>
    public class TextToPdfConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        
        public int Convert(string sourcePath, string targetPath)
        {
            if (!File.Exists(sourcePath))
            {
                return ErrorMessages.FileNotExist;
            }

            try
            {
                var content = File.ReadAllText(sourcePath, System.Text.Encoding.Default);
                var content2 = File.ReadAllText(sourcePath, System.Text.Encoding.UTF8);
                Document document = new Document();
                BaseFont bf = BaseFont.CreateFont("c:\\windows\\fonts\\SIMSUN.TTC,1", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
      
                Font font = new Font(bf);

                PdfWriter.GetInstance(document, new FileStream(targetPath, FileMode.Create));
                document.Open();

                document.Add(new Paragraph(content, font));
                document.Close();
                return ErrorMessages.ConvertSuccess;
            }
            catch(Exception ex)
            {
                _logger.Error(ex.StackTrace);
                _logger.Error(ex.Message);
            }

            return ErrorMessages.ConvertFailed;

        }
    }
}
