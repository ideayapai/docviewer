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
    /// Represents a renderable upload progress bar display element.
    /// </summary>
    public class UploadProgressBar : MarkerComponentBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="UploadProgressBar" /> class.
        /// </summary>
        public UploadProgressBar()
            : base("div")
        { }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override string MarkerClassName
        {
            get { return "su-progressbar"; }
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override void AddAttributesToRender(HtmlTextWriter writer, string className, string defaultStyle)
        {
            base.AddAttributesToRender(writer, className, defaultStyle);

            writer.AddStyleAttribute("display", "none");
        }
    }
}
#endif