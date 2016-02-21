#if NET35
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Linq.Expressions;
using System.Collections;
using System.ComponentModel;
using Krystalware.SlickUpload.Web.Controls;
using System.Web.UI;
using Krystalware.SlickUpload.Web.Internal;

namespace Krystalware.SlickUpload.Web.Mvc
{
    /// <summary>
    /// Represents a renderable file selector control.
    /// </summary>
    public class FileSelector : MvcComponentBase, IFileSelector
    {
        /// <inheritdoc />
        public Template Template { get; set; }
        /// <inheritdoc />
        public Template UnskinnedTemplate { get; set; }
        /// <inheritdoc />
        public Template FolderTemplate { get; set; }
        /// <inheritdoc />
        public Template UnsupportedTemplate { get; set; }
        /// <inheritdoc />
        public Template DropZoneTemplate { get; set; }
        /// <inheritdoc />
        public string ContainerTagName { get; set; }
        /// <inheritdoc />
        public string UploadConnectorId { get; set; }
        /// <inheritdoc />
        public string DropZoneId { get; set; }
        /// <inheritdoc />
        public int? MaxFiles { get; set; }
        /// <inheritdoc />
        public int? MaxFileSize { get; set; }
        /// <inheritdoc />
        public string ValidExtensions { get; set; }
        /// <inheritdoc />
        public bool? IsSkinned { get; set; }
        /// <inheritdoc />
        public bool? ShowDropZoneOnDocumentDragOver { get; set; }

        /// <inheritdoc />
        public string OnClientFileAdding { get; set; }
        /// <inheritdoc />
        public string OnClientFileAdded { get; set; }
        /// <inheritdoc />
        public string OnClientFileValidated { get; set; }
        /// <inheritdoc />
        public string OnClientFileRemoved { get; set; }
    }
}
#endif