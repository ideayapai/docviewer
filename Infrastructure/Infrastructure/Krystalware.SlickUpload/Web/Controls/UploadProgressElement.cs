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
    /// Displays an upload progress element.
    /// </summary>
    [
    ToolboxData(@"<{0}:UploadProgressElement runat=""server"" />"),
    ToolboxBitmap(typeof(UploadProgressElement), "UploadProgressElement.png"),
    Description("Displays an upload progress element"),
    //Designer(typeof(FileSelectorDesigner))
    ]
    public class UploadProgressElement : MarkerControlBase
    {
        /// <see cref="Krystalware.SlickUpload.Web.Mvc.UploadProgressElement.Element" copy="true" />
        public UploadProgressElementType Element
        {
            get
            {
                object elementObject = ViewState["UploadProgressElementType"];

                if (elementObject != null)
                    return (UploadProgressElementType)elementObject;
                else
                    return UploadProgressElementType.CurrentFileName;
            }
            set
            {
                ViewState["UploadProgressElementType"] = value;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override string MarkerClassName
        {
            get { return "su-" + Element.ToString().ToLower(); }
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override string DesignContent
        {
            get { return Element.ToString(); }
        }

    }
}
