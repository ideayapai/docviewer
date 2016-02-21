using System.Text.RegularExpressions;
using Common.Logging;
using ImageStore.Services.Context;
using ImageStore.Services.Utils;

namespace ImageStore.Services.Process
{
    /// <summary>
    /// 检查验证信息
    /// </summary>
    public class ImageCheckProcesser : ImageBaseProcesser
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public override int Process(UploadImageContext context)
        {
            if (IsFileSizeExtendMaxSize(context.ContentContentLength))
            {
                _logger.InfoFormat("图片超过了大小，图片大小为{0}", context.ContentContentLength);
                return ImageErrorMessage.ImageTooLarge;
            }

            if (IsFileSizeIsNegative(context.ContentContentLength))
            {
                _logger.Info("图片大小为负数");
                return ImageErrorMessage.ImageNegative;
            }

            if (!IsImageTypeSupport(context.ImageExt))
            {
                _logger.InfoFormat("图片后缀为{0},后缀不支持", context.ImageExt);
                return ImageErrorMessage.ImageTypeNotSupport;
            }

            return NextProcesser.Process(context);
        }
        
        private static bool IsFileSizeIsNegative(long contentLength)
        {
            return contentLength <= 0;
        }

        private static bool IsFileSizeExtendMaxSize(long contentLength)
        {
            return contentLength > ImageStoreSettings.MaxImageSize;
        }

        private static bool IsImageTypeSupport(string imageExt)
        {
            if (string.IsNullOrEmpty(imageExt))
            {
                return false;
            }

            var regex = new Regex(ImageStoreSettings.SupportImageType, RegexOptions.IgnoreCase);
            return regex.IsMatch(imageExt);
        }
    }
}