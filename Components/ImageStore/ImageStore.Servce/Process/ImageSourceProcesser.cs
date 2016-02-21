using System;
using Common.Logging;
using ImageStore.Services.Context;
using ImageStore.Services.Tool;
using ImageStore.Services.Utils;

namespace ImageStore.Services.Process
{
    public class ImageSourceProcesser : ImageBaseProcesser
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public override int Process(UploadImageContext context)
        {
            SaveSourceImage(context);

            return NextProcesser.Process(context);
        }

        private void SaveSourceImage(UploadImageContext context)
        {
            _logger.Info("ImageSourceProcesser");

            try
            {
                string fileName = FileUtils.SaveImage(context.ImageStream, context.ImageExt, ImageStoreSettings.ImageDir);
                context.ImageUrl = ImageStoreSettings.ImageUrl + fileName;

                _logger.InfoFormat("ImageSourceProcesser Complete, fileName:{0}, compressUrl:{1}", fileName, context.CompressUrl);

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }
        }

    }
}