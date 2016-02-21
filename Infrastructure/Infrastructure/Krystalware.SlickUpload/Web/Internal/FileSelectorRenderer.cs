using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Krystalware.SlickUpload.Web.Internal
{
    class FileSelectorRenderer : ComponentRendererBase<IFileSelector>
    {
        public override void Render(IFileSelector selector, HtmlTextWriter w)
        {
            if (selector.Template != null)
            {
                if (!IsDesignMode(selector))
                    w.AddStyleAttribute("display", "none");
                selector.AddAttributesToRender(w, "su-fileselector", selector.Parent is ISlickUpload ? "float:left" : null);

                w.RenderBeginTag(string.IsNullOrEmpty(selector.ContainerTagName) ? "div" : selector.ContainerTagName);

                selector.Template.Render(w);

                w.RenderEndTag();
            }

            if (selector.FolderTemplate != null)
            {
                w.AddAttribute("id", selector.Id + "_folder");
                w.AddAttribute("class", "su-folderselector");
                if (!IsDesignMode(selector))
                    w.AddStyleAttribute("display", "none");

                if (selector.Parent is ISlickUpload)
                    w.AddStyleAttribute("float", "left");

                w.RenderBeginTag(string.IsNullOrEmpty(selector.ContainerTagName) ? "div" : selector.ContainerTagName);

                selector.FolderTemplate.Render(w);

                w.RenderEndTag();
            }
            
            if (selector.UnskinnedTemplate != null)
            {
                if (!IsDesignMode(selector))
                    w.AddStyleAttribute("display", "none");

                if (selector.Template != null)
                {
                    w.AddAttribute("id", selector.Id + "_unskinned");
                    w.AddAttribute("class", "su-fileselector su-unskinned");
                }
                else
                    selector.AddAttributesToRender(w, "su-fileselector su-unskinned", null);

                w.RenderBeginTag("div");

                selector.UnskinnedTemplate.Render(w);

                w.RenderEndTag();
            }

            if (selector.Template == null && selector.UnskinnedTemplate == null)
            {
                if (!IsDesignMode(selector))
                    w.AddStyleAttribute("display", "none");

                selector.AddAttributesToRender(w, "su-fileselector su-unskinned", null);

                w.RenderBeginTag("div");

                w.AddAttribute("type", "file");
                w.RenderBeginTag("input");
                w.RenderEndTag();

                w.RenderEndTag();
            }

            if (selector.DropZoneTemplate != null)
            {
                w.AddAttribute("id", selector.Id + "_dropzone");
                w.AddAttribute("class", "su-dropzone");

                if (!IsDesignMode(selector))
                    w.AddStyleAttribute("display", "none");

                w.RenderBeginTag("div");

                selector.DropZoneTemplate.Render(w);

                w.RenderEndTag();
            }
            
            if (selector.UnsupportedTemplate != null)
            {
                if (!IsDesignMode(selector))
                    w.AddStyleAttribute("display", "none");

                w.AddAttribute("id", selector.Id + "_unsupported");
                w.AddAttribute("class", "su-fileselector su-unsupported");

                w.RenderBeginTag("div");

                selector.UnsupportedTemplate.Render(w);

                w.RenderEndTag();
            }            
        }

        public override void Register(IFileSelector component)
        {
            if (component.DropZoneTemplate != null && component.ShowDropZoneOnDocumentDragOver != true)
            {
                ComponentHelper.RegisterInitStatement("if (kw.support.dragDrop) { document.getElementById(\"" + component.Id + "_dropzone\").style.display = \"block\"; }");
            }

            base.Register(component);
        }

        protected override Dictionary<string, object> GetSettings(IFileSelector selector)
        {
            Dictionary<string, object> settings = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(selector.Id))
                throw new Exception("FileSelector Id is required.");
            if (string.IsNullOrEmpty(selector.UploadConnectorId))
                throw new Exception("FileSelector UploadConnectorId is required.");

            settings["id"] = selector.Id;

            if (!string.IsNullOrEmpty(selector.UploadConnectorId))
            {
                settings["uploadConnector"] = selector.UploadConnectorId;
            }
            else
            {
                // TODO: implement
            }

            if (selector.UnskinnedTemplate != null || selector.Template == null)
                settings["unskinnedElement"] = selector.Template == null ? selector.Id : selector.Id + "_unskinned";

            if (selector.FolderTemplate != null)
                settings["folderElement"] = selector.Id + "_folder";

            if (!string.IsNullOrEmpty(selector.DropZoneId))
                settings["dropZone"] = selector.DropZoneId;
            else if (selector.DropZoneTemplate != null)
                settings["dropZone"] = selector.Id + "_dropzone";

            if (selector.UnsupportedTemplate != null)
                settings["unsupportedElement"] = selector.Id + "_unsupported";

            if (selector.MaxFiles != null)
                settings["maxFiles"] = selector.MaxFiles.Value;
            if (selector.MaxFileSize != null)
                settings["maxFileSize"] = selector.MaxFileSize.Value;
            if (!string.IsNullOrEmpty(selector.ValidExtensions))
                settings["validExtensions"] = selector.ValidExtensions;
            if (selector.Template == null)
                settings["isSkinned"] = false;
            else if (selector.IsSkinned != null)
                settings["isSkinned"] = selector.IsSkinned.Value;
            if (selector.ShowDropZoneOnDocumentDragOver != null)
                settings["showDropZoneOnDocumentDragOver"] = selector.ShowDropZoneOnDocumentDragOver.Value;

            if (!string.IsNullOrEmpty(selector.OnClientFileAdding))
                settings["onFileAdding"] = selector.OnClientFileAdding;
            if (!string.IsNullOrEmpty(selector.OnClientFileAdded))
                settings["onFileAdded"] = selector.OnClientFileAdded;
            if (!string.IsNullOrEmpty(selector.OnClientFileValidated))
                settings["onFileValidated"] = selector.OnClientFileValidated;
            if (!string.IsNullOrEmpty(selector.OnClientFileRemoved))
                settings["onFileRemoved"] = selector.OnClientFileRemoved;

            return settings;
        }
    }
}
