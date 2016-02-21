using System;
using System.IO;
using System.Reflection;
using System.Web;

using Krystalware.SlickUpload.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Hosting;
using Krystalware.SlickUpload.Blobify.S3;

namespace Krystalware.SlickUpload.Storage
{
	/// <summary>
	/// An <see cref="IUploadStreamProvider" /> that writes to files.
	/// </summary>
	public class S3UploadStreamProvider : UploadStreamProviderBase
	{
		/// <summary>
		/// Enumeration of the methods to use when generating an object name.
		/// </summary>
		public enum ObjectNameMethod
		{
			/// <summary>
			/// Use the name the file was named on the client.
			/// </summary>
			Client,
			/// <summary>
            /// Generate a unique <see cref="Guid" />.
			/// </summary>
			Guid,
			/// <summary>
            /// Generate a unique <see cref="Guid" /> with the client filename's extension appended.
			/// </summary>
			GuidWithExtension
		}

        S3BlobClient _client;

        ObjectNameMethod _objectNameMethod;

        string _cacheControl;
        string _contentDisposition;
        string _contentEncoding;
        S3CannedAcl _cannedAcl;
        S3StorageClass _storageClass;

        /// <summary>
        /// Creates a new instance of the <see cref="S3UploadStreamProvider" /> class with the specified configuration settings.
        /// </summary>
        /// <param name="settings">The <see cref="UploadStreamProviderElement" /> object that holds the configuration settings.</param>
        public S3UploadStreamProvider(UploadStreamProviderElement settings)
            : base(settings)
		{
            string accessKeyId = Settings.Parameters["accessKeyId"];
            string secretAccessKey = Settings.Parameters["secretAccessKey"];
            string bucketName = Settings.Parameters["bucketName"];
            bool useHttps;

            if (string.IsNullOrEmpty(accessKeyId))
                throw new Exception("accessKeyId must be specified for SlickUpload S3 provider");
            if (string.IsNullOrEmpty(secretAccessKey))
                throw new Exception("secretAccessKey must be specified for SlickUpload S3 provider");
            if (string.IsNullOrEmpty(bucketName))
                throw new Exception("bucketName must be specified for SlickUpload S3 provider");

            if (!bool.TryParse(Settings.Parameters["useHttps"], out useHttps))
                useHttps = true;

            _client = new S3BlobClient(accessKeyId, secretAccessKey, bucketName, useHttps);

            _objectNameMethod = TypeFactory.ParseEnum<ObjectNameMethod>(Settings.Parameters["objectNameMethod"], ObjectNameMethod.Client);

            _cacheControl = Settings.Parameters["cacheControl"];
            _contentDisposition = Settings.Parameters["contentDisposition"];
            _contentEncoding = Settings.Parameters["contentEncoding"];
            _cannedAcl = TypeFactory.ParseEnum<S3CannedAcl>(Settings.Parameters["cannedAcl"], S3CannedAcl.Private);
            _storageClass = TypeFactory.ParseEnum<S3StorageClass>(Settings.Parameters["storageClass"], S3StorageClass.Standard);
        }

        /// <summary>
        /// Returns the object name to use for a given <see cref="UploadedFile" />.
        /// </summary>
        /// <param name="file">The <see cref="UploadedFile" /> for which to generate an object name.</param>
        /// <returns>The generated object name.</returns>
        public virtual string GetObjectName(UploadedFile file)
        {
            switch (_objectNameMethod)
            {
                default:
                case ObjectNameMethod.Client:
                    return file.ClientName;
                case ObjectNameMethod.Guid:
                    return Guid.NewGuid().ToString("n");
                case ObjectNameMethod.GuidWithExtension:
                    return Guid.NewGuid().ToString("n") + Path.GetExtension(file.ClientName);
            }
        }

        /// <summary>
        /// Returns the <see cref="S3BlobInfo" /> that defines how to store given <see cref="UploadedFile" />.
        /// </summary>
        /// <param name="file">The <see cref="UploadedFile" /> for which to get a <see cref="S3BlobInfo" />.</param>
        /// <returns>The <see cref="S3BlobInfo" />.</returns>
        public virtual S3BlobInfo CreateBlobInfo(UploadedFile file)
        {
            string fileContentDisposition = null;

            if (!string.IsNullOrEmpty(_contentDisposition))
                fileContentDisposition = string.Format(_contentDisposition, file.ClientName);

            S3BlobInfo blobInfo = new S3BlobInfo()
            {
                Name = GetObjectName(file),
                ContentType = !string.IsNullOrEmpty(file.ContentType) ? file.ContentType : "binary/octet-stream",
                CacheControl = _cacheControl,
                ContentDisposition = fileContentDisposition,
                ContentEncoding = _contentEncoding,
                CannedAcl = _cannedAcl,
                StorageClass = _storageClass
                // TODO: pass length?
                //Length = file.ContentLength > 0 ? (long?)file.ContentLength : null
            };

            return blobInfo;
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override Stream GetWriteStream(UploadedFile file)
        {
            S3BlobInfo blobInfo = CreateBlobInfo(file);

            file.ServerLocation = blobInfo.Name;

            // TODO: pass length?
            return _client.GetPutBlobStream(blobInfo);
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override void RemoveOutput(UploadedFile file)
        {
            _client.DeleteBlob(file.ServerLocation);
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override Stream GetReadStream(UploadedFile file)
        {
            return _client.GetBlob(file.ServerLocation);
        }
    }
}