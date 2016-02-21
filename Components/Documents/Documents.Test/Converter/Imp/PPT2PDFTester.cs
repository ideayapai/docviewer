using System;
using System.IO;
using Documents.Converter.Imp;
using Documents.Exceptions;
using Infrasturcture.Errors;
using NUnit.Framework;

namespace Documents.Test.Converter.Imp
{
    [TestFixture]
    public class PPT2PDFTester
    {
        private readonly PPTToPdfConverter _pptToPdfConverter = new PPTToPdfConverter();

        [Test]
        public void should_convert_from_ppt_to_pdf()
        {
            var from = Environment.CurrentDirectory + @"\TestFiles\\智慧市政综合指挥平台.pptx";
            var to = Environment.CurrentDirectory + @"\TestFiles\\智慧市政综合指挥平台pptx.pdf";

            Assert.AreEqual(ErrorMessages.Success, _pptToPdfConverter.Convert(from,to));
            Assert.IsTrue(File.Exists(to));

        }

        [Test]
        public void should_convert_from_97formatppt_to_pdf()
        {
            var from = Environment.CurrentDirectory + @"\TestFiles\\智慧市政综合指挥平台97format.ppt";
            var to = Environment.CurrentDirectory + @"\TestFiles\\智慧市政综合指挥平台97formatppt.pdf";

            Assert.AreEqual(ErrorMessages.Success, _pptToPdfConverter.Convert(from,to));
            Assert.IsTrue(File.Exists(to));
        }

        [Test]
        public void should_not_convert_from_ppt_to_pdf_if_file_doesntexists()
        {
            var from = Environment.CurrentDirectory + @"\TestFiles\filennotexist.pptx";
            var to = Environment.CurrentDirectory + @"\TestFiles\filennotexist.pdf";

            Assert.Catch(typeof(ConverterException), () => _pptToPdfConverter.Convert(from, to));
            Assert.IsFalse(File.Exists(to));
        }
    }
}