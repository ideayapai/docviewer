using System;
using System.Text;
using System.IO;
using System.Web;
using Krystalware.SlickUpload.Configuration;

namespace Krystalware.SlickUpload.Storage
{
    /// <summary>
    /// An <see cref="IUploadStreamProvider" /> that writes to in process memory.
    /// </summary>
    public class MemoryUploadStreamProvider : UploadStreamProviderBase
    {
		/// <summary>
        /// Creates a new instance of the <see cref="MemoryUploadStreamProvider" /> class with the specified configuration settings.
		/// </summary>
        /// <param name="settings">The <see cref="UploadStreamProviderElement" /> object that holds the configuration settings.</param>
        public MemoryUploadStreamProvider(UploadStreamProviderElement settings)
            : base(settings)
        { }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override void CloseWriteStream(UploadedFile file, Stream stream, bool isComplete)
        {
            if (!isComplete)
                stream.Dispose();

            // We actually want to leave the stream open so we can read from it later if it is complete
            stream.Seek(0, SeekOrigin.Begin);
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override Stream GetReadStream(UploadedFile file)
        {
            MemoryStream s = (MemoryStream)HttpContext.Current.Application[file.ServerLocation];

            // Remove the stream from application state. This means GetReadStream can only be called once per file, but ensures that
            // app state remains clean

            HttpContext.Current.Application.Remove(file.ServerLocation);

            return s;
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override Stream GetWriteStream(UploadedFile file)
        {
            // Create a stream
            MemoryStream s = new MemoryStream();
            string key = Guid.NewGuid().ToString();

            // Store it for later
            file.ServerLocation = key;
            HttpContext.Current.Application[file.ServerLocation] = s;

            return s;
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override void RemoveOutput(UploadedFile file)
        {
            // Upload was cancelled or errored, so we need to remove and dispose the stream
            MemoryStream s = (MemoryStream)HttpContext.Current.Application[file.ServerLocation];

            if (s != null)
                s.Dispose();

            HttpContext.Current.Application.Remove(file.ServerLocation);
        }
    }
}
