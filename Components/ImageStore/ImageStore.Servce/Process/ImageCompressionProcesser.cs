using System;
using System.Drawing;
using Common.Logging;
using ImageStore.Services.Context;
using ImageStore.Services.Tool;
using ImageStore.Services.Utils;

namespace ImageStore.Services.Process
{
    public class ImageCompressionProcesser : ImageBaseProcesser
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public override int Process(UploadImageContext context)
        {
            _logger.Info("ImageCompressionProcesser");

            try
            {
                Size size = GetImageSize(context);
     
                var stream = ImageCompress.CompressImage(context.ImageFormat, context.ImageStream, size, true);

                string fileName = FileUtils.SaveImage(stream, context.ImageExt, ImageStoreSettings.CompressDir);
                context.CompressUrl = ImageStoreSettings.CompressUrl + fileName;

                _logger.InfoFormat("ImageCompressionProcesser Complete, fileName:{0}, compressUrl:{1}", fileName, context.CompressUrl);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                _logger.Error(e.StackTrace);
                return ImageErrorMessage.ImageUndefinedError;
            }

            return NextProcesser.Process(context);
        }


        public Size GetImageSize(UploadImageContext context)
        {
            var sourceImgSize = new Size();
            using (var originalImage = Image.FromStream(context.ImageStream))
            {
                sourceImgSize.Width = originalImage.Width;
                sourceImgSize.Height = originalImage.Height;
            }

            var width = sourceImgSize.Width;
            var height = sourceImgSize.Height;

            if (width > ImageConstants.MaxImageWidth)
            {
                var percent = ImageConstants.MaxImageWidth / (double)width;
                height = (int)(percent * height);
                width = ImageConstants.MaxImageWidth;
            }
            return new Size { Width = width, Height = height };
        }

    }
}