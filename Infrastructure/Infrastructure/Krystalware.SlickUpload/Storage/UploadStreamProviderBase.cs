using System;
using System.IO;
using System.Configuration;
using Krystalware.SlickUpload.Configuration;

namespace Krystalware.SlickUpload.Storage
{
    /// <summary>
    /// Exposes an abstract base class for <see cref="IUploadStreamProvider"/>s.
    /// </summary>
    public abstract class UploadStreamProviderBase : IUploadStreamProvider
    {
        UploadStreamProviderElement _settings;

        /// <summary>
        /// Gets the settings for this upload stream provider.
        /// </summary>
        protected UploadStreamProviderElement Settings { get { return _settings; } }

        /// <summary>
        /// Creates a new instance of the <see cref="UploadStreamProviderBase" /> class with the specified settings.
        /// </summary>
        /// <param name="settings">The <see cref="UploadStreamProviderElement" /> object that holds the configuration settings.</param>
        protected UploadStreamProviderBase(UploadStreamProviderElement settings)
        {
            _settings = settings;
        }

        /// <inheritdoc />
        public abstract Stream GetWriteStream(UploadedFile file);

        /// <inheritdoc />
        public virtual void CloseWriteStream(UploadedFile file, Stream stream, bool isComplete)
        {
            ((IDisposable)stream).Dispose();
        }

        /// <inheritdoc />
        public abstract void RemoveOutput(UploadedFile file);

        /// <inheritdoc />
        public abstract Stream GetReadStream(UploadedFile file);
    }
}
