using System.Web.UI;

namespace Krystalware.SlickUpload.Web.Internal
{
    class ScriptElement
    {
        public int RenderOrder { get; private set; }
        public string Script { get; private set; }
        public Control Control { get; private set; }

        public ScriptElement(Control control, string script, int renderOrder)
        {
            Script = script;
            RenderOrder = renderOrder;
        }
    }
}
