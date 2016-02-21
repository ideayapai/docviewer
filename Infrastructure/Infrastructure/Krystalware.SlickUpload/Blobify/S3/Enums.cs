namespace Krystalware.SlickUpload.Blobify.S3
{
    /// <summary>
    /// Defines the Amazon S3 Access Control List (ACL) setting to use for an object. Default is <see cref="S3CannedAcl.Private"/>.
    /// </summary>
    public enum S3CannedAcl
    {
        /// <summary>
        /// Owner gets FULL_CONTROL. No one else has access rights (default).
        /// </summary>
        Private,
        /// <summary>
        /// Owner gets FULL_CONTROL. Anonymous users (the AllUsers) group gets READ access.
        /// If this policy is used on an object, it can be read from a browser with no authentication.
        /// </summary>
        PublicRead,
        /// <summary>
        /// Owner gets FULL_CONTROL, Anonymous users (the AllUsers) group gets READ and WRITE access.
        /// </summary>
        PublicReadWrite,
        /// <summary>
        /// Owner gets FULL_CONTROL. The AuthenticatedUsers group (any principal authenticated as a registered 
        /// Amazon S3 user) gets READ access.
        /// </summary>
        AuthenticatedRead,
        /// <summary>
        /// Object owner gets FULL_CONTROL. Bucket owner gets READ access. If specified when creating a bucket, Amazon S3 ignores it.
        /// </summary>
        BucketOwnerRead,
        /// <summary>
        /// Both the object owner and the bucket owner get FULL_CONTROL. If specified when creating a bucket, Amazon S3 ignores it.
        /// </summary>
        BucketOwnerFullControl,
        /// <summary>
        /// The LogDelivery group gets WRITE and the READ_ACP permission on the bucket. Only applicable for buckets.
        /// </summary>
        LogDeliveryWrite
    }

    /// <summary>
    /// Defines the Amazon S3 Storage Class to use for an object. Default is <see cref="S3StorageClass.Standard"/>.
    /// </summary>
    public enum S3StorageClass
    {
        /// <summary>
        /// Use standard storage.
        /// </summary>
        Standard,
        /// <summary>
        /// Use reduced redundancy storage.
        /// </summary>
        ReducedRedundancy
    }
}
