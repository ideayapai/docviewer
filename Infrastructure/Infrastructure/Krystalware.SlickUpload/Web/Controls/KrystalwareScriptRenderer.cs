using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Drawing;
using Krystalware.SlickUpload.Web.Internal;

namespace Krystalware.SlickUpload.Web.Controls
{
    /// <summary>
    /// Renders Krystalware client script for the current page.
    /// </summary>
    [
    ToolboxData(@"<{0}:KrystalwareScriptRenderer runat=""server"" />"),
    Description("Renders Krystalware client script for the current page."),
    //Designer(typeof(FileSelectorDesigner))
    ]
    public class KrystalwareScriptRenderer : Control
    {
        /// <summary>
        /// Gets or sets a boolean that specifies whether to render the Krystalware javascript includes or just the component script.
        /// </summary>
        /// <default-value>true</default-value>
        public bool ScriptInclude { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override void Render(HtmlTextWriter writer)
        {
            if (ComponentHelper.GetParentUpdatePanel(this) != null)
                throw new InvalidOperationException("KrystalwareScriptRenderer is not valid within an UpdatePanel.");

            ComponentHelper.RenderScripts(writer, ScriptInclude);
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
            if (Visible && ScriptInclude)
                ComponentHelper.HasScriptRendererWithIncludeScripts = true;

            base.OnPreRender(e);
        }

        public KrystalwareScriptRenderer()
        {
            ScriptInclude = true;
        }
    }
}
