using System;
using System.IO;
using Documents.Enums;
using Documents.Reader;
using NUnit.Framework;

namespace Documents.Test.Reader
{
    [TestFixture]
    public class DocumentReaderTester
    {
        readonly DocumentReader _reader = new DocumentReader();

        [Test]
        public void should_read_content_from_word()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.docx";
            string content = _reader.Read(file);
            Assert.IsTrue(content.Contains("市政"));
        }

        [Test]
        public void should_read_content_from_word_by_stream()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.docx";
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                string content = _reader.Read(stream, DocumentType.Word);
                Assert.IsTrue(content.Contains("市政"));
            }
        }

        [Test]
        public void should_read_content_from_word_with97format()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台97format.doc";
            string content = _reader.Read(file);
            Assert.IsTrue(content.Contains("市政"));
            
        }
        [Test]
        public void should_read_content_from_word_with97format_by_stream()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台97format.doc";
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                string content = _reader.Read(stream, DocumentType.Word);
                Assert.IsTrue(content.Contains("市政"));
            }
        }

        [Test]
        public void should_read_content_from_excel()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\报价明细表.xlsx";
            string content = _reader.Read(file);
            Assert.IsTrue(content.Contains("市政"));
        }

        [Test]
        public void should_read_content_from_excel_by_stream()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\报价明细表.xlsx";
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                string content = _reader.Read(stream, DocumentType.Excel);
                Assert.IsTrue(content.Contains("市政"));
            }
        }

        [Test]
        public void should_read_content_from_ppt()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.pptx";
            string content = _reader.Read(file);
            Assert.IsTrue(content.Contains("市政"));
        }

        [Test]
        public void should_read_content_from_ppt_by_stream()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.pptx";
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                string content = _reader.Read(stream, DocumentType.PPT);
                Assert.IsTrue(content.Contains("市政"));
            }
        }

        [Test]
        public void should_read_content_from_ppt_with97format()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台97format.ppt";
            string content = _reader.Read(file);
            Assert.IsTrue(content.Contains("市政"));
        }


        [Test]
        public void should_read_content_from_ppt_with97format_by_stream()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台97format.ppt";
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                string content = _reader.Read(stream, DocumentType.PPT);
                Assert.IsTrue(content.Contains("市政"));
            }
        }

        [Test]
        public void should_read_content_from_pdf()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\Guidance_User_Manual_v1.2_cn.pdf";
            string content = _reader.Read(file);
            Assert.IsTrue(content.Contains("大疆创新"));
        }

        [Test]
        public void should_read_content_from_pdf_by_stream()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\Guidance_User_Manual_v1.2_cn.pdf";
            string content = _reader.Read(file);
            Assert.IsTrue(content.Contains("大疆创新"));
        }

        [Test]
        public void should_read_content_from_rtf()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\TestRTF.rtf";
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                string content = _reader.Read(stream, DocumentType.RTF);
                Assert.IsTrue(content.Contains("管理信息系统"));
            }
        }

        [Test]
        public void should_read_content_from_rtf_by_stream()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\TestRTF.rtf";
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                string content = _reader.Read(stream, DocumentType.RTF);
                Assert.IsTrue(content.Contains("管理信息系统"));
            }
        }
        
        [Test]
        public void should_read_content_from_txt()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.txt";
            string content = _reader.Read(file);
            Assert.IsTrue(content.Contains("市政"));
        }

        [Test]
        public void should_read_content_from_txt_by_stream()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.txt";
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                string content = _reader.Read(stream, DocumentType.TXT);
                Assert.IsTrue(content.Contains("市政"));
            }
        }

        [Test]
        public void should_read_content_from_txt_unicode()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台unicode.txt";
            string content = _reader.Read(file);
            Assert.IsTrue(content.Contains("市政"));
        }

        [Test]
        public void should_read_content_from_txt_unicode_by_stream()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台unicode.txt";
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                string content = _reader.Read(stream, DocumentType.TXT);
                Assert.IsTrue(content.Contains("市政"));
            }
        }

     
    }

}