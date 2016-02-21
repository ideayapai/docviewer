
using System.Collections.Specialized;
using ImageStore.Services.Client;
using NUnit.Framework;

namespace ImageStore.UnitTest
{
    [TestFixture]
    public class ImageUploaderTester
    {
        [Test]
        public void should_call_imageUploader_to_upload_image()
        {
            var nvc = new NameValueCollection();

            ImageClient.Upload(TestConstants.TestFileName, nvc);
        }
    }
}
