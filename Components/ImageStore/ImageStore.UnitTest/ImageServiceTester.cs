using System;
using ImageStore.Services;
using NUnit.Framework;

namespace ImageStore.UnitTest
{
    [TestFixture]
    public class ImageServiceTester
    {
        [Test]
        public void should_upload_image()
        {
            ImageService imageService = new ImageService();
            var result = imageService.Upload(TestConstants.TestFileName);

            Assert.IsTrue(result.Success);
            Console.WriteLine("compressImageUrl:" + result.CompressImageUrl);
            Console.WriteLine("thumbImageUrl:" + result.ThumbImageUrl);
            Console.WriteLine("imageUrl:" + result.ImageUrl);
        }


    }
}


