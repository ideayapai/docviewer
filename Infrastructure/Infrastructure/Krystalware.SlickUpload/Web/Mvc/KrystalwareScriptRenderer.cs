#if NET35
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.UI;
using Krystalware.SlickUpload.Web.Controls;
using Krystalware.SlickUpload.Web.Internal;

namespace Krystalware.SlickUpload.Web.Mvc
{
    /// <summary>
    /// Renders Krystalware client script for the current view.
    /// </summary>
    public class KrystalwareScriptRenderer
    {
        /// <summary>
        /// Gets or sets a boolean that specifies whether to render the Krystalware javascript includes or just the component script.
        /// </summary>
        /// <default-value>true</default-value>
        public bool ScriptInclude { get; set; }

        /// <summary>
        /// Renders Krystalware client script for the current view to the specified <see cref="HtmlTextWriter" />.
        /// </summary>
        /// <param name="writer">The <see cref="HtmlTextWriter" /> to which to render.</param>
        public void Render(HtmlTextWriter writer)
        {
            ComponentHelper.RenderScripts(writer, ScriptInclude);
        }

        public KrystalwareScriptRenderer()
        {
            ScriptInclude = true;
        }
    }
}
#endif