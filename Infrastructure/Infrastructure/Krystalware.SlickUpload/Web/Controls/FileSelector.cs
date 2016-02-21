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
    ///  Provides an interface for users to select files to upload.
    /// </summary>
    [ParseChildren(ChildrenAsProperties = true),
    ToolboxData(
@"<{0}:FileSelector runat=""server"">
    <Template>                               
        <a href=""javascript:;"">Add files</a>
    </Template>
    <FolderTemplate>       
        <a href=""javascript:;"">Add folder</a>
    </FolderTemplate>
    <DropZoneTemplate>                               
        <div>Drag and drop files here.</div>
    </DropZoneTemplate>
</{0}:FileSelector>"),
    ToolboxBitmap(typeof(FileSelector), "FileSelector.png"),
    Description(" Provides an interface for users to select files to upload"),
    Designer(typeof(FileSelectorDesigner))
    ]
    public class FileSelector : WebControlBase, IFileSelector, INamingContainer
    {
        ITemplate _template;
        ITemplate _unskinnedTemplate;
        ITemplate _folderTemplate;
        ITemplate _unsupportedTemplate;
        ITemplate _dropZoneTemplate;

        /// <summary>
        /// Gets or sets the <see cref="ITemplate" /> that defines how the default mode of the <see cref="FileSelector" /> control is displayed.
        /// </summary>
        [TemplateContainer(typeof(FileSelector)),
        Browsable(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue(typeof(ITemplate), ""),
        Description("Default template")]
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

        /// <summary>
        /// Gets or sets the <see cref="ITemplate" /> that defines how the unskinned mode of the <see cref="FileSelector" /> control is displayed.
        /// </summary>
        [TemplateContainer(typeof(FileSelector)),
        Browsable(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue(typeof(ITemplate), ""),
        Description("Unskinned template")]
        public ITemplate UnskinnedTemplate
        {
            get
            {
                return _unskinnedTemplate;
            }
            set
            {
                _unskinnedTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ITemplate" /> that defines how the folder selector mode of the <see cref="FileSelector" /> control is displayed.
        /// </summary>
        [TemplateContainer(typeof(FileSelector)),
        Browsable(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue(typeof(ITemplate), ""),
        Description("Folder template")]
        public ITemplate FolderTemplate
        {
            get
            {
                return _folderTemplate;
            }
            set
            {
                _folderTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ITemplate" /> that defines how the unsupported mode of the <see cref="FileSelector" /> control is displayed.
        /// </summary>
        [TemplateContainer(typeof(FileSelector)),
        Browsable(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue(typeof(ITemplate), ""),
        Description("Unsupported template")]
        public ITemplate UnsupportedTemplate
        {
            get
            {
                return _unsupportedTemplate;
            }
            set
            {
                _unsupportedTemplate = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ITemplate" /> that defines how the dropzone mode of the <see cref="FileSelector" /> control is displayed.
        /// </summary>
        [TemplateContainer(typeof(FileSelector)),
        Browsable(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue(typeof(ITemplate), ""),
        Description("DropZone template")]
        public ITemplate DropZoneTemplate
        {
            get
            {
                return _dropZoneTemplate;
            }
            set
            {
                _dropZoneTemplate = value;
            }
        }

        /// <see cref="IFileSelector.MaxFiles" copy="true" />
        [Description("The maximum number of files that can be selected, or -1 for unlimited.")]
        public int MaxFiles
        {
            get
            {
                object maxFilesObject = ViewState["MaxFiles"];

                if (maxFilesObject != null)
                    return int.Parse(maxFilesObject.ToString());
                else
                    return -1;
            }
            set
            {
                ViewState["MaxFiles"] = value;
            }
        }

        /// <see cref="IFileSelector.MaxFileSize" copy="true" />
        [Description("The individual maximum file size that can be selected, or -1 for unlimited.")]
        public int MaxFileSize
        {
            get
            {
                object maxFilesObject = ViewState["MaxFileSize"];

                if (maxFilesObject != null)
                    return int.Parse(maxFilesObject.ToString());
                else
                    return -1;
            }
            set
            {
                ViewState["MaxFileSize"] = value;
            }
        }

        /// <inheritdoc />
        public string ValidExtensions
        {
            get
            {
                return ViewState["ValidExtensions"] as string;
            }
            set
            {
                ViewState["ValidExtensions"] = value;
            }
        }

        /// <see cref="IFileSelector.IsSkinned" copy="true" />
        public bool IsSkinned
        {
            get
            {
                return (ViewState["IsSkinned"] as bool?) != false;
            }
            set
            {
                ViewState["IsSkinned"] = value;
            }
        }

        /// <see cref="IFileSelector.ShowDropZoneOnDocumentDragOver" copy="true" />
        public bool ShowDropZoneOnDocumentDragOver
        {
            get
            {
                return (ViewState["ShowDropZoneOnDocumentDragOver"] as bool?) == true;
            }
            set
            {
                ViewState["ShowDropZoneOnDocumentDragOver"] = value;
            }
        }

        /// <inheritdoc />
        [Description("A javascript function to call before a file is added.")]
        public string OnClientFileAdding
        {
            get
            {
                return ViewState["OnClientFileAdding"] as string;
            }
            set
            {
                ViewState["OnClientFileAdding"] = value;
            }
        }

        /// <inheritdoc />
        [Description("A javascript function to call when a file is added.")]
        public string OnClientFileAdded
        {
            get
            {
                return ViewState["OnClientFileAdded"] as string;
            }
            set
            {
                ViewState["OnClientFileAdded"] = value;
            }
        }

        /// <inheritdoc />
        [Description("A javascript function to call when a file is validated.")]
        public string OnClientFileValidated
        {
            get
            {
                return ViewState["OnClientFileValidated"] as string;
            }
            set
            {
                ViewState["OnClientFileValidated"] = value;
            }
        }

        /// <inheritdoc />
        [Description("A javascript function to call when a file is removed.")]
        public string OnClientFileRemoved
        {
            get
            {
                return ViewState["OnClientFileRemoved"] as string;
            }
            set
            {
                ViewState["OnClientFileRemoved"] = value;
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
        public string DropZoneId
        {
            get
            {
                return ViewState["DropZoneId"] as string;
            }
            set
            {
                ViewState["DropZoneId"] = value;
            }
        }

        /// <inheritdoc />
        [Description("The tag name of the container element for the file selector control."),
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

        string IFileSelector.UploadConnectorId
        {
            get
            {
                return ComponentHelper.FindRegistryControlID<UploadConnector>(this, UploadConnectorId);
            }
        }

        Mvc.Template IFileSelector.Template
        {
            get
            {
                if (Template != null)
                    return new Mvc.Template(Template);
                else
                    return null;
            }
        }

        Mvc.Template IFileSelector.UnskinnedTemplate
        {
            get
            {
                if (UnskinnedTemplate != null)
                    return new Mvc.Template(UnskinnedTemplate);
                else
                    return null;
            }
        }

        Mvc.Template IFileSelector.FolderTemplate
        {
            get
            {
                if (FolderTemplate != null)
                    return new Mvc.Template(FolderTemplate);
                else
                    return null;
            }
        }

        Mvc.Template IFileSelector.DropZoneTemplate
        {
            get
            {
                if (DropZoneTemplate != null)
                    return new Mvc.Template(DropZoneTemplate);
                else
                    return null;
            }
        }

        Mvc.Template IFileSelector.UnsupportedTemplate
        {
            get
            {
                if (UnsupportedTemplate != null)
                    return new Mvc.Template(UnsupportedTemplate);
                else
                    return null;
            }
        }

        int? IFileSelector.MaxFiles { get { return MaxFiles >= 0 ? (int?)MaxFiles : null; } }
        int? IFileSelector.MaxFileSize { get { return MaxFileSize >= 0 ? (int?)MaxFileSize : null; } }
        bool? IFileSelector.IsSkinned { get { return IsSkinned; } }
        bool? IFileSelector.ShowDropZoneOnDocumentDragOver { get { return ShowDropZoneOnDocumentDragOver; } }
    }
}
