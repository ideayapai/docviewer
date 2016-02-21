using System;
using System.IO;
using NUnit.Framework;
using Services.Documents;

namespace Services.Tests
{
    [TestFixture]
    public class DocumentSettingTester
    {
        [Test]
        public void should_set_configuration_if_not_exist()
        {
            string path = DocumentSettings.GetStorePath("file.txt");
            Console.WriteLine(path);
            Assert.IsTrue(Directory.Exists(Path.GetDirectoryName(path)));
        }
    }
}
