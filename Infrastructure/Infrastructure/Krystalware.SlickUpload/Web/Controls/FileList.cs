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
    /// Displays a list of files that have been selected for upload.
    /// </summary>
    [ParseChildren(ChildrenAsProperties = true),
    ToolboxData(
@"<{0}:FileList runat=""server"">
    <ItemTemplate>                               
        <{0}:FileListElement Element=""FileName"" runat=""server""/>
        &ndash;
        <{0}:FileListElement Element=""FileSize"" runat=""server""/>
        <{0}:FileListRemoveCommand runat=""server"" href=""javascript:;"">[x]</{0}:FileListRemoveCommand>
        <{0}:FileListElement Element=""ValidationMessage"" runat=""server"" style=""color:#f00""/>
    </ItemTemplate>
</{0}:FileList>"),
    ToolboxBitmap(typeof(FileList), "FileList.png"),
    Description("Displays a list of files that have been selected for upload."),
    Designer(typeof(FileListDesigner))
    ]
    public class FileList : WebControlBase, IFileList, INamingContainer
    {
        ITemplate _itemTemplate;

        /// <summary>
        /// Gets or sets the <see cref="ITemplate" /> that defines how each item in the <see cref="FileList" /> control is displayed.
        /// </summary>
        [TemplateContainer(typeof(FileList)),
        Browsable(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue(typeof(ITemplate), ""),
        Description("Item template")]
        public ITemplate ItemTemplate
        {
            get
            {
                return _itemTemplate;
            }
            set
            {
                _itemTemplate = value;
            }
        }

        /// <inheritdoc />
        public string FileSelectorId
        {
            get
            {
                return ViewState["FileSelectorId"] as string;
            }
            set
            {
                ViewState["FileSelectorId"] = value;
            }
        }

        /// <inheritdoc />
        public string InvalidFileSizeMessage
        {
            get
            {
                return ViewState["InvalidFileSizeMessage"] as string;
            }
            set
            {
                ViewState["InvalidFileSizeMessage"] = value;
            }
        }

        /// <inheritdoc />
        public string InvalidExtensionMessage
        {
            get
            {
                return ViewState["InvalidExtensionMessage"] as string;
            }
            set
            {
                ViewState["InvalidExtensionMessage"] = value;
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
        public string FileValidationMessageFormatter
        {
            get
            {
                return ViewState["FileValidationMessageFormatter"] as string;
            }
            set
            {
                ViewState["FileValidationMessageFormatter"] = value;
            }
        }

        /// <inheritdoc />
        [Description("The tag name of the container element to generate for the file list control."),
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

        /// <inheritdoc />
        [Description("The tag name of the container element to generate for each file item."),
        DefaultValue("div")]
        public string ItemTagName
        {
            get
            {
                return ViewState["ItemTagName"] as string;
            }
            set
            {
                ViewState["ItemTagName"] = value;
            }
        }
        
        string IFileList.FileSelectorId
        {
            get
            {
                return ComponentHelper.FindRegistryControlID<FileSelector>(this, FileSelectorId);
            }
        }

        Mvc.Template IFileList.ItemTemplate
        {
            get
            {
                if (ItemTemplate != null)
                    return new Mvc.Template(ItemTemplate);
                else
                    return null;
            }
        }
    }
}
