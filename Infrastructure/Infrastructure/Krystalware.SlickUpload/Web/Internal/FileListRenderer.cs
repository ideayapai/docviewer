using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Krystalware.SlickUpload.Web.Internal
{
    class FileListRenderer : ComponentRendererBase<IFileList>
    {
        public override void Render(IFileList list, HtmlTextWriter w)
        {
            list.AddAttributesToRender(w, "su-filelist", list.Parent is ISlickUpload ? "clear:both" : null);

            w.RenderBeginTag(string.IsNullOrEmpty(list.ContainerTagName) ? "div" : list.ContainerTagName);

            // TODO: require template to be specified?
            if (list.ItemTemplate != null)
            {
                w.AddAttribute("id", list.Id + "_template");

                if (!IsDesignMode(list))
                    w.AddStyleAttribute("display", "none");

                w.RenderBeginTag(string.IsNullOrEmpty(list.ItemTagName) ? "div" : list.ItemTagName);

                list.ItemTemplate.Render(w);

                w.RenderEndTag();
            }
            else
            {
                throw new Exception("FileList ItemTemplate is required.");
            }

            w.RenderEndTag();       
        }

        protected override Dictionary<string, object> GetSettings(IFileList list)
        {
            if (list.ItemTemplate == null)
                return null;

            Dictionary<string, object> settings = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(list.Id))
                throw new Exception("FileList Id is required.");
            if (string.IsNullOrEmpty(list.FileSelectorId))
                throw new Exception("FileList FileSelectorId is required.");

            settings["id"] = list.Id;
            settings["templateElement"] = list.Id + "_template";

            if (!string.IsNullOrEmpty(list.FileSelectorId))
            {
                settings["fileSelector"] = list.FileSelectorId;
            }
            else
            {
                // TODO: implement
            }

            if (!string.IsNullOrEmpty(list.InvalidFileSizeMessage))
                settings["invalidFileSizeMessage"] = list.InvalidFileSizeMessage;
            if (!string.IsNullOrEmpty(list.InvalidExtensionMessage))
                settings["invalidExtensionMessage"] = list.InvalidExtensionMessage;
            if (!string.IsNullOrEmpty(list.FileSizeFormatter))
                settings["fileSizeFormatter"] = new LiteralString(list.FileSizeFormatter);
            if (!string.IsNullOrEmpty(list.FileValidationMessageFormatter))
                settings["fileValidationMessageFormatter"] = new LiteralString(list.FileValidationMessageFormatter);

            return settings;
        }
    }
}
