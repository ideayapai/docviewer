using Krystalware.SlickUpload.Web.Mvc;

namespace Krystalware.SlickUpload.Web.Internal
{
    interface IFileSelector : IRenderableComponent
    {
        /// <summary>
        /// Gets or sets the <see cref="Template" /> that defines how the default mode of the <see cref="FileSelector" /> control is displayed.
        /// </summary>
        Template Template { get; }
        /// <summary>
        /// Gets or sets the <see cref="Template" /> that defines how the unskinned mode of the <see cref="FileSelector" /> control is displayed.
        /// </summary>
        Template UnskinnedTemplate { get; }
        /// <summary>
        /// Gets or sets the <see cref="Template" /> that defines how the folder selector mode of the <see cref="FileSelector" /> control is displayed.
        /// </summary>
        Template FolderTemplate { get; }
        /// <summary>
        /// Gets or sets the <see cref="Template" /> that defines how the unsupported mode of the <see cref="FileSelector" /> control is displayed.
        /// </summary>
        Template UnsupportedTemplate { get; }
        /// <summary>
        /// Gets or sets the <see cref="Template" /> that defines how the dropzone mode of the <see cref="FileSelector" /> control is displayed.
        /// </summary>
        Template DropZoneTemplate { get; }

        /// <summary>
        /// Gets or sets the ID of the external DOM element to use as a file drop zone for this <see cref="FileSelector" />.
        /// </summary>
        string DropZoneId { get; }
        /// <summary>
        /// Gets or sets the ID of the <see cref="UploadConnector" /> associated with this <see cref="FileSelector" />.
        /// </summary>
        string UploadConnectorId { get; }
        /// <summary>
        /// Gets or sets the maximum number of files that can be selected, or null for unlimited.
        /// </summary>
        /// <default-value>100</default-value>
        int? MaxFiles { get; }
        /// <summary>
        /// Gets or sets the maximum individual file size (in KB), or null for unlimited.
        /// </summary>
        /// <default-value>2097140</default-value>
        int? MaxFileSize { get; }
        /// <summary>
        /// Gets or sets a comma seperated list of valid extensions, or null for all extensions.
        /// </summary>
        string ValidExtensions { get; }
        /// <summary>
        /// Gets or sets a boolean that specifies whether the control will render in skinned mode if the browser supports it.
        /// </summary>
        /// <default-value>true</default-value>
        bool? IsSkinned { get; }
        /// <summary>
        /// Gets or sets a boolean that specifies whether to show the dropzone on document mouseover and hide it on document mouseout.
        /// </summary>
        /// <default-value>false</default-value>
        bool? ShowDropZoneOnDocumentDragOver { get; }
        /// <summary>
        /// Gets or sets the tag name of the container element to generate for the entire file selector.
        /// </summary>
        /// <default-value>div</default-value>
        string ContainerTagName { get; }

        /// <summary>
        /// Gets or sets a javascript function to call before a file is added. Return false from this function to cancel the addition.
        /// The file size may or may not be known at this point, depending on browser support.
        /// </summary>
        string OnClientFileAdding { get; }
        /// <summary>
        /// Gets or sets a javascript function to call when a file is added.
        /// </summary>
        string OnClientFileAdded { get; }
        /// <summary>
        /// Gets or sets a javascript function to call when a file is validated. This may occur multiple times for a file if it requires a size calculation request.
        /// </summary>
        string OnClientFileValidated { get; }
        /// <summary>
        /// Gets or sets a javascript function to call when a file is removed.
        /// </summary>
        string OnClientFileRemoved { get; }

    }
}
