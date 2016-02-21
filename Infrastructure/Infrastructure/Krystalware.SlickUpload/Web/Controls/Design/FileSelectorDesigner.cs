using System;
using System.Web.UI;
using System.Web.UI.Design;
using System.ComponentModel;

namespace Krystalware.SlickUpload.Web.Controls.Design
{
    /// <summary>
    /// Provides a designer for the <see cref="FileSelector" /> control.
    /// </summary>
    public class FileSelectorDesigner : ControlDesigner
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

                Control control = (FileSelector)this.Component;

                TemplateGroup group = new TemplateGroup("FileSelector Templates");
                
                group.AddTemplateDefinition(new TemplateDefinition(this, "File selection", control, "Template", false));
                group.AddTemplateDefinition(new TemplateDefinition(this, "Unskinned file selection", control, "UnskinnedTemplate", false));
                group.AddTemplateDefinition(new TemplateDefinition(this, "Folder selection", control, "FolderTemplate", false));
                group.AddTemplateDefinition(new TemplateDefinition(this, "Unsupported message", control, "UnsupportedTemplate", false));
                group.AddTemplateDefinition(new TemplateDefinition(this, "DropZone", control, "DropZoneTemplate", false));

                templateGroups.Add(group);

                return templateGroups;
            }
        }
    }
}
