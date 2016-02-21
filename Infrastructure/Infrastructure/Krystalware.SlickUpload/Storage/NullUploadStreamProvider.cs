using System;
using System.IO;
using System.Reflection;
using System.Web;

using Krystalware.SlickUpload.Configuration;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Hosting;

namespace Krystalware.SlickUpload.Storage
{
	/// <summary>
    /// An <see cref="IUploadStreamProvider" /> that writes to <see cref="Stream.Null" />.
	/// </summary>
	public class NullUploadStreamProvider : UploadStreamProviderBase
	{
        /// <summary>
        /// Creates a new instance of the <see cref="NullUploadStreamProvider" /> class with the specified configuration settings.
        /// </summary>
        /// <param name="settings">The <see cref="UploadStreamProviderElement" /> object that holds the configuration settings.</param>
        public NullUploadStreamProvider(UploadStreamProviderElement settings)
            : base(settings)
		{ }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override Stream GetWriteStream(UploadedFile file)
        {
            file.ServerLocation = file.ClientName;

            return Stream.Null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override void RemoveOutput(UploadedFile file)
        { }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override Stream GetReadStream(UploadedFile file)
        {
            return Stream.Null;
        }
    }
}