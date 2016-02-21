using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Krystalware.SlickUpload.Web.Internal;

namespace Krystalware.SlickUpload.Web.Controls
{
    /// <summary>
    /// An abstract base class that provides the core functionality for WebForms controls.
    /// </summary>
    public abstract class WebControlBase : WebControl, IRenderableComponent
    {
        /// <summary>
        /// Creates an instance of the <see cref="WebControlBase" /> control with a the default (&lt;div&gt;) container tag.
        /// </summary>
        public WebControlBase()
        { }

        /// <summary>
        /// Creates an instance of the <see cref="WebControlBase" /> control with the specified container tag name.
        /// </summary>
        /// <param name="tag">The tag name to use.</param>
        public WebControlBase(string tag)
            : base(tag)
        { }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            if (!typeof(MarkerControlBase).IsAssignableFrom(this.GetType()))
                ComponentHelper.AddToRegistry(this);
        }
        
        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
            RendererFactory.GetRenderer(this).Register(this);
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override void Render(HtmlTextWriter writer)
        {
            RendererFactory.GetRenderer(this).Render(this, writer);
        }

        string IRenderableComponent.Id
        {
            get { return ClientID; }
        }
        
        void IRenderableComponent.AddAttributesToRender(HtmlTextWriter w, string className, string defaultStyle)
        {
            ComponentHelper.AddCssClass(this, className);

            AddAttributesToRender(w);
        }

        Control IRenderableComponent.Control
        {
            get { return this; }
        }

        IRenderableComponent IRenderableComponent.Parent { get { return Parent as IRenderableComponent; } }
    }
}