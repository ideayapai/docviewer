using System;
using System.Web.UI;
using System.Web.UI.Design;
using System.ComponentModel;

namespace Krystalware.SlickUpload.Web.Controls.Design
{
    /// <summary>
    /// Provides a designer for the <see cref="SlickUpload" /> control.
    /// </summary>
    public class SlickUploadDesigner : ControlDesigner
    {
        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            SetViewFlags(ViewFlags.TemplateEditing, true);
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override TemplateGroupCollection TemplateGroups
        {
            get
            {
                TemplateGroupCollection templateGroups = new TemplateGroupCollection();

                Control control = (SlickUpload)this.Component;

                TemplateGroup group = new TemplateGroup("FileSelector Templates");

                group.AddTemplateDefinition(new TemplateDefinition(this, "File selection", control, "SelectorTemplate", false));
                group.AddTemplateDefinition(new TemplateDefinition(this, "Unskinned file selection", control, "SelectorUnskinnedTemplate", false));
                group.AddTemplateDefinition(new TemplateDefinition(this, "Folder selection", control, "SelectorFolderTemplate", false));
                group.AddTemplateDefinition(new TemplateDefinition(this, "Unsupported message", control, "SelectorUnsupportedTemplate", false));
                group.AddTemplateDefinition(new TemplateDefinition(this, "DropZone", control, "SelectorDropZoneTemplate", false));

                templateGroups.Add(group);

                group = new TemplateGroup("File item template");

                group.AddTemplateDefinition(new TemplateDefinition(this, "File item", control, "FileItemTemplate", false));

                templateGroups.Add(group);

                group = new TemplateGroup("Progress display template");

                group.AddTemplateDefinition(new TemplateDefinition(this, "Progress display", control, "ProgressTemplate", false));

                templateGroups.Add(group); 
                
                return templateGroups;
            }
        }
    }
}
