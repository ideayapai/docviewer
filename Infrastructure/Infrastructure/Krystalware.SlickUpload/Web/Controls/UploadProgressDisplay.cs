using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Drawing;
using Krystalware.SlickUpload.Web.Controls.Design;
using Krystalware.SlickUpload.Web.Internal;

namespace Krystalware.SlickUpload.Web.Controls
{
    /// <summary>
    /// Displays upload progress to users based on an AJAX handler.
    /// </summary>
    [ParseChildren(ChildrenAsProperties = true),
    ToolboxData(
@"<{0}:UploadProgressDisplay runat=""server"">
    <div>
        Uploading <{0}:UploadProgressElement Element=""FileCount"" runat=""server""/> file(s),
        <{0}:UploadProgressElement Element=""ContentLengthText"" runat=""server"">(calculating)</{0}:UploadProgressElement>.
    </div>
    <div>
        Currently uploading: <{0}:UploadProgressElement Element=""CurrentFileName"" runat=""server""/>
        file <{0}:UploadProgressElement Element=""CurrentFileIndex"" runat=""server""/>
        of <{0}:UploadProgressElement Element=""FileCount"" runat=""server""/>.
    </div>
    <div>
        Speed: <{0}:UploadProgressElement Element=""SpeedText"" runat=""server"">(calculating)</{0}:UploadProgressElement>
    </div>
    <div>
        <{0}:UploadProgressElement Element=""TimeRemainingText"" runat=""server"">(calculating)</{0}:UploadProgressElement>
    </div>
    <div style=""border:1px solid #008800; height: 1.5em; position: relative;"">
        <{0}:UploadProgressBar runat=""server"" style=""background-color:#00ee00; width:0; height:1.5em;"" />
        <div class=""progressBarText"" style=""text-align: center; position: absolute; top: .15em; width: 100%;"">
            <{0}:UploadProgressElement Element=""PercentCompleteText"" runat=""server"">(calculating)</{0}:UploadProgressElement>
        </div>
    </div>
</{0}:UploadProgressDisplay>"),
    ToolboxBitmap(typeof(UploadProgressDisplay), "UploadProgressDisplay.png"),
    Description("Displays upload progress to users based on an AJAX handler"),
    Designer(typeof(UploadProgressDisplayDesigner))
    ]
    public class UploadProgressDisplay : WebControlBase, IUploadProgressDisplay, INamingContainer
    {
        ITemplate _template;

        /// <summary>
        /// Gets or sets the <see cref="ITemplate" /> that defines how the <see cref="UploadProgressDisplay" /> control is displayed.
        /// </summary>
        [TemplateContainer(typeof(UploadProgressDisplay)),
        Browsable(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue(typeof(ITemplate), ""),
        Description("Uplevel mode template")]
        public ITemplate Template
        {
            get
            {
                return _template;
            }
            set
            {
                _template = value;
            }
        }

        /// <inheritdoc />
        public string UploadConnectorId
        {
            get
            {
                return ViewState["UploadConnectorId"] as string;
            }
            set
            {
                ViewState["UploadConnectorId"] = value;
            }
        }

        /// <inheritdoc />
        public string FileSizeFormatter
        {
            get
            {
                return ViewState["FileSizeFormatter"] as string;
            }
            set
            {
                ViewState["FileSizeFormatter"] = value;
            }
        }

        /// <inheritdoc />
        public string PercentFormatter
        {
            get
            {
                return ViewState["PercentFormatter"] as string;
            }
            set
            {
                ViewState["PercentFormatter"] = value;
            }
        }

        /// <inheritdoc />
        public string TimeFormatter
        {
            get
            {
                return ViewState["TimeFormatter"] as string;
            }
            set
            {
                ViewState["TimeFormatter"] = value;
            }
        }

        /// <inheritdoc />
        [Description("The tag name of the container element to for the upload progress display control."),
        DefaultValue("div")]
        public string ContainerTagName
        {
            get
            {
                return ViewState["ContainerTagName"] as string;
            }
            set
            {
                ViewState["ContainerTagName"] = value;
            }
        }

        /// <see cref="IUploadProgressDisplay.ShowDuringUpload" copy="true" />
        public bool ShowDuringUpload
        {
            get
            {
                return (ViewState["ShowDuringUpload"] as bool?) != false;
            }
            set
            {
                ViewState["ShowDuringUpload"] = value;
            }
        }

        /// <see cref="IUploadProgressDisplay.HideAfterUpload" copy="true" />
        public bool HideAfterUpload
        {
            get
            {
                return (ViewState["HideAfterUpload"] as bool?) != false;
            }
            set
            {
                ViewState["HideAfterUpload"] = value;
            }
        }
        
        string IUploadProgressDisplay.UploadConnectorId
        {
            get
            {
                return ComponentHelper.FindRegistryControlID<UploadConnector>(this, UploadConnectorId);
            }
        }

        Mvc.Template IUploadProgressDisplay.Template
        {
            get
            {
                if (Template != null)
                    return new Mvc.Template(Template);
                else
                    return null;
            }
        }

        bool? IUploadProgressDisplay.ShowDuringUpload { get { return ShowDuringUpload; } }
        bool? IUploadProgressDisplay.HideAfterUpload { get { return HideAfterUpload; } }
    }
}