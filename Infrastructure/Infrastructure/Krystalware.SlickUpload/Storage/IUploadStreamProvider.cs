using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Krystalware.SlickUpload.Storage
{
    /// <summary>
    /// Represents a provider that can be called to get a <see cref="Stream" /> to which to write an <see cref="UploadedFile" />.
    /// </summary>
    public interface IUploadStreamProvider
    {
        /// <summary>
        /// Returns a <see cref="Stream" /> to which to write the file as it is uploaded. The provider must store appropriate information in the <see cref="UploadedFile"/>.ServerLocation or <see cref="UploadedFile"/>.Data property to allow it
        /// to implement the RemoveOutput and GetReadStream methods.
        /// </summary>
        /// <param name="file">The  <see cref="UploadedFile" /> for which to get a <see cref="Stream" />.</param>
        /// <returns>A <see cref="Stream" /> to which to write the <see cref="UploadedFile" />.</returns>
        Stream GetWriteStream(UploadedFile file);

        /// <summary>
        /// Closes an output stream. Should only be called by the SlickUpload framework.
        /// </summary>
        /// <param name="file">The <see cref="UploadedFile" /> for which to close the <see cref="Stream" />.</param>
        /// <param name="stream">The <see cref="Stream" /> to close.</param>
        /// <param name="isComplete">A boolean that specifies whether this file was completely uploaded or aborted midstream (i.e. upload request is terminating).</param>
        void CloseWriteStream(UploadedFile file, Stream stream, bool isComplete);

        /// <summary>
        /// Removes the output file or record for an upload. Should only be called by the SlickUpload framework. The CloseWriteStream
        /// method is guaranteed to have been called before this method is called.
        /// </summary>
        /// <param name="file">The <see cref="UploadedFile" /> for which to remove the output.</param>
        void RemoveOutput(UploadedFile file);

        /// <summary>
        /// Returns a <see cref="Stream" /> of the file data that was uploaded for the specified <see cref="UploadedFile" />.
        /// </summary>
        /// <param name="file">The  <see cref="UploadedFile" /> for which to get a <see cref="Stream" />.</param>
        /// <returns>A <see cref="Stream" /> of the file data that was uploaded.</returns>
        Stream GetReadStream(UploadedFile file);
    }
}
