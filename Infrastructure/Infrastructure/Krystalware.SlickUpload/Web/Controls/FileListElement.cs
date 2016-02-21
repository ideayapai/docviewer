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
    /// Displays a file list element.
    /// </summary>
    [
    ToolboxData(@"<{0}:FileListElement runat=""server"" />"),
    ToolboxBitmap(typeof(FileListElement), "FileListElement.png"),
    Description("Displays a file list element"),
    //Designer(typeof(FileSelectorDesigner))
    ]
    public class FileListElement : MarkerControlBase
    {
        /// <see cref="Krystalware.SlickUpload.Web.Mvc.FileListElement.Element" copy="true" />
        public FileListElementType Element
        {
            get
            {
                object elementObject = ViewState["FileListElementType"];

                if (elementObject != null)
                    return (FileListElementType)elementObject;
                else
                    return FileListElementType.FileName; 
            }
            set
            {
                ViewState["FileListElementType"] = value;
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
