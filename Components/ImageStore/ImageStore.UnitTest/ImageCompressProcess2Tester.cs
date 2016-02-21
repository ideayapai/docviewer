using System.Drawing;
using ImageStore.Services.Process;
using ImageStore.Services.Utils;
using NUnit.Framework;

namespace ImageStore.UnitTest
{
    [TestFixture]
    public class ImageCompressProcess2Tester
    {
        private int Compress()
        {
            using(var context = UploadImageContextMaker.MockImageContext(TestConstants.TestFileName))
            {
                var process = new ImageCompressionProcesser();
                return process.Process(context);
            }
        }

        private Size GetSize()
        {
            using(var context = UploadImageContextMaker.MockImageContext(TestConstants.TestFileName))
            {
                var process = new ImageCompressionProcesser();
                return process.GetImageSize(context);
            }
        }

        [Test]
        public void should_return_undifined_errror_if_compress_image_fail()
        {
            var process = new ImageCompressionProcesser();
            int result = process.Process(null);
            Assert.AreEqual(result, ImageErrorMessage.ImageUndefinedError);
        }

        [Test]
        public void should_compress_image()
        {
            int result = Compress();
            Assert.AreNotEqual(result, ImageErrorMessage.ImageUndefinedError);

            var size = GetSize();
            Assert.IsTrue(size.Width<= ImageConstants.MaxImageWidth);
        }

        [Test]
        public void should_compress_common_image()
        {
            int result = Compress();
            Assert.AreEqual(result, ImageErrorMessage.ImageUploadSucess);
        }

        [Test]
        public void should_compress_no_change_image()
        {
            int result = Compress();
            Assert.AreEqual(result, ImageErrorMessage.ImageUploadSucess);
        }
    }
    

}
