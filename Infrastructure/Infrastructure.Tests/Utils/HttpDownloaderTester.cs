using System;
using System.IO;
using Infrasturcture.Utils;
using NUnit.Framework;

namespace Infrasturcture.Tests.Utils
{
    [TestFixture]
    public class HttpDownloaderTester
    {

        [Test]
        public void should_download_office()
        {
            string uri = @"http://192.168.1.29:2002/test.doc";
            string localfile = Guid.NewGuid() + ".doc";
            string localPath = Path.Combine(Environment.CurrentDirectory, localfile);
            HttpDownloader.SaveAsFile(uri, localPath);
            Assert.IsTrue(File.Exists(localPath));
        }


        [Test]
        public void should_download_ppt()
        {
            string uri = @"http://192.168.1.29:2002/test.ppt";
            string localfile = Guid.NewGuid() + ".pptx";
            string localPath = Path.Combine(Environment.CurrentDirectory, localfile);
            //HttpDownloader.SaveAsFile(uri, localPath);
            //Assert.IsTrue(File.Exists(localPath));
        }
    }
}
