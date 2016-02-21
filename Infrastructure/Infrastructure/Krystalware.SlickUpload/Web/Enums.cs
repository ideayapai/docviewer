using System;
using System.Collections.Generic;
using System.Text;

namespace Krystalware.SlickUpload.Web
{
    /// <summary>
    /// An enumeration of the possible file elements a FileList control.
    /// </summary>
    public enum FileListElementType
    {
        /// <summary>
        /// File name.
        /// </summary>
        FileName,
        /// <summary>
        /// File size.
        /// </summary>
        FileSize,
        /// <summary>
        /// File validation message.
        /// </summary>
        ValidationMessage
    }

    /// <summary>
    /// An enumeration of the possible progress elements that can be displayed in an UploadProgressElement control.
    /// </summary>
    public enum UploadProgressElementType
    {
        /// <summary>
        /// The name of the file currently being uploaded.
        /// </summary>
        CurrentFileName,
        /// <summary>
        /// The one-based index of the file being uploaded.
        /// </summary>
        CurrentFileIndex,
        /// <summary>
        /// Count of the files being uploaded.
        /// </summary>
        FileCount,
        /// <summary>
        /// Content length text of the files being uploaded.
        /// </summary>
        ContentLengthText,
        /// <summary>
        /// Current percent complete text.
        /// </summary>
        PercentCompleteText,
        /// <summary>
        /// Current speed text.
        /// </summary>
        SpeedText,
        /// <summary>
        /// Current time remaining text.
        /// </summary>
        TimeRemainingText
    }

    /// <summary>
    /// Defines the potential locations for the community edition branding.
    /// </summary>
    public enum BrandLocation
    {
        /// <summary>
        /// Apply the branding inline, after each <see cref="FileSelector"/>.
        /// </summary>
        Inline,
        /// <summary>
        /// Apply the branding to the bottom right of the page.
        /// </summary>
        BottomRight
    }
}
