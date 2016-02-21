using System;
using System.Collections.Generic;

namespace Krystalware.SlickUpload.Blobify.Abstract
{
    public class RestBlobInfo<TBlobPartInfo> : BlobInfo where TBlobPartInfo : RestBlobPartInfo
    {
        List<TBlobPartInfo> _parts = new List<TBlobPartInfo>();

        public List<TBlobPartInfo> Parts { get { return _parts; } }
        public string UploadId { get; internal set; }

        // TODO: rewrite doc comments

        /// <summary>
        /// Gets or sets the optional expiration date of the object. If specified, it will be 
        /// stored by S3 and returned as a standard Expires header when the object is retrieved.
        /// </summary>
        public DateTime? Expires { get; set; }        

        /// <summary>
        /// Gets or sets the cache control for this request as the raw HTTP header you would like
        /// S3 to return along with your object when requested. An example value for this might
        /// be "max-age=3600, must-revalidate".
        /// </summary>
        public string CacheControl { get; set; }

        /// <summary>
        /// Gets or sets the MIME type of this object. It will be stored by S3 and returned as a
        /// standard Content-Type header when the object is retrieved.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the size of the object you are adding. Setting this property is required.
        /// </summary>
        /*public long ContentLength
        {
            get { return WebRequest.ContentLength; }
            set { WebRequest.ContentLength = value; contentLengthWasSet = true; }
        }

        /// <summary>
        /// Gets or sets the base64 encoded 128-bit MD5 digest of the message (without the headers)
        /// according to RFC 1864.
        /// </summary>
        public string ContentMD5
        {
            get { return WebRequest.Headers[HttpRequestHeader.ContentMd5]; }
            set { WebRequest.Headers[HttpRequestHeader.ContentMd5] = value; }
        }*/

        /// <summary>
        /// Gets or sets presentational information for the object. It will be stored by S3 and
        /// returned as a standard Content-Disposition header when the object is retrieved.
        /// </summary>
        /// <remarks>
        /// One use of this header is to cause a browser to download this resource as a file attachment
        /// instead of displaying it inline. For that behavior, use a string like:
        /// "Content-disposition: attachment; filename=mytextfile.txt"
        /// </remarks>
        public string ContentDisposition { get; set; }

        /// <summary>
        /// Gets or sets the specified encoding of the object data. It will be stored by S3 
        /// and returned as a standard Content-Encoding header when the object is retrieved.
        /// </summary>
        public string ContentEncoding { get; set; }
    }
}
