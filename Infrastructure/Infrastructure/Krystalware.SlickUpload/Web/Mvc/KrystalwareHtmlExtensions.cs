#if NET35
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.IO;
using System.Reflection;
using Krystalware.SlickUpload.Web.Internal;

namespace Krystalware.SlickUpload.Web.Mvc
{
    /// <summary>
    /// Represents support for rendering Krystalware components to Mvc/Razor view engines. Contains extension methods that extend the <see cref="HtmlHelper" /> class.
    /// </summary>
    [Obfuscation(Feature = "hiding-place")]
    public static class KrystalwareHtmlExtensions
    {
        /// <summary>
        /// Renders the specified <see cref="MvcComponentBase" /> to the current WebForms view.
        /// </summary>
        /// <param name="helper">The HTML helper instance that this method extends.</param>
        /// <param name="component">The <see cref="MvcComponentBase" /> to render.</param>
        public static void KrystalwareWebForms(this HtmlHelper helper, MvcComponentBase component)
        {
            IComponentRenderer renderer = RendererFactory.GetRenderer(component);

            renderer.Register(component);

            using (HtmlTextWriter w = new HtmlTextWriter(helper.ViewContext.Writer))
                renderer.Render(component, w);
        }

        /// <summary>
        /// Renders the specified <see cref="KrystalwareScriptRenderer" /> to the current WebForms view.
        /// </summary>
        /// <param name="helper">The HTML helper instance that this method extends.</param>
        /// <param name="renderer">The <see cref="KrystalwareScriptRenderer" /> to render.</param>
        public static void KrystalwareWebForms(this HtmlHelper helper, KrystalwareScriptRenderer renderer)
        {
            using (HtmlTextWriter w = new HtmlTextWriter(helper.ViewContext.Writer))
                renderer.Render(w);
        }

        /// <summary>
        /// Renders the specified <see cref="MvcComponentBase" /> and returns the output for a Razor view.
        /// </summary>
        /// <param name="helper">The HTML helper instance that this method extends.</param>
        /// <param name="component">The <see cref="MvcComponentBase" /> to render.</param>
        public static MvcHtmlString KrystalwareRazor(this HtmlHelper helper, MvcComponentBase component)
        {          
            IComponentRenderer renderer = RendererFactory.GetRenderer(component);

            renderer.Register(component);

            using (StringWriter s = new StringWriter())
            {
                using (HtmlTextWriter w = new HtmlTextWriter(s))
                    renderer.Render(component, w);

                return MvcHtmlString.Create(s.ToString());
            }
        }

        /// <summary>
        /// Renders the specified <see cref="KrystalwareScriptRenderer" /> and returns the output for a Razor view.
        /// </summary>
        /// <param name="helper">The HTML helper instance that this method extends.</param>
        /// <param name="renderer">The <see cref="KrystalwareScriptRenderer" /> to render.</param>
        public static MvcHtmlString KrystalwareRazor(this HtmlHelper helper, KrystalwareScriptRenderer renderer)
        {
            using (StringWriter s = new StringWriter())
            {
                using (HtmlTextWriter w = new HtmlTextWriter(s))
                    renderer.Render(w);

                return MvcHtmlString.Create(s.ToString());
            }
        }
    }
}
#endif