using System;
using System.IO;
using Documents.Converter.Imp;
using Infrasturcture.Errors;
using NUnit.Framework;

namespace Documents.Test.Converter.Imp
{
    [TestFixture]
    public class PdfToHtmlTester
    {
        readonly PdfToHtmlConverter _pdfToHtmlConverter = new PdfToHtmlConverter();

        [Test]
        public void should_convert_from_drm_pdf_to_html()
        {
            //string from = Environment.CurrentDirectory + @"\TestFiles\Guidance_User_Manual_v1.2_cn.pdf";
            //string to = Environment.CurrentDirectory + @"\TestFiles\Guidance_User_Manual_v1.2_cn.pdf.html";
           
            //var result = _pdfToHtmlConverter.Convert(from, to);
            //Assert.AreEqual(ErrorMessages.Success, result);
            //Assert.IsTrue(File.Exists(to)); 
        }

    }
    
}
