using System;
using System.Collections.Generic;
using System.Text;
using Krystalware.SlickUpload.Web;

namespace Krystalware.SlickUpload
{
    /// <summary>
    /// Represents a contract for upload filtering.
    /// </summary>
    public interface IUploadFilter
    {
        /// <summary>
        /// Given an <see cref="UploadHttpRequest" />, returns a boolean that specifies whether SlickUpload should handle it.
        /// Return true to process the request, false to skip it. To abort and show an error, throw an exception.
        /// </summary>
        /// <param name="request">The <see cref="UploadHttpRequest" /> to filter.</param>
        /// <returns>A boolean that specifies whether SlickUpload should handle the specified <see cref="UploadHttpRequest" />.</returns>
        bool ShouldHandleRequest(UploadHttpRequest request);

        /// <summary>
        /// Given an <see cref="UploadHttpRequest" /> and <see cref="UploadedFile" />, returns a boolean that specifies whether to process the file or skip it.
        /// Return true to process the file, false to skip it. To abort and show an error, throw an exception.
        /// </summary>
        /// <param name="request">The <see cref="UploadHttpRequest" /> to filter.</param>
        /// <param name="file">The <see cref="UploadedFile" /> to filter.</param>
        /// <returns>A boolean that specifies whether SlickUpload should process the specified <see cref="UploadedFile" />.</returns>
        bool ShouldHandleFile(UploadHttpRequest request, UploadedFile file);
    }
}
