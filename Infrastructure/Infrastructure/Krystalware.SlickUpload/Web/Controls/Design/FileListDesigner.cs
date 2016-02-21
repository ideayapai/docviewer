using System;
using System.Web.UI;
using System.Web.UI.Design;
using System.ComponentModel;

namespace Krystalware.SlickUpload.Web.Controls.Design
{
    /// <summary>
    /// Provides a designer for the <see cref="FileList" /> control.
    /// </summary>
    public class FileListDesigner : ControlDesigner
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

                Control control = (FileList)this.Component;

                TemplateGroup group = new TemplateGroup("FileList Templates");
                
                group.AddTemplateDefinition(new TemplateDefinition(this, "File item", control, "Template", false));

                templateGroups.Add(group);

                return templateGroups;
            }
        }
    }
}
