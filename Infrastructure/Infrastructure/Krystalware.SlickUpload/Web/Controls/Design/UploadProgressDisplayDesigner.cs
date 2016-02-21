using System;
using System.Web.UI;
using System.Web.UI.Design;
using System.ComponentModel;

namespace Krystalware.SlickUpload.Web.Controls.Design
{
    /// <summary>
    /// Provides a designer for the <see cref="UploadProgressDisplay" /> control.
    /// </summary>
    public class UploadProgressDisplayDesigner : ControlDesigner
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

                Control control = (UploadProgressDisplay)this.Component;

                TemplateGroup group = new TemplateGroup("UploadProgressDisplay Templates");
                
                group.AddTemplateDefinition(new TemplateDefinition(this, "Progress display", control, "Template", false));

                templateGroups.Add(group);

                return templateGroups;
            }
        }
    }
}
