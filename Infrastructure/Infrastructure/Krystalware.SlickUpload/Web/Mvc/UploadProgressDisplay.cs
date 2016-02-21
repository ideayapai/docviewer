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
    /// Represents a renderable upload progress display control.
    /// </summary>
    public class UploadProgressDisplay : MvcComponentBase, IUploadProgressDisplay
    {
        /// <inheritdoc />
        public Template Template { get; set; }
        /// <inheritdoc />
        public string UploadConnectorId { get; set; }
        /// <inheritdoc />
        public string ContainerTagName { get; set; }
        /// <inheritdoc />
        public string FileSizeFormatter { get; set; }
        /// <inheritdoc />
        public string PercentFormatter { get; set; }
        /// <inheritdoc />
        public string TimeFormatter { get; set; }
        /// <inheritdoc />
        public bool? ShowDuringUpload { get; set; }
        /// <inheritdoc />
        public bool? HideAfterUpload { get; set; }
    }
}
#endif