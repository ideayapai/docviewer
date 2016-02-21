using System;
using System.IO;
using Documents.Converter.Imp;
using Infrasturcture.Errors;
using NUnit.Framework;

namespace Documents.Test.Converter.Imp
{
    [TestFixture]
    public class Office2PDFTester
    {
       
        [Test]
        public void should_convert_from_word_to_pdf()
        {
            string from = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.docx";
            string to = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台docx.pdf";

            WordToPdfConverter converter = new WordToPdfConverter();

            Assert.AreEqual(ErrorMessages.Success, converter.Convert(from, to));
            Assert.IsTrue(File.Exists(to));

        }

        [Test]
        public void should_convert_from_excel_to_pdf()
        {
            ExcelToPdfConverter _converter = new ExcelToPdfConverter();
            Assert.AreEqual(ErrorMessages.Success, _converter.Convert(Environment.CurrentDirectory + @"\TestFiles\报价明细表.xlsx",
                                           Environment.CurrentDirectory + @"\TestFiles\报价明细表excel.pdf"));
            Assert.IsTrue(File.Exists(Environment.CurrentDirectory + @"\TestFiles\报价明细表excel.pdf"));
        }

        [Test]
        public void should_convert_from_ppt_to_pdf()
        {
            PPTToPdfConverter _converter = new PPTToPdfConverter();
            Assert.AreEqual(ErrorMessages.Success, _converter.Convert(Environment.CurrentDirectory + @"\TestFiles\\智慧市政综合指挥平台.pptx",
                                         Environment.CurrentDirectory + @"\TestFiles\\智慧市政综合指挥平台pptx.pdf"));
            Assert.IsTrue(File.Exists(Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台pptx.pdf"));

        }

        [Test]
        public void should_convert_from_txt_to_pdf()
        {
            TextToPdfConverter _converter = new TextToPdfConverter();
            Assert.AreEqual(ErrorMessages.Success, _converter.Convert(Environment.CurrentDirectory + @"\TestFiles\\智慧市政综合指挥平台.txt",
                                         Environment.CurrentDirectory + @"\TestFiles\\智慧市政综合指挥平台txt.pdf"));
            Assert.IsTrue(File.Exists(Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台txt.pdf"));

        }

        [Test]
        public void should_convert_from_txt_unicode_to_pdf()
        {
            TextToPdfConverter _converter = new TextToPdfConverter();
            Assert.AreEqual(ErrorMessages.Success, _converter.Convert(Environment.CurrentDirectory + @"\TestFiles\\智慧市政综合指挥平台unicode.txt",
                                         Environment.CurrentDirectory + @"\TestFiles\\智慧市政综合指挥平台unicode.pdf"));
            Assert.IsTrue(File.Exists(Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台unicode.pdf"));

        }

        [Test]
        public void should_convert_from_txt_unicodebig_to_pdf()
        {
            TextToPdfConverter _converter = new TextToPdfConverter();
            Assert.AreEqual(ErrorMessages.Success, _converter.Convert(Environment.CurrentDirectory + @"\TestFiles\\智慧市政综合指挥平台unicodebig.txt",
                                         Environment.CurrentDirectory + @"\TestFiles\\智慧市政综合指挥平台unicodebig.pdf"));
            Assert.IsTrue(File.Exists(Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台unicodebig.pdf"));

        }

        [Test]
        public void should_convert_from_txt_utf8_to_pdf()
        {
            TextToPdfConverter _converter = new TextToPdfConverter();
            Assert.AreEqual(ErrorMessages.Success, _converter.Convert(Environment.CurrentDirectory + @"\TestFiles\\智慧市政综合指挥平台utf8.txt",
                                         Environment.CurrentDirectory + @"\TestFiles\\智慧市政综合指挥平台utf8.pdf"));
            Assert.IsTrue(File.Exists(Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台utf8.pdf"));

        }

    }

}