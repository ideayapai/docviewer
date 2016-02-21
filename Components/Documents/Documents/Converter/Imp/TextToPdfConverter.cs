using System;
using System.IO;
using System.Text;
using Common.Logging;
using Documents.Exceptions;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Infrasturcture.Errors;

namespace Documents.Converter.Imp
{
    /// <summary>
    /// 转换Word为PDF
    /// </summary>
    public class TextToPdfConverter : IConverter
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        
        public int Convert(string from, string to)
        {
            _logger.DebugFormat("Text转换为Pdf, {0},到:{1}", from, to);

            try
            {
                var content = File.ReadAllText(@from, Encoding.Default);
               
                Document document = new Document();
                BaseFont bf = BaseFont.CreateFont("c:\\windows\\fonts\\SIMSUN.TTC,1", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
      
                Font font = new Font(bf);
                
                PdfWriter.GetInstance(document, new FileStream(@to, FileMode.Create));
                document.Open();

                document.Add(new Paragraph(content, font));
                document.Close();

                return ErrorMessages.Success;
            }
            catch(Exception ex)
            {
                throw new ConverterException(ErrorMessages.TextToPdfFailed, ex);
            }

        }

        public int Convert(Stream @from, Stream to)
        {
            throw new NotImplementedException();
        }
    }
}
