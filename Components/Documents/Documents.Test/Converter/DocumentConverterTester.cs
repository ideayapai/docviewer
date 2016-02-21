using System;
using System.IO;
using Documents.Converter;
using Documents.Enums;
using NUnit.Framework;

namespace Documents.Test.Converter
{
    [TestFixture]
    public class DocumentConverterTester
    {
        private readonly DocumenConverter _documentConverter = new DocumenConverter();

     

        [Test]
        public void should_convert_from_word_to_html()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.docx";
            string target = file + ".html";
            int result = _documentConverter.Convert(file, target, ConvertFileType.WordToHtml);
            Assert.AreEqual(result, 0);
            Assert.IsTrue(File.Exists(target));
        }


     

        [Test]
        public void should_convert_from_xls_to_html()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\报价明细表.xlsx";
            string target = file + ".html";
            int result = _documentConverter.Convert(file, target, ConvertFileType.ExcelToHtml);
            Assert.IsTrue(File.Exists(target));
            Assert.AreEqual(result, 0);
        }

        [Test]
        public void should_convert_from_ppt_to_html()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.pptx";
            string target = file + ".html";
            int result = _documentConverter.Convert(file, target, ConvertFileType.PPTToHtml);
            Assert.IsTrue(File.Exists(target));
            Assert.AreEqual(result, 0);
        }

        [Test]
        public void should_convert_from_swg_to_svg()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\test.dwg";
            string target = file + ".svg";
            int result = _documentConverter.Convert(file, target, ConvertFileType.CadToSvg);
            Assert.AreEqual(result, 0);
            Assert.IsTrue(File.Exists(target));
        }

        [Test]
        public void should_convert_txt_file()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\sample.txt";
            string target = file + ".html";
            int result = _documentConverter.Convert(file, target, ConvertFileType.TextToHtml);
            Assert.IsTrue(File.Exists(target));
            Assert.AreEqual(result, 0);
        }

        [Test]
        public void should_convert_rtf_file()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\TestRTF.rtf";
            string target = file + ".html";
            int result = _documentConverter.Convert(file, target, ConvertFileType.WordToHtml);
            Assert.IsTrue(File.Exists(target));
            Assert.AreEqual(result, 0);
        }

    }
    
}
