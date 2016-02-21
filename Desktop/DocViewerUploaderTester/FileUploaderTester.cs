using System;
using System.Collections.Specialized;
using DocViewerUploader;
using NUnit.Framework;

namespace DocViewerUploaderTester
{
    [TestFixture]
    public class FileUploaderTester
    {
        [Test]
        public void should_submit_image_success()
        {
            var nvc = new NameValueCollection {{"userId", Guid.NewGuid().ToString()},
                                               {"userName", "admin"}};
            var fileUploader = new FormUploader("http://192.168.1.29:2001/api/Document/Add");
            var result = fileUploader.Upload("pic.bmp", nvc);
        }

        [Test]
        public void should_submit_office_success()
        {
            var nvc = new NameValueCollection {{"userId", Guid.NewGuid().ToString()},
                                               {"userName", "admin"}};
            var fileUploader = new FormUploader("http://192.168.1.29:2001/api/Document/Add");
            fileUploader.Upload("office.xlsx", nvc);
        }
    }
}
