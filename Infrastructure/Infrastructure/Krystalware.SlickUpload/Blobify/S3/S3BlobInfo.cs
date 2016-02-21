using Krystalware.SlickUpload.Blobify.Abstract;

namespace Krystalware.SlickUpload.Blobify.S3
{
    public class S3BlobInfo : RestBlobInfo<S3BlobPartInfo>
    {
        /// <summary>
        /// Gets or sets the Amazon S3 Access Control List (ACL) setting to apply to this object. The default is
        /// <see cref="S3CannedAcl.Private"/>.
        /// </summary>
        /// <remarks>
        /// More complex permission sets than the CannedAcl values can be set after object creation, using <see cref="S3BlobClient.SetObjectAcl" /> (not currently implemented).
        /// </remarks>
        public S3CannedAcl CannedAcl { get; set; }

        /// <summary>
        /// Gets or sets the Amazon S3 Storage Class to use for this object. The default is <see cref="S3StorageClass.Standard"/>.
        /// </summary>
        public S3StorageClass StorageClass { get; set; }
    }
}
