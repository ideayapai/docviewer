using System;
using System.IO;
using DwgLib;
using NUnit.Framework;

namespace DwgConverterTester
{
    [TestFixture]
    public class CadConverterTests
    {
        private const string sourcePath = @"\TestFiles\别墅设计全套施工图.dwg";

        [Test]
        public void should_export_dwg_to_pdf()
        {
            CadConverter converter = new CadConverter();
        

            int result = converter.Convert(Environment.CurrentDirectory + sourcePath,
                                                      Environment.CurrentDirectory + sourcePath + ".pdf");
            Assert.AreEqual(result, 0);
            Assert.True(File.Exists(Environment.CurrentDirectory + sourcePath + ".pdf"));
        }

        [Test]
        public void should_export_dwg_to_bmp()
        {
            CadConverter converter = new CadConverter();


            int result = converter.Convert(Environment.CurrentDirectory + sourcePath,
                                                      Environment.CurrentDirectory + sourcePath + ".bmp");
            Assert.AreEqual(result, 0);
            Assert.True(File.Exists(Environment.CurrentDirectory + sourcePath + ".bmp"));
        }

        [Test]
        public void should_export_dwg_to_svg()
        {
            CadConverter converter = new CadConverter();


            int result = converter.Convert(Environment.CurrentDirectory + sourcePath,
                                                      Environment.CurrentDirectory + sourcePath + ".svg");
            Assert.AreEqual(result, 0);
            Assert.True(File.Exists(Environment.CurrentDirectory + sourcePath + ".svg"));
        }
    }
}
