using Common.Logging;
using ImageStore.Services.Context;
using ImageStore.Services.Utils;

namespace ImageStore.Services.Process
{
    public class ImageEmptyProcesser : ImageBaseProcesser
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public ImageEmptyProcesser() : base(null)
        {
        }

        public override int Process(UploadImageContext context)
        {
            _logger.Debug("ImageCompressionProcesser");

            return ImageErrorMessage.ImageUploadSucess;
        }
    }
}