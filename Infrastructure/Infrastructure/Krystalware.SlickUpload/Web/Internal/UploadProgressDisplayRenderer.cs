using System.Collections.Generic;
using System.Web.UI;

namespace Krystalware.SlickUpload.Web.Internal
{
    class UploadProgressDisplayRenderer : ComponentRendererBase<IUploadProgressDisplay>
    {
        public override void Render(IUploadProgressDisplay display, HtmlTextWriter w)
        {
            display.AddAttributesToRender(w, "su-uploadprogressdisplay", display.Parent is ISlickUpload ? "clear:both" : null);
            if (!IsDesignMode(display))
                w.AddStyleAttribute("display", "none");

            w.RenderBeginTag(string.IsNullOrEmpty(display.ContainerTagName) ? "div" : display.ContainerTagName);

            if (display.Template != null)
                display.Template.Render(w);

            w.RenderEndTag();       
        }

        protected override Dictionary<string, object> GetSettings(IUploadProgressDisplay display)
        {
            if (display.Template == null)
                return null;

            Dictionary<string, object> settings = new Dictionary<string, object>();

            settings["id"] = display.Id;

            if (!string.IsNullOrEmpty(display.UploadConnectorId))
            {
                settings["uploadConnector"] = display.UploadConnectorId;
            }
            else
            {
                // TODO: implement
            }

            if (!string.IsNullOrEmpty(display.FileSizeFormatter))
                settings["fileSizeFormatter"] = new LiteralString(display.FileSizeFormatter);
            if (!string.IsNullOrEmpty(display.PercentFormatter))
                settings["percentFormatter"] = new LiteralString(display.PercentFormatter);
            if (!string.IsNullOrEmpty(display.TimeFormatter))
                settings["timeFormatter"] = new LiteralString(display.TimeFormatter);
            if (display.ShowDuringUpload != null)
                settings["showDuringUpload"] = display.ShowDuringUpload;
            if (display.HideAfterUpload != null)
                settings["hideAfterUpload"] = display.HideAfterUpload;

            return settings;
        }
    }
}
