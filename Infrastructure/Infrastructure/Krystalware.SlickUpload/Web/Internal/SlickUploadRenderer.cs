using System.Collections.Generic;
using System.Web.UI;

namespace Krystalware.SlickUpload.Web.Internal
{
    class SlickUploadRenderer : ComponentRendererBase<ISlickUpload>
    {
        public override void Render(ISlickUpload component, HtmlTextWriter w)
        {
            component.AddAttributesToRender(w, "su-slickupload", "overflow:hidden;zoom:1");
            w.RenderBeginTag("div");

            // TODO: validate that things are populated
            RendererFactory.GetRenderer(component.FileSelector).Render(component.FileSelector, w);
            if (component.FileList != null)
                RendererFactory.GetRenderer(component.FileList).Render(component.FileList, w);
            if (component.UploadProgressDisplay != null)
                RendererFactory.GetRenderer(component.UploadProgressDisplay).Render(component.UploadProgressDisplay, w);
            RendererFactory.GetRenderer(component.UploadConnector).Render(component.UploadConnector, w);

            w.RenderEndTag();
        }

        public override void Register(ISlickUpload component)
        {
            if (component.Control == null)
            {
                RendererFactory.GetRenderer(component.FileSelector).Register(component.FileSelector);
                if (component.FileList != null)
                    RendererFactory.GetRenderer(component.FileList).Register(component.FileList);
                if (component.UploadProgressDisplay != null)
                    RendererFactory.GetRenderer(component.UploadProgressDisplay).Register(component.UploadProgressDisplay);
                RendererFactory.GetRenderer(component.UploadConnector).Register(component.UploadConnector);
            }

            base.Register(component);
        }

        protected override Dictionary<string, object> GetSettings(ISlickUpload slickUpload)
        {
            Dictionary<string, object> settings = new Dictionary<string, object>();

            settings["id"] = slickUpload.Id;
            settings["fileSelector"] = slickUpload.FileSelector.Id;
            if (slickUpload.FileList != null)
                settings["fileList"] = slickUpload.FileList.Id;
            if (slickUpload.UploadProgressDisplay != null)
                settings["uploadProgressDisplay"] = slickUpload.UploadProgressDisplay.Id;
            settings["uploadConnector"] = slickUpload.UploadConnector.Id;

            return settings;
        }
    }
}
