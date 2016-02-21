using System;
using System.IO;
using Documents.Converter.Imp;
using Documents.Exceptions;
using Infrasturcture.Errors;
using NUnit.Framework;

namespace Documents.Test.Converter.Imp
{
    [TestFixture]
    public class Word2PDFTester
    {
        private readonly WordToPdfConverter _wordToPdfConverter = new WordToPdfConverter();

        [Test]
        public void should_convert_from_word_to_pdf()
        {
            var from = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.docx";
            var to = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台docx.pdf";

            Assert.AreEqual(ErrorMessages.Success, _wordToPdfConverter.Convert(from,to));
            Assert.IsTrue(File.Exists(to));
        }


        [Test]
        public void should_not_convert_from_word_to_pdf_if_file_doesntexists()
        {
            var from = Environment.CurrentDirectory + @"\TestFiles\filennotexist.docx";
            var to = Environment.CurrentDirectory + @"\TestFiles\filennotexist.pdf";
            Assert.Catch(typeof(ConverterException), () => _wordToPdfConverter.Convert(from, to));
            Assert.IsFalse(File.Exists(to));
        }
    }

}