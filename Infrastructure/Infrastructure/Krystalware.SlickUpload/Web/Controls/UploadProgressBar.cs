using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Drawing;

namespace Krystalware.SlickUpload.Web.Controls
{
    /// <summary>
    /// Displays an upload progress bar element.
    /// </summary>
    [
    ToolboxData(@"<{0}:UploadProgressBar runat=""server"" style=""background-color:#00ee00; width:0; height:1.5em;"" />"),
    ToolboxBitmap(typeof(UploadProgressBar), "UploadProgressBar.png"),
    Description("Displays an upload progress bar element"),
    //Designer(typeof(FileSelectorDesigner))
    ]
    public class UploadProgressBar : MarkerControlBase
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
        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (DesignMode)
                Style["width"] = "20%";

            base.AddAttributesToRender(writer);

            if (!DesignMode)
            {
                writer.AddStyleAttribute("display", "none");
            }
            /*else
            {
                writer.AddStyleAttribute("width", "20%");
            }*/
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override string DesignContent
        {
            get { return null; }
        }

    }
}
