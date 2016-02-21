using System.Configuration;
using ImageStore.Services.Context;
using ImageStore.Services.Process;
using ImageStore.Services.Utils;
using NUnit.Framework;

namespace ImageStore.UnitTest
{
    [TestFixture]
    public class ImageSaveProcesser2Tester
    {
        private UploadImageContext GetSaveResult()
        {
            var context = UploadImageContextMaker.MockImageContext(TestConstants.TestFileName);
            var process = new ImageSourceProcesser();
            int result = process.Process(context);
            Assert.AreEqual(result, ImageErrorMessage.ImageUploadSucess);
            return context;
        }

        [Test]
        public void should_save_image_if_upload_type_is_image()
        {
            using(var result = GetSaveResult())
            {
                
                Assert.AreNotEqual(result.ImageUrl.IndexOf(ConfigurationManager.AppSettings["ImageStore.ImageUrl"]), -1);            
            }
        }

        [Test]
        public void should_save_image_if_upload_type_is_no_change()
        {
            using(var result = GetSaveResult())
            {
                Assert.AreNotEqual(result.ImageUrl.IndexOf(ConfigurationManager.AppSettings["ImageStore.ImageUrl"]), -1);                
            }
        }

        [Test]
        public void should_save_image_if_upload_type_is_common_change()
        {
            using(var result = GetSaveResult())
            {
                Assert.AreNotEqual(result.ImageUrl.IndexOf(ConfigurationManager.AppSettings["ImageStore.ImageUrl"]), -1);                
            }
        }
    }
    

}
