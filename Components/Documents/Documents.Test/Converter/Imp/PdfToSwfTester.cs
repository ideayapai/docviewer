using System;
using System.IO;
using Documents.Converter.Imp;
using Infrasturcture.Errors;
using NUnit.Framework;

namespace Documents.Test.Converter.Imp
{
    [TestFixture]
    public class PdfToSwfTester
    {
        private readonly PdfToSwfConverter _pdfToSwfConverter = new PdfToSwfConverter();

        [Test]
        public void should_convert_from_pdf_to_swf()
        {
            Assert.AreEqual(ErrorMessages.Success, _pdfToSwfConverter.Convert(Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.pdf",
                                          Environment.CurrentDirectory + @"\TestFiles\1983_08_29_00_55智慧市政综合指挥平台pdf.swf"));
            Assert.IsTrue(File.Exists(Environment.CurrentDirectory + @"\TestFiles\1983_08_29_00_55智慧市政综合指挥平台pdf.swf")); 
        }

        [Test]
        public void should_not_convert_from_pdf_to_swf_if_file_doesntexists()
        {
            Assert.AreEqual(ErrorMessages.FileNotExist, _pdfToSwfConverter.Convert(Environment.CurrentDirectory + @"\TestFiles\filennotexist.pdf",
                                           Environment.CurrentDirectory + @"\TestFiles\filennotexist.swf"));
            Assert.IsTrue(!File.Exists(Environment.CurrentDirectory + @"\TestFiles\filennotexist.swf"));
        }
    }
    
}
