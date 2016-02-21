using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Krystalware.SlickUpload.Web.Internal;
using Krystalware.SlickUpload.Web.Mvc;

namespace Krystalware.SlickUpload.Web.Controls
{
    /// <summary>
    /// An abstract base class that provides the core functionality for WebForms display controls.
    /// </summary>
    [ParseChildren(false)]
    public abstract class MarkerControlBase : WebControlBase, IMarkerComponent
    {
        /// <summary>
        /// Returns the marker class name to be added to the HTML class attribute.
        /// </summary>
        protected abstract string MarkerClassName { get; }

        /// <summary>
        /// Returns the placeholder content to display in design mode.
        /// </summary>
        protected abstract string DesignContent { get; }

        string IMarkerComponent.ClassName { get { return MarkerClassName; } }
        string IMarkerComponent.TagName { get { return TagName; } }
        string IMarkerComponent.DesignContent { get { return DesignContent; } }

        Template IMarkerComponent.Template
        {
            get
            {
                // TODO: check for content as well
                //if (this.HasControls())
                    return Template.CreateTemplate((HtmlTextWriter w) => this.RenderContents(w));
                //else
                //    return null;
            }
        }

        /// <summary>
        /// Creates an instance of the <see cref="MarkerControlBase" /> control with the default &lt;span&gt; container tag.
        /// </summary>
        public MarkerControlBase()
            : this("span")
        { }

        /// <summary>
        /// Creates an instance of the <see cref="MarkerControlBase" /> control with the specified container tag name.
        /// </summary>
        /// <param name="tag">The tag name to use.</param>
        public MarkerControlBase(string tag)
            : base(tag)
        { }
    }
}
