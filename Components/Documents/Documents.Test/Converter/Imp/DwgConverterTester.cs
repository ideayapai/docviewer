using System;
using System.IO;
using Documents.Converter.Imp;
using NUnit.Framework;

namespace Documents.Test.Converter.Imp
{
    [TestFixture]
    public class DwgConverterTester
    {
        private readonly DwgConverter _documentTransformer = new DwgConverter();

        private const string sourcePath =  @"\TestFiles\别墅设计全套施工图.dwg";

        [Test]
        public void should_convert_from_dwg_to_svg()
        {
            int result = _documentTransformer.Convert(Environment.CurrentDirectory + sourcePath,
                                                      Environment.CurrentDirectory + sourcePath + ".svg");
            Assert.AreEqual(result, 0);
            Assert.IsTrue(File.Exists(Environment.CurrentDirectory + sourcePath + ".svg"));
        }

        [Test]
        public void should_convert_from_dwg_to_pdf()
        {
            int result = _documentTransformer.Convert(Environment.CurrentDirectory + sourcePath,
                                                      Environment.CurrentDirectory + sourcePath + ".pdf");
            Assert.AreEqual(result, 0);
            Assert.IsTrue(File.Exists(Environment.CurrentDirectory + sourcePath + ".pdf"));
        }

        [Test]
        public void should_convert_from_dwg_to_bmp()
        {
            int result = _documentTransformer.Convert(Environment.CurrentDirectory + sourcePath,
                                                     Environment.CurrentDirectory + sourcePath + ".bmp");
            Assert.AreEqual(result, 0);
            Assert.IsTrue(File.Exists(Environment.CurrentDirectory + sourcePath + ".bmp"));
        }

    }
    
}
