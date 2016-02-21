using Krystalware.SlickUpload.Web.Mvc;

namespace Krystalware.SlickUpload.Web.Internal
{
    interface IFileList : IRenderableComponent
    {
        /// <summary>
        /// Gets or sets the <see cref="Template" /> to use for file list items.
        /// </summary>
        Template ItemTemplate { get; }
        /// <summary>
        /// Gets or sets the ID of the <see cref="FileSelector" /> associated with this <see cref="FileList" />.
        /// </summary>
        string FileSelectorId { get; }
        /// <summary>
        /// Gets or sets the message to display for invalid file sizes.
        /// </summary>
        /// <default-value>"File is too large."</default-value>
        string InvalidFileSizeMessage { get; }
        /// <summary>
        /// Gets or sets the message to display for invalid file extensions.
        /// </summary>
        /// <default-value>"Invalid file extension."</default-value>
        string InvalidExtensionMessage { get; }
        /// <summary>
        /// Gets or sets a reference to the client side file size formatter function to use.
        /// </summary>
        /// <default-value>"kw.defaultFileSizeFormatter"</default-value>
        string FileSizeFormatter { get; }
        /// <summary>
        /// Gets or sets a reference to the client side file validation message formatter function to use.
        /// </summary>
        /// <default-value>"kw.defaultFileValidationMessageFormatter"</default-value>
        string FileValidationMessageFormatter { get; }
        /// <summary>
        /// Gets or sets the tag name of the container element to generate for the entire file list.
        /// </summary>
        /// <default-value>"div"</default-value>
        string ContainerTagName { get; }
        /// <summary>
        /// Gets or sets the tag name of the container element to generate for each file item.
        /// </summary>
        /// <default-value>"div"</default-value>
        string ItemTagName { get; }
    }
}
