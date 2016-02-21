using Krystalware.SlickUpload.Web.Mvc;

namespace Krystalware.SlickUpload.Web.Internal
{
    interface IMarkerComponent : IRenderableComponent
    {
        string ClassName { get; }
        string TagName { get; }
        /// <summary>
        /// Returns the placeholder content to display in design mode.
        /// </summary>
        string DesignContent { get; }

        Template Template { get; }
    }
}
