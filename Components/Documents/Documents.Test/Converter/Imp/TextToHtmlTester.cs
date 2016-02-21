using System;
using Documents.Converter.Imp;
using Infrasturcture.Errors;
using NUnit.Framework;

namespace Documents.Test.Converter.Imp
{
    [TestFixture]
    public class Text2HtmlTester
    {
        private readonly TextToHtmlConverter _strategy = new TextToHtmlConverter();

        [Test]
        public void should_convert_text()
        {
            string sourcePath = Environment.CurrentDirectory + @"\TestFiles\test.txt";
            string targetPath = Environment.CurrentDirectory + @"\TestFiles\test.html";

            var result = _strategy.Convert(sourcePath, targetPath);
            Assert.AreEqual(result, ErrorMessages.Success);
        }
    }
}
