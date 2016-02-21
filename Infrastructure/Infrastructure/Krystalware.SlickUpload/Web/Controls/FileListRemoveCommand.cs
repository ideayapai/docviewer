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
    /// Displays a file list remove command.
    /// </summary>
    [
    ToolboxData(@"<{0}:FileListRemoveCommand runat=""server"" href=""javascript:;"">[x]</{0}:FileListRemoveCommand>"),
    ToolboxBitmap(typeof(FileListRemoveCommand), "FileListRemoveCommand.png"),
    Description("Displays a file list remove command"),
    //Designer(typeof(FileSelectorDesigner))
    ]
    public class FileListRemoveCommand : MarkerControlBase
    {
        /// <summary>
        /// Creates a new instance of the <see cref="FileListRemoveCommand" /> class.
        /// </summary>
        public FileListRemoveCommand()
            : base("a")
        { }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override string MarkerClassName
        {
            get { return "su-removecommand"; }
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
