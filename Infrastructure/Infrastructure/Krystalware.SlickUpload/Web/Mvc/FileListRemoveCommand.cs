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
    /// Represents a renderable file list remove command.
    /// </summary>
    public class FileListRemoveCommand : MarkerComponentBase
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
    }
}
#endif