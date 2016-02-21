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
    /// Represents a renderable file list control.
    /// </summary>
    public class FileList : MvcComponentBase, IFileList
    {
        /// <inheritdoc />
        public Template ItemTemplate { get; set; }
        /// <inheritdoc />
        public string FileSelectorId { get; set; }
        /// <inheritdoc />
        public string InvalidFileSizeMessage { get; set; }
        /// <inheritdoc />
        public string InvalidExtensionMessage { get; set; }
        /// <inheritdoc />
        public string FileSizeFormatter { get; set; }
        /// <inheritdoc />
        public string FileValidationMessageFormatter { get; set; }
        /// <inheritdoc />
        public string ContainerTagName { get; set; }
        /// <inheritdoc />
        public string ItemTagName { get; set; }
    }
}
#endif