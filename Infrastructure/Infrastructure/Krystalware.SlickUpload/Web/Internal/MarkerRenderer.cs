using System.Collections.Generic;
using System.Web.UI;
using Krystalware.SlickUpload.Web.Mvc;

namespace Krystalware.SlickUpload.Web.Internal
{
    class MarkerComponentRenderer : ComponentRendererBase<IMarkerComponent>
    {
        public override void Render(IMarkerComponent marker, HtmlTextWriter w)
        {
            marker.AddAttributesToRender(w, marker.ClassName, null);

            w.RenderBeginTag(marker.TagName); 
            
            Template template = marker.Template;

            if (!IsDesignMode(marker) || string.IsNullOrEmpty(marker.DesignContent))
            {
                if (template != null)
                    template.Render(w);
            }
            else if (!string.IsNullOrEmpty(marker.DesignContent))
            {
                w.Write("(" + marker.DesignContent + ")");
            }
            
            w.RenderEndTag();
        }

        protected override Dictionary<string, object> GetSettings(IMarkerComponent uploadConnector)
        {
            //Dictionary<string, object> settings = new Dictionary<string, object>();

            //return settings;
            return null;
        }
    }
}