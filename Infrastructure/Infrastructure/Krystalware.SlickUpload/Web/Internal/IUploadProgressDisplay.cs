using Krystalware.SlickUpload.Web.Mvc;

namespace Krystalware.SlickUpload.Web.Internal
{
    interface IUploadProgressDisplay : IRenderableComponent
    {
        /// <summary>
        /// Gets or sets the <see cref="Template" /> that defines how the <see cref="UploadProgressDisplay" /> control is displayed.
        /// </summary>
        Template Template { get; }
        /// <summary>
        /// Gets or sets the ID of the <see cref="UploadConnector" /> associated with this <see cref="UploadProgressDisplay" />.
        /// </summary>
        string UploadConnectorId { get; }
        /// <summary>
        /// Gets or sets the tag name of the container element to generate for the entire upload progress display.
        /// </summary>
        string ContainerTagName { get; }
        /// <summary>
        /// Gets or sets a reference to the client side file size formatter function to use.
        /// </summary>
        /// <default-value>"kw.defaultFileSizeFormatter"</default-value>
        string FileSizeFormatter { get; }
        /// <summary>
        /// Gets or sets a reference to the client side percent formatter function to use.
        /// </summary>
        /// <default-value>"kw.defaultPercentFormatter"</default-value>
        string PercentFormatter { get; }
        /// <summary>
        /// Gets or sets a reference to the client side time formatter function to use.
        /// </summary>
        /// <default-value>"kw.defaultTimeFormatter"</default-value>
        string TimeFormatter { get; }
        /// <summary>
        /// Gets or sets a boolean that specifies whether to automatically show the upload progress display during an upload.
        /// </summary>
        /// <default-value>true</default-value>
        bool? ShowDuringUpload { get; }
        /// <summary>
        /// Gets or sets a boolean that specifies whether to automatically hide the upload progress display after an upload.
        /// </summary>
        /// <default-value>true</default-value>
        bool? HideAfterUpload { get; }
    }
}
