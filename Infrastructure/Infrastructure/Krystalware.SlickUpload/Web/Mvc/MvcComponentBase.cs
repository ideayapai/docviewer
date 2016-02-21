using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Krystalware.SlickUpload.Web.Internal;

namespace Krystalware.SlickUpload.Web.Mvc
{
    /// <summary>
    /// An abstract base class that provides the core functionality for Mvc/Razor renderable elements.
    /// </summary>
    public abstract class MvcComponentBase : IRenderableComponent
    {
        /// <inheritdoc />
        public virtual string Id { get; set; }
        /// <summary>
        /// An object that contains the HTML attributes to set for the element. The parameters are retrieved through reflection by examining the properties of the object. This object is typically created by using object initializer syntax.
        /// </summary>
        public object HtmlAttributes { get; set; }

        Control IRenderableComponent.Control { get { return null; } }
        IRenderableComponent IRenderableComponent.Parent { get { return Parent; } }
        internal IRenderableComponent Parent { get; set; }

        void IRenderableComponent.AddAttributesToRender(HtmlTextWriter w, string className, string defaultStyle)
        {
            this.AddAttributesToRender(w, className, defaultStyle);
        }

        /// <inheritdoc />
        protected virtual void AddAttributesToRender(HtmlTextWriter w, string className, string defaultStyle)
        {
            ComponentHelper.AddAttributesToRender(w, Id, className, defaultStyle, HtmlAttributes);
        }

        /// <summary>
        /// Creates an instance of the <see cref="MvcComponentBase" /> class.
        /// </summary>
        protected MvcComponentBase()
        { }
    }
}
