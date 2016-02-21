using System;
using System.Web;
using Infrasturcture.Store.Utils;
using NUnit.Framework;

namespace Infrasturcture.Tests.Store
{
    [TestFixture]
    public class MimeMappingTester
    {
        [Test]
        public void should_get_mimeType_with_fileName()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.docx";
            var mimeType = MimeMapping.GetMimeMapping(file);
            Assert.AreEqual(mimeType, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }

        [Test]
        public void should_get_mimeType_with_fileName_by_suffix()
        {
            string file = Environment.CurrentDirectory + @"\TestFiles\智慧市政综合指挥平台.docx";
            var mimeType = MimeMapper.GetMimeMapping(file);
            Assert.AreEqual(mimeType, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
        }
    }
}
