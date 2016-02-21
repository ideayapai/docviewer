using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using Krystalware.SlickUpload.Web.Internal;
using Krystalware.SlickUpload.Web.Mvc;

namespace Krystalware.SlickUpload.Web.Mvc
{
    /// <summary>
    /// An abstract base class that provides the core functionality for Mvc/Razor renderable display elements.
    /// </summary>
    [ParseChildren(false)]
    public abstract class MarkerComponentBase : MvcComponentBase, IMarkerComponent
    {
        string _tagName;

        /// <summary>
        /// Returns the marker class name to be added to the HTML class attribute.
        /// </summary>
        protected abstract string MarkerClassName { get; }
        string IMarkerComponent.ClassName { get { return MarkerClassName; } }
        string IMarkerComponent.TagName { get { return _tagName; } }
        string IMarkerComponent.DesignContent { get { return null; } }

        /// <summary>
        /// Gets or sets the <see cref="Template" /> to render for this display element.
        /// </summary>
        public Template Template { get; set; }  

        /// <summary>
        /// Creates an instance of the <see cref="MarkerComponentBase" /> contained with the default &lt;span&gt; container tag.
        /// </summary>
        protected MarkerComponentBase()
            : this("span")
        { }

        /// <summary>
        /// Creates an instance of the <see cref="MarkerComponentBase" /> with the specified tag name.
        /// </summary>
        /// <param name="tag">The name of the containing tag.</param>
        protected MarkerComponentBase(string tag)
        {
            _tagName = tag;
        }
    }
}
