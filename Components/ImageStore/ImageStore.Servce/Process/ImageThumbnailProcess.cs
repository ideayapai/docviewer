using System;
using System.Drawing;
using System.IO;
using Common.Logging;
using ImageStore.Services.Context;
using ImageStore.Services.Tool;
using ImageStore.Services.Utils;

namespace ImageStore.Services.Process
{
    /// <summary>
    /// 保存
    /// </summary>
    public class ImageThumbnailProcess : ImageBaseProcesser
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public override int Process(UploadImageContext context)
        {
            _logger.Debug("ImageThumbnailProcess");

            try
            {
                Size thumbnailSize = GetThumbnailSize(context.ImageStream);
                var stream = ImageCompress.CompressImage(context.ImageFormat, context.ImageStream, thumbnailSize, false);
                string fileName = FileUtils.SaveImage(stream, context.ImageExt, ImageStoreSettings.ThumbDir);
                context.ThumbUrl = ImageStoreSettings.ThumbUrl + fileName;

                _logger.InfoFormat("ImageSourceProcesser Complete, fileName:{0}, compressUrl:{1}", fileName, context.CompressUrl);

            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return ImageErrorMessage.ImageUndefinedError;
            }

            return NextProcesser.Process(context);
        }

        private Size GetThumbnailSize(Stream stream)
        {
            return ImageCompress.GetSizeByType(stream, ImageConstants.MaxSmallimageWidth, ImageConstants.MaxSmallimageHeight);
        }

     
    }


}
