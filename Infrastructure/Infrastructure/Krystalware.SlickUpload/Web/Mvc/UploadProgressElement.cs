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

namespace Krystalware.SlickUpload.Web.Mvc
{
    /// <summary>
    /// Represents a renderable upload progress display element.
    /// </summary>
    public class UploadProgressElement : MarkerComponentBase
    {
        /// <summary>
        /// Gets or sets the <see cref="UploadProgressElementType" /> to display.
        /// </summary>
        public UploadProgressElementType Element { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override string MarkerClassName
        {
            get { return "su-" + Element.ToString().ToLower(); }
        }
    }
}
#endif