using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ImageStore.Services.Tool
{
    public static class ImageCompress
    {
        public static MemoryStream CompressImage(ImageFormat format, Stream stream, Size size, bool isbig)
        {
            using (var image = Image.FromStream(stream))
            {
                Image bitmap = new Bitmap(size.Width, size.Height);
                Graphics g = Graphics.FromImage(bitmap);
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.Transparent);

                //按指定面积缩小
                if (!isbig)
                {
                    g.DrawImage(image, 
                                new Rectangle(0, 0, size.Width, size.Height), 
                                ImageCutSize(image, size),
                                GraphicsUnit.Pixel);
                }
                //按等比例缩小
                else
                {
                    g.DrawImage(image, 
                                new Rectangle(0, 0, size.Width, size.Height), 
                                new Rectangle(0, 0, image.Width, image.Height),
                                GraphicsUnit.Pixel);
                }

                var compressedImageStream = new MemoryStream();
                bitmap.Save(compressedImageStream, format);
                g.Dispose();
                bitmap.Dispose();
                return compressedImageStream;
            }
        }


        public static MemoryStream CutPic(Stream stream, int pPartStartPointX, int pPartStartPointY, int pPartWidth,
                                   int pPartHeight, int pOrigStartPointX, int pOrigStartPointY)
        {
            using (Image originalImg = Image.FromStream(stream))
            {
                var cutImageStream = new MemoryStream();
                Bitmap partImg = new Bitmap(pPartWidth, pPartHeight);
                Graphics graphics = Graphics.FromImage(partImg);
                Rectangle destRect = new Rectangle(new Point(pPartStartPointX, pPartStartPointY),
                                                   new Size(pPartWidth, pPartHeight));
                Rectangle origRect = new Rectangle(new Point(pOrigStartPointX, pOrigStartPointY),
                                                   new Size(pPartWidth, pPartHeight));
                graphics.DrawImage(originalImg, destRect, origRect, GraphicsUnit.Pixel);
                partImg.Save(cutImageStream, ImageFormat.Jpeg);
                graphics.Dispose();
                partImg.Dispose();
                return cutImageStream;
            }
        }

        public static MemoryStream CompressImage(Stream stream, Size size)
        {
            using (var image = Image.FromStream(stream))
            {
                Image bitmap = new Bitmap(size.Width, size.Height);
                Graphics g = Graphics.FromImage(bitmap);
                g.InterpolationMode = InterpolationMode.High;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.Clear(Color.Transparent);
                g.DrawImage(image, new Rectangle(0, 0, size.Width, size.Height),
                            new Rectangle(0, 0, image.Width, image.Height),
                            GraphicsUnit.Pixel);
                var compressedImageStream = new MemoryStream();
                bitmap.Save(compressedImageStream, ImageFormat.Jpeg);
                g.Dispose();
                bitmap.Dispose();
                return compressedImageStream;
            }
        }

        public static Size GetSizeByType(Stream stream, int templateWidth, int templateHeight)
        {
            using (var originalImage = Image.FromStream(stream))
            {
                var width = originalImage.Width;
                var height = originalImage.Height;
                //缩略图宽、高

                //宽大于模版的横图
                if (WidthExtendsHeight(originalImage))
                {
                    if (originalImage.Width > templateWidth)
                    {
                        //宽按模版，高按比例缩放
                        width = templateWidth;
                        height = (int)(originalImage.Height * (width / (double)originalImage.Width));
                    }
                }
                else
                {
                    if (originalImage.Height > templateHeight)
                    {
                        //高按模版，宽按比例缩放
                        height = templateHeight;
                        width = (int)(originalImage.Width * (height / (double)originalImage.Height));
                    }
                }

                return new Size { Width = width, Height = height };
            }

        }

        private static bool WidthExtendsHeight(Image originalImage)
        {
            return originalImage.Width > originalImage.Height || originalImage.Width == originalImage.Height;
        }

        private static Rectangle ImageCutSize(Image image, Size size)
        {
            int imageX, imageY, imageWidth, imageHeight;

            if (image.Width < image.Height)
            {
                imageWidth = image.Width;
                imageHeight = image.Width;
            }
            else
            {
                imageWidth = image.Height;
                imageHeight = image.Height;
            }
            if (image.Width > size.Width)
            {
                imageX = (image.Width - imageWidth) / 2;
            }
            else
            {
                imageX = 0;
            }
            if (image.Height > size.Height)
            {
                imageY = (image.Height - imageHeight) / 2;
            }
            else
            {
                imageY = 0;
            }
            return new Rectangle(imageX, imageY, imageWidth, imageHeight);
        }
    }
}
