using Krystalware.SlickUpload.Blobify.Abstract;

namespace Krystalware.SlickUpload.Blobify.S3
{
    public class S3BlobPartInfo : RestBlobPartInfo
    {
        public string ETag { get; internal set; }
    }
}
