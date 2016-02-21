using System;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using ImageStore.Services.Files;

namespace ImageStore.Services.Context
{
    public class UploadImageContext : IDisposable
    {
        private readonly BaseFile _imageFile;
        private Stream _stream;
        private long _contentLength;
        private string _fileName;

        /// <summary>
        /// 本地文件构造参数
        /// </summary>
        /// <param name="fileName"></param>
        public UploadImageContext(string fileName)
        {
            _imageFile = new LocalFile(fileName);
            _stream = _imageFile.Stream;
            _contentLength = _imageFile.ContentLength;
            _fileName = _imageFile.FileName;
        }

        /// <summary>
        /// 图片构造文件
        /// </summary>
        /// <param name="baseFile"></param>
        public UploadImageContext(Stream stream, string fileName)
        {
            _stream = stream;
            _fileName = fileName;
            _contentLength = _stream.Length;
        }

        /// <summary>
        /// 网络文件构造参数
        /// </summary>
        /// <param name="httpPostedFile"></param>
        public UploadImageContext(HttpPostedFileBase httpPostedFile)
        {
            _imageFile = new NetworkFile(httpPostedFile);
            _stream = _imageFile.Stream;
            _contentLength = _imageFile.ContentLength;
            _fileName = _imageFile.FileName;

        }

        

        /// <summary>
        /// 原始图片数据流
        /// </summary>
        public Stream ImageStream 
        {
            get { return _stream; }
         
        }

        /// <summary>
        /// 图片长度
        /// </summary>
        public long ContentContentLength 
        {
            get { return _contentLength; } 
        }

        /// <summary>
        /// 图片名称
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
        }
        
        /// <summary>
        /// 原始文件Url
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 压缩文件Url
        /// </summary>
        public string CompressUrl { get; set; }

        /// <summary>
        /// 小图片Url
        /// </summary>
        public string ThumbUrl { get; set; }

        /// <summary>
        /// 图片后缀 
        /// </summary>
        public string ImageExt
        {
            get { return Path.GetExtension(_fileName); }
        }

        /// <summary>
        /// 图片格式
        /// </summary>
        public ImageFormat ImageFormat 
        { 
            get
            {
                switch (ImageExt.ToLower())
                {
                    case ".jpg":
                    case ".jpeg":
                        return ImageFormat.Jpeg;
                    case ".gif":
                        return ImageFormat.Gif;
                    case ".bmp":
                        return ImageFormat.Bmp;
                    case ".tif":
                        return ImageFormat.Tiff;
                    case ".ico":
                        return ImageFormat.Icon;
                    case ".png":
                        return ImageFormat.Png;
                    case ".emf":
                        return ImageFormat.Emf;
                    case "image/x-exif":
                        return ImageFormat.Exif;
                    case "image/x-wmf":
                        return ImageFormat.Wmf;
                    default:
                        return ImageFormat.MemoryBmp;
                }
            }
        }

        /// <summary>
        /// 关闭文件
        /// </summary>
        public void Close()
        {
            if (_imageFile != null)
            {
                _imageFile.Close();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);////释放托管资源
            GC.SuppressFinalize(this);//请求系统不要调用指定对象的终结器. //该方法在对象头中设置一个位，系统在调用终结器时将检查这个位
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)//_isDisposed为false表示没有进行手动dispose
            {
                if (disposing)
                {
                    Close();
                }
            }
            _isDisposed = true;
        }

        private bool _isDisposed;

        ~UploadImageContext()
        {
            this.Dispose(false);//释放非托管资源，托管资源由终极器自己完成了
        }
    }
}