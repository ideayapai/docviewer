using System.Web.UI;

namespace Krystalware.SlickUpload.Web.Internal
{
    interface IComponentRenderer
    {
        void Register(IRenderableComponent component);
        void Render(IRenderableComponent component, HtmlTextWriter w);
    }
}
