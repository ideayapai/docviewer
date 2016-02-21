using System.Web.UI;

namespace Krystalware.SlickUpload.Web.Internal
{
    interface IRenderableComponent
    {
        /// <summary>
        /// Gets or sets the HTML Id attribute for this component.
        /// </summary>
        string Id { get; }
        Control Control { get; }
        IRenderableComponent Parent { get; }

        /// <summary>
        /// Adds the current attributes to the specified <see cref="HtmlTextWriter" />.
        /// </summary>
        /// <param name="w">The <see cref="HtmlTextWriter" /> to which to add attributes.</param>
        /// <param name="className">The HTML class attribute to use.</param>
        /// <param name="existingStyle">The existing HTML style attribute string.</param>
        void AddAttributesToRender(HtmlTextWriter w, string className, string existingStyle);
    }
}
