using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Drawing;
using Krystalware.SlickUpload.Web.Controls.Design;
using System.Drawing.Design;
using Krystalware.SlickUpload.Web.Internal;

namespace Krystalware.SlickUpload.Web.Controls
{
    /// <summary>
    /// Provides an interface for users to select files, upload them, and view rich progress based on an AJAX handler.
    /// </summary>
    [ParseChildren(ChildrenAsProperties = true),
    ToolboxData(
@"<{0}:SlickUpload Id=""slickUpload"" Style=""overflow: hidden; zoom: 1"" FileSelectorStyle=""float:left;padding-right:1em"" FileListStyle=""clear:both"" UploadProgressDisplayStyle=""clear:both"" runat=""server"">
    <SelectorTemplate>                               
        <a href=""javascript:;"">Add files</a>
    </SelectorTemplate>
    <SelectorFolderTemplate>       
        <a href=""javascript:;"">Add folder</a>
    </SelectorFolderTemplate>
    <FileItemTemplate>                               
        <{0}:FileListElement Element=""FileName"" runat=""server""/>
        &ndash;
        <{0}:FileListElement Element=""FileSize"" runat=""server""/>
        <{0}:FileListRemoveCommand runat=""server"" href=""javascript:;"">[x]</{0}:FileListRemoveCommand>
        <{0}:FileListElement Element=""ValidationMessage"" runat=""server"" style=""color:#f00""/>
    </FileItemTemplate>
    <ProgressTemplate>                               
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
    </ProgressTemplate>
</{0}:SlickUpload>"),
    ToolboxBitmap(typeof(SlickUpload), "SlickUpload.png"),
    Description("Provides an interface for users to select files, upload them, and view rich progress based on an AJAX handler"),
    Designer(typeof(SlickUploadDesigner))
    ]
    public class SlickUpload : WebControlBase, ISlickUpload, INamingContainer
    {
        FileSelector _fileSelector = new FileSelector();
        FileList _fileList = new FileList();
        UploadProgressDisplay _uploadProgressDisplay = new UploadProgressDisplay();
        UploadConnector _uploadConnector = new UploadConnector();

        /// <see cref="UploadConnector.UploadComplete" copy="true"/>
        public event EventHandler<UploadSessionEventArgs> UploadComplete;

        // Common
        /// <see cref="IUploadProgressDisplay.FileSizeFormatter" copy="true" />
        public string FileSizeFormatter 
        {
            get { return _fileList.FileSizeFormatter; }
            set
            {
                _fileList.FileSizeFormatter = value;
                _uploadProgressDisplay.FileSizeFormatter = value;
            }
        }

        // FileSelector
        /// <summary>
        /// Gets or sets an inline style attribute for the <see cref="FileSelector" /> container element.
        /// </summary>
        public string FileSelectorStyle
        {
            get { return _fileSelector.Style.Value; }
            set { _fileSelector.Style.Value = value; }
        }

        /// <see cref="IFileSelector.Template" copy="true" />
        [TemplateContainer(typeof(FileSelector)),
        Browsable(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue(typeof(ITemplate), ""),
        Description("Downlevel mode template")]
        public ITemplate SelectorTemplate
        {
            get { return _fileSelector.Template; }
            set { _fileSelector.Template = value; }
        }
        /// <see cref="IFileSelector.UnskinnedTemplate" copy="true" />
        [TemplateContainer(typeof(FileSelector)),
        Browsable(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue(typeof(ITemplate), ""),
        Description("Downlevel mode template")]
        public ITemplate SelectorUnskinnedTemplate 
        {
            get { return _fileSelector.UnskinnedTemplate; }
            set { _fileSelector.UnskinnedTemplate = value; }
        }
        /// <see cref="IFileSelector.UnskinnedTemplate" copy="true" />
        [TemplateContainer(typeof(FileSelector)),
        Browsable(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue(typeof(ITemplate), ""),
        Description("Downlevel mode template")]
        public ITemplate SelectorFolderTemplate
        {
            get { return _fileSelector.FolderTemplate; }
            set { _fileSelector.FolderTemplate = value; }
        }
        /// <see cref="IFileSelector.UnsupportedTemplate" copy="true" />
        [TemplateContainer(typeof(FileSelector)),
        Browsable(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue(typeof(ITemplate), ""),
        Description("Downlevel mode template")]
        public ITemplate SelectorUnsupportedTemplate 
        {
            get { return _fileSelector.UnsupportedTemplate; }
            set { _fileSelector.UnsupportedTemplate = value; }
        }
        /// <see cref="IFileSelector.DropZoneTemplate" copy="true" />
        [TemplateContainer(typeof(FileSelector)),
        Browsable(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue(typeof(ITemplate), ""),
        Description("Downlevel mode template")]
        public ITemplate SelectorDropZoneTemplate
        {
            get { return _fileSelector.DropZoneTemplate; }
            set { _fileSelector.DropZoneTemplate = value; }
        }
        /// <see cref="IFileSelector.MaxFiles" copy="true" />
        public int MaxFiles 
        {
            get { return _fileSelector.MaxFiles; }
            set { _fileSelector.MaxFiles = value; }
        }
        /// <see cref="IFileSelector.MaxFileSize" copy="true" />
        public int MaxFileSize 
        {
            get { return _fileSelector.MaxFileSize; }
            set { _fileSelector.MaxFileSize = value; }
        }
        /// <see cref="IFileSelector.ValidExtensions" copy="true" />
        public string ValidExtensions 
        {
            get { return _fileSelector.ValidExtensions; }
            set { _fileSelector.ValidExtensions = value; }
        }
        /// <see cref="IFileSelector.IsSkinned" copy="true" />
        public bool IsSkinned 
        {
            get { return _fileSelector.IsSkinned; }
            set { _fileSelector.IsSkinned = value; }
        }
        /// <see cref="IFileSelector.ShowDropZoneOnDocumentDragOver" copy="true" />
        public bool ShowDropZoneOnDocumentDragOver 
        {
            get { return _fileSelector.ShowDropZoneOnDocumentDragOver; }
            set { _fileSelector.ShowDropZoneOnDocumentDragOver = value; }
        }
        /// <see cref="IFileSelector.ContainerTagName" copy="true" />
        public string SelectorContainerTagName 
        {
            get { return _fileSelector.ContainerTagName; }
            set { _fileSelector.ContainerTagName = value; }
        }
        /// <see cref="IFileSelector.OnClientFileAdding" copy="true" />
        public string OnClientFileAdding 
        {
            get { return _fileSelector.OnClientFileAdding; }
            set { _fileSelector.OnClientFileAdding = value; }
        }
        /// <see cref="IFileSelector.OnClientFileAdded" copy="true" />
        public string OnClientFileAdded 
        {
            get { return _fileSelector.OnClientFileAdded; }
            set { _fileSelector.OnClientFileAdded = value; }
        }
        /// <see cref="IFileSelector.OnClientFileValidated" copy="true" />
        public string OnClientFileValidated
        {
            get { return _fileSelector.OnClientFileValidated; }
            set { _fileSelector.OnClientFileValidated = value; }
        }
        /// <see cref="IFileSelector.OnClientFileRemoved" copy="true" />
        public string OnClientFileRemoved 
        {
            get { return _fileSelector.OnClientFileRemoved; }
            set { _fileSelector.OnClientFileRemoved = value; }
        }

        // FileList
        /// <summary>
        /// Gets or sets an inline style attribute for the <see cref="FileList" /> container element.
        /// </summary>
        public string FileListStyle
        {
            get { return _fileList.Style.Value; }
            set { _fileList.Style.Value = value; }
        }
        /// <see cref="IFileList.ItemTemplate" copy="true" />
        [TemplateContainer(typeof(FileList)),
        Browsable(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue(typeof(ITemplate), ""),
        Description("Downlevel mode template")]
        public ITemplate FileItemTemplate  
        {
            get { return _fileList.ItemTemplate; }
            set { _fileList.ItemTemplate = value; }
        }
        /// <see cref="IFileList.InvalidFileSizeMessage" copy="true" />
        public string InvalidFileSizeMessage 
        {
            get { return _fileList.InvalidFileSizeMessage; }
            set { _fileList.InvalidFileSizeMessage = value; }
        }
        /// <see cref="IFileList.InvalidExtensionMessage" copy="true" />
        public string InvalidExtensionMessage 
        {
            get { return _fileList.InvalidExtensionMessage; }
            set { _fileList.InvalidExtensionMessage = value; }
        }
        /// <see cref="IFileList.FileValidationMessageFormatter" copy="true" />
        public string FileValidationMessageFormatter 
        {
            get { return _fileList.FileValidationMessageFormatter; }
            set { _fileList.FileValidationMessageFormatter = value; }
        }
        /// <see cref="IFileList.ContainerTagName" copy="true" />
        public string FileContainerTagName 
        {
            get { return _fileList.ContainerTagName; }
            set { _fileList.ContainerTagName = value; }
        }
        /// <see cref="IFileList.ItemTagName" copy="true" />
        public string FileItemTagName 
        {
            get { return _fileList.ItemTagName; }
            set { _fileList.ItemTagName = value; }
        }

        // UploadProgressDisplay
        /// <summary>
        /// Gets or sets an inline style attribute for the <see cref="UploadProgressDisplay" /> container element.
        /// </summary>
        public string UploadProgressDisplayStyle
        {
            get { return _uploadProgressDisplay.Style.Value; }
            set { _uploadProgressDisplay.Style.Value = value; }
        }
        /// <see cref="IUploadProgressDisplay.Template" copy="true" />
        [TemplateContainer(typeof(UploadProgressDisplay)),
        Browsable(false),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue(typeof(ITemplate), ""),
        Description("Downlevel mode template")]
        public ITemplate ProgressTemplate 
        {
            get { return _uploadProgressDisplay.Template; }
            set { _uploadProgressDisplay.Template = value; }
        }
        /// <see cref="IUploadProgressDisplay.ContainerTagName" copy="true" />
        public string ProgressContainerTagName 
        {
            get { return _uploadProgressDisplay.ContainerTagName; }
            set { _uploadProgressDisplay.ContainerTagName = value; }
        }
        /// <see cref="IUploadProgressDisplay.PercentFormatter" copy="true" />
        public string PercentFormatter 
        {
            get { return _uploadProgressDisplay.PercentFormatter; }
            set { _uploadProgressDisplay.PercentFormatter = value; }
        }
        /// <see cref="IUploadProgressDisplay.TimeFormatter" copy="true" />
        public string TimeFormatter 
        {
            get { return _uploadProgressDisplay.TimeFormatter; }
            set { _uploadProgressDisplay.TimeFormatter = value; }
        }
        /// <see cref="IUploadProgressDisplay.ShowDuringUpload" copy="true" />
        public bool ShowProgressDuringUpload 
        {
            get { return _uploadProgressDisplay.ShowDuringUpload; }
            set { _uploadProgressDisplay.ShowDuringUpload = value; }
        }
        /// <see cref="IUploadProgressDisplay.HideAfterUpload" copy="true" />
        public bool HideProgressAfterUpload 
        {
            get { return _uploadProgressDisplay.HideAfterUpload; }
            set { _uploadProgressDisplay.HideAfterUpload = value; }
        }

        // UploadConnector
        /// <see cref="IUploadConnector.UploadHandlerUrl" copy="true" />
        public string UploadHandlerUrl 
        {
            get { return _uploadConnector.UploadHandlerUrl; }
            set { _uploadConnector.UploadHandlerUrl = value; }
        }
        /// <see cref="IUploadConnector.CompleteHandlerUrl" copy="true" />
        public string CompleteHandlerUrl 
        {
            get { return _uploadConnector.CompleteHandlerUrl; }
            set { _uploadConnector.CompleteHandlerUrl = value; }
        }
        /// <see cref="IUploadConnector.CompletionMethod" copy="true" />
        public string CompletionMethod
        {
            get { return _uploadConnector.CompletionMethod; }
            set { _uploadConnector.CompletionMethod = value; }
        }
        /// <see cref="IUploadConnector.CompletionBody" copy="true" />
        public string CompletionBody
        {
            get { return _uploadConnector.CompletionBody; }
            set { _uploadConnector.CompletionBody = value; }
        }
        /// <see cref="IUploadConnector.CompletionContentType" copy="true" />
        public string CompletionContentType
        {
            get { return _uploadConnector.CompletionContentType; }
            set { _uploadConnector.CompletionContentType = value; }
        }
        //public string UploadSessionId 
        /// <see cref="IUploadConnector.UploadFormId" copy="true" />
        public string UploadFormId 
        {
            get { return _uploadConnector.UploadFormId; }
            set { _uploadConnector.UploadFormId = value; }
        }
        /// <see cref="IUploadConnector.AutoUploadOnSubmit" copy="true" />
        public bool AutoUploadOnSubmit 
        {
            get { return _uploadConnector.AutoUploadOnSubmit; }
            set { _uploadConnector.AutoUploadOnSubmit = value; }
        }
        /// <see cref="IUploadConnector.AutoCompleteAfterLastFile" copy="true" />
        public bool AutoCompleteAfterLastFile 
        {
            get { return _uploadConnector.AutoCompleteAfterLastFile; }
            set { _uploadConnector.AutoCompleteAfterLastFile = value; }
        }
        /// <see cref="IUploadConnector.ConfirmNavigateDuringUploadMessage" copy="true" />
        public string ConfirmNavigateDuringUploadMessage 
        {
            get { return _uploadConnector.ConfirmNavigateDuringUploadMessage; }
            set { _uploadConnector.ConfirmNavigateDuringUploadMessage = value; }
        }
        /// <see cref="IUploadConnector.UploadProfile" copy="true" />
        public string UploadProfile 
        {
            get { return _uploadConnector.UploadProfile; }
            set { _uploadConnector.UploadProfile = value; }
        }

        /// <see cref="IUploadConnector.OnClientBeforeSessionStart" copy="true" />
        public string OnClientBeforeSessionStart 
        {
            get { return _uploadConnector.OnClientBeforeSessionStart; }
            set { _uploadConnector.OnClientBeforeSessionStart = value; }
        }
        /// <see cref="IUploadConnector.OnClientUploadSessionStarted" copy="true" />
        public string OnClientUploadSessionStarted 
        {
            get { return _uploadConnector.OnClientUploadSessionStarted; }
            set { _uploadConnector.OnClientUploadSessionStarted = value; }
        }
        /// <see cref="IUploadConnector.OnClientUploadFileStarted" copy="true" />
        public string OnClientUploadFileStarted 
        {
            get { return _uploadConnector.OnClientUploadFileStarted; }
            set { _uploadConnector.OnClientUploadFileStarted = value; }
        }
        /// <see cref="IUploadConnector.OnClientUploadFileEnded" copy="true" />
        public string OnClientUploadFileEnded 
        {
            get { return _uploadConnector.OnClientUploadFileEnded; }
            set { _uploadConnector.OnClientUploadFileEnded = value; }
        }
        /// <see cref="IUploadConnector.OnClientUploadSessionProgress" copy="true" />
        public string OnClientUploadSessionProgress 
        {
            get { return _uploadConnector.OnClientUploadSessionProgress; }
            set { _uploadConnector.OnClientUploadSessionProgress = value; }
        }
        /// <see cref="IUploadConnector.OnClientBeforeSessionEnd" copy="true" />
        public string OnClientBeforeSessionEnd 
        {
            get { return _uploadConnector.OnClientBeforeSessionEnd; }
            set { _uploadConnector.OnClientBeforeSessionEnd = value; }
        }
        /// <see cref="IUploadConnector.OnClientUploadSessionEnded" copy="true" />
        public string OnClientUploadSessionEnded
        {
            get { return _uploadConnector.OnClientUploadSessionEnded; }
            set { _uploadConnector.OnClientUploadSessionEnded = value; }
        }

        /// <see cref="IUploadConnector.Data" copy="true" />
        [TypeConverter(typeof(StringToDictionaryConverter))]
        [Editor(typeof(UITypeEditor), typeof(UITypeEditor))]
        public Dictionary<string, string> Data
        {
            get { return _uploadConnector.Data; }
            set { _uploadConnector.Data = value; }
        }

        /// <see cref="UploadConnector.UploadSession" copy="true" />
        [Browsable(false),
        Description("The current upload session.")]
        public UploadSession UploadSession
        {
            get
            {
                return _uploadConnector.UploadSession;
            }
        }

        /// <see cref="UploadConnector.UploadedFiles" copy="true" />
        [Browsable(false),
        Description("The current collection of uploaded files.")]
        public ICollection<UploadedFile> UploadedFiles
        {
            get
            {
                return _uploadConnector.UploadedFiles;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override void CreateChildControls()
        {
            _fileSelector.ID = "selector";
            Controls.Add(_fileSelector);

            _fileList.ID = "list";
            _fileList.FileSelectorId = _fileSelector.ClientID;
            Controls.Add(_fileList);

            _uploadProgressDisplay.ID = "display";
            Controls.Add(_uploadProgressDisplay);

            _uploadConnector.ID = "connector";
            Controls.Add(_uploadConnector);

            _uploadConnector.UploadComplete += new EventHandler<UploadSessionEventArgs>(uploadConnector_UploadComplete);
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            EnsureChildControls();

            base.OnInit(e);
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            _fileList.FileSelectorId = _fileSelector.ClientID;
            _fileSelector.UploadConnectorId = _uploadConnector.ClientID;
            _uploadProgressDisplay.UploadConnectorId = _uploadConnector.ClientID;

            base.OnLoad(e);
        }

        void uploadConnector_UploadComplete(object sender, UploadSessionEventArgs e)
        {
            if (UploadComplete != null)
                UploadComplete(this, e);
        }

        IFileSelector ISlickUpload.FileSelector
        {
            get { return _fileSelector; }
        }

        IFileList ISlickUpload.FileList
        {
            get { return _fileList.ItemTemplate != null ? _fileList : null; }
        }

        IUploadProgressDisplay ISlickUpload.UploadProgressDisplay
        {
            get { return _uploadProgressDisplay.Template != null ? _uploadProgressDisplay : null; }
        }

        IUploadConnector ISlickUpload.UploadConnector
        {
            get { return _uploadConnector; }
        }
    }
}