using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TextReader = Documents.Reader.Imp.TextReader;

namespace Documents.Test.Reader
{
    [TestFixture]
    public class TextReaderTester
    {
        private readonly TextReader _reader = new TextReader();
            
        [Test]
        public void should_read_text_file()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\文档测试.txt";
            string content = _reader.Read(file);
            Console.WriteLine(content);
            Assert.IsTrue(content=="it's just test.文档测试.");
        }

        [Test]
        public void should_read_text_file_with_stream()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\文档测试.txt";
            using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                string content = _reader.Read(fs);
                Console.WriteLine(content);
                Assert.IsTrue(content == "it's just test.文档测试.");
            }
        
        }
    }
}
