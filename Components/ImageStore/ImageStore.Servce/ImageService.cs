using System.IO;
using System.Web;
using Common.Logging;
using ImageStore.Services.Context;
using ImageStore.Services.Files;
using ImageStore.Services.Process;
using ImageStore.Services.Utils;

namespace ImageStore.Services
{
    public class ImageService
    {
        private readonly ImageCheckProcesser _checkProcesser = new ImageCheckProcesser();
        private readonly ImageThumbnailProcess _thumbnailProcesser = new ImageThumbnailProcess();
        private readonly ImageCompressionProcesser _compressionProcesser = new ImageCompressionProcesser();
        private readonly ImageSourceProcesser _sourceProcesser = new ImageSourceProcesser();
        private readonly ImageEmptyProcesser _emptyProcesser = new ImageEmptyProcesser();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public ImageService()
        {
            _checkProcesser.Next(_sourceProcesser).Next(_compressionProcesser).Next(_thumbnailProcesser).Next(_emptyProcesser);
        }

        public UploadImageResult Upload(HttpPostedFileBase file)
        {
            using (var context = new UploadImageContext(file))
            {
                return Upload(context);
            }
            
        }

        public UploadImageResult Upload(Stream stream, string fileName)
        {
            using (var context = new UploadImageContext(stream, fileName))
            {
                return Upload(context);
            }
        }
      
        public UploadImageResult Upload(string fileName)
        {
            _logger.Debug("Upload Image FileName\n\r==================");

            using (var context = new UploadImageContext(fileName))
            {
                return Upload(context);
            }
        }

        private UploadImageResult Upload(UploadImageContext context)
        {
            _logger.Debug("Upload Image \n\r==================");

            int returnCode = _checkProcesser.Process(context);
            bool success = (returnCode == ImageErrorMessage.ImageUploadSucess);

            var result = new UploadImageResult
            {
                Success = success,
                ImageName = context.FileName,
                ImageUrl = context.ImageUrl,
                ThumbImageUrl = context.ThumbUrl,
                CompressImageUrl = context.CompressUrl,
                ReturnCode = returnCode
            };
            _logger.DebugFormat("the image {0} upload succeed!the image path was:{1}, thumb image path:{2}", result.ImageName, result.ImageUrl, result.ThumbImageUrl);
            _logger.Debug("\n\r===========================================");

            return result;
        }
        
    }
}
