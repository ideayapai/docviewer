#if NET35
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Linq.Expressions;
using Krystalware.SlickUpload.Web.Controls;
using System.Web;
using Krystalware.SlickUpload.Web.Internal;

namespace Krystalware.SlickUpload.Web.Mvc
{
    /// <summary>
    /// Represents a renderable SlickUpload control. This control composes the <see cref="FileSelector" />,
    /// <see cref="FileList" />, <see cref="UploadProgressDisplay" />, and <see cref="UploadConnector" /> controls into one control.
    /// </summary>
    public class SlickUpload : MvcComponentBase, ISlickUpload
    {
        // Common
        /// <see cref="IUploadProgressDisplay.FileSizeFormatter" copy="true" />
        public string FileSizeFormatter { get; set; }

        // FileSelector
        /// <see cref="MvcComponentBase.HtmlAttributes" copy="true" />
        public object FileSelectorHtmlAttributes { get; set; }
        /// <see cref="IFileSelector.Template" copy="true" />
        public Template SelectorTemplate { get; set; }
        /// <see cref="IFileSelector.UnskinnedTemplate" copy="true" />
        public Template SelectorUnskinnedTemplate { get; set; }
        /// <see cref="IFileSelector.FolderTemplate" copy="true" />
        public Template SelectorFolderTemplate { get; set; }
        /// <see cref="IFileSelector.UnsupportedTemplate" copy="true" />
        public Template SelectorUnsupportedTemplate { get; set; }
        /// <see cref="IFileSelector.DropZoneTemplate" copy="true" />
        public Template SelectorDropZoneTemplate { get; set; }
        /*/// <see cref="IFileSelector.DropZoneId" copy="true" />
        //public string DropZoneId { get; set; }*/
        /// <see cref="IFileSelector.MaxFiles" copy="true" />
        public int? MaxFiles { get; set; }
        /// <see cref="IFileSelector.MaxFileSize" copy="true" />
        public int? MaxFileSize { get; set; }
        /// <see cref="IFileSelector.ValidExtensions" copy="true" />
        public string ValidExtensions { get; set; }
        /// <see cref="IFileSelector.IsSkinned" copy="true" />
        public bool? IsSkinned { get; set; }
        /// <see cref="IFileSelector.ShowDropZoneOnDocumentDragOver" copy="true" />
        public bool? ShowDropZoneOnDocumentDragOver { get; set; }
        /// <see cref="IFileSelector.ContainerTagName" copy="true" />
        public string SelectorContainerTagName { get; set; }

        /// <see cref="IFileSelector.OnClientFileAdding" copy="true" />
        public string OnClientFileAdding { get; set; }
        /// <see cref="IFileSelector.OnClientFileAdded" copy="true" />
        public string OnClientFileAdded { get; set; }
        /// <see cref="IFileSelector.OnClientFileValidated" copy="true" />
        public string OnClientFileValidated { get; set; }
        /// <see cref="IFileSelector.OnClientFileRemoved" copy="true" />
        public string OnClientFileRemoved { get; set; }

        // FileList
        /// <see cref="MvcComponentBase.HtmlAttributes" copy="true" />
        public object FileListHtmlAttributes { get; set; }
        /// <see cref="IFileList.ItemTemplate" copy="true" />
        public Template FileItemTemplate { get; set; }
        /// <see cref="IFileList.InvalidFileSizeMessage" copy="true" />
        public string InvalidFileSizeMessage { get; set; }
        /// <see cref="IFileList.InvalidExtensionMessage" copy="true" />
        public string InvalidExtensionMessage { get; set; }
        /// <see cref="IFileList.FileValidationMessageFormatter" copy="true" />
        public string FileValidationMessageFormatter { get; set; }
        /// <see cref="IFileList.ContainerTagName" copy="true" />
        public string FileContainerTagName { get; set; }
        /// <see cref="IFileList.ItemTagName" copy="true" />
        public string FileItemTagName { get; set; }

        // UploadProgressDisplay
        /// <see cref="MvcComponentBase.HtmlAttributes" copy="true" />
        public object UploadProgressDisplayHtmlAttributes { get; set; }
        /// <see cref="IUploadProgressDisplay.Template" copy="true" />
        public Template ProgressTemplate { get; set; }
        /// <see cref="IUploadProgressDisplay.ContainerTagName" copy="true" />
        public string ProgressContainerTagName { get; set; }
        /// <see cref="IUploadProgressDisplay.PercentFormatter" copy="true" />
        public string PercentFormatter { get; set; }
        /// <see cref="IUploadProgressDisplay.TimeFormatter" copy="true" />
        public string TimeFormatter { get; set; }
        /// <see cref="IUploadProgressDisplay.ShowDuringUpload" copy="true" />
        public bool? ShowProgressDuringUpload { get; set; }
        /// <see cref="IUploadProgressDisplay.HideAfterUpload" copy="true" />
        public bool? HideProgressAfterUpload { get; set; }

        // UploadConnector
        /// <see cref="IUploadConnector.UploadHandlerUrl" copy="true" />
        public string UploadHandlerUrl { get; set; }
        /// <see cref="IUploadConnector.CompleteHandlerUrl" copy="true" />
        public string CompleteHandlerUrl { get; set; }
        /// <see cref="IUploadConnector.CompletionMethod" copy="true" />
        public string CompletionMethod { get; set; }
        /// <see cref="IUploadConnector.CompletionBody" copy="true" />
        public string CompletionBody { get; set; }
        /// <see cref="IUploadConnector.CompletionContentType" copy="true" />
        public string CompletionContentType { get; set; }
        //public string UploadSessionId { get; set; }
        /// <see cref="IUploadConnector.UploadFormId" copy="true" />
        public string UploadFormId { get; set; }
        /// <see cref="IUploadConnector.AutoUploadOnSubmit" copy="true" />
        public bool? AutoUploadOnSubmit { get; set; }
        /// <see cref="IUploadConnector.AutoCompleteAfterLastFile" copy="true" />
        public bool? AutoCompleteAfterLastFile { get; set; }
        /// <see cref="IUploadConnector.ConfirmNavigateDuringUploadMessage" copy="true" />
        public string ConfirmNavigateDuringUploadMessage { get; set; }
        /// <see cref="IUploadConnector.UploadProfile" copy="true" />
        public string UploadProfile { get; set; }

        /// <see cref="IUploadConnector.OnClientBeforeSessionStart" copy="true" />
        public string OnClientBeforeSessionStart { get; set; }
        /// <see cref="IUploadConnector.OnClientUploadSessionStarted" copy="true" />
        public string OnClientUploadSessionStarted { get; set; }
        /// <see cref="IUploadConnector.OnClientUploadFileStarted" copy="true" />
        public string OnClientUploadFileStarted { get; set; }
        /// <see cref="IUploadConnector.OnClientUploadFileEnded" copy="true" />
        public string OnClientUploadFileEnded { get; set; }
        /// <see cref="IUploadConnector.OnClientUploadSessionProgress" copy="true" />
        public string OnClientUploadSessionProgress { get; set; }
        /// <see cref="IUploadConnector.OnClientBeforeSessionEnd" copy="true" />
        public string OnClientBeforeSessionEnd { get; set; }
        /// <see cref="IUploadConnector.OnClientUploadSessionEnded" copy="true" />
        public string OnClientUploadSessionEnded { get; set; }

        /// <see cref="IUploadConnector.Data" copy="true" />
        public Dictionary<string, string> Data { get; set; }

        IFileSelector _fileSelector;
        IFileList _fileList;
        IUploadProgressDisplay _uploadProgressDisplay;
        IUploadConnector _uploadConnector;

        IFileSelector ISlickUpload.FileSelector
        {
            get
            {
                if (_fileSelector == null)
                {
                    _fileSelector = new FileSelector()
                    {
                        Id = Id + "_selector",
                        UploadConnectorId = Id + "_connector",
                        Template = SelectorTemplate,
                        UnskinnedTemplate = SelectorUnskinnedTemplate,
                        FolderTemplate = SelectorFolderTemplate,
                        UnsupportedTemplate = SelectorUnsupportedTemplate,
                        DropZoneTemplate = SelectorDropZoneTemplate,
                        MaxFiles = MaxFiles,
                        MaxFileSize = MaxFileSize,
                        ValidExtensions = ValidExtensions,
                        IsSkinned = IsSkinned,
                        ShowDropZoneOnDocumentDragOver = ShowDropZoneOnDocumentDragOver,
                        ContainerTagName = SelectorContainerTagName,
                        OnClientFileAdded = OnClientFileAdded,
                        OnClientFileValidated = OnClientFileValidated,
                        OnClientFileAdding = OnClientFileAdding,
                        OnClientFileRemoved = OnClientFileRemoved,
                        HtmlAttributes = FileSelectorHtmlAttributes
                    };
                }

                return _fileSelector;
            }
        }

        IFileList ISlickUpload.FileList
        {
            get
            {
                if (_fileList == null && FileItemTemplate != null)
                {
                    _fileList = new FileList()
                    {
                        Id = Id + "_list",
                        FileSelectorId = Id + "_selector",
                        ItemTemplate = FileItemTemplate,
                        InvalidFileSizeMessage = InvalidFileSizeMessage,
                        InvalidExtensionMessage = InvalidExtensionMessage,
                        FileValidationMessageFormatter = FileValidationMessageFormatter,
                        ContainerTagName = FileContainerTagName,
                        ItemTagName = FileItemTagName,
                        FileSizeFormatter = FileSizeFormatter,
                        HtmlAttributes = FileListHtmlAttributes
                    };
                }

                return _fileList;
            }
        }

        IUploadProgressDisplay ISlickUpload.UploadProgressDisplay
        {
            get
            {
                if (_uploadProgressDisplay == null && ProgressTemplate != null)
                {
                    _uploadProgressDisplay = new UploadProgressDisplay()
                    {
                        Id = Id + "_progress",
                        UploadConnectorId = Id + "_connector",
                        Template = ProgressTemplate,
                        ContainerTagName = ProgressContainerTagName,
                        PercentFormatter = PercentFormatter,
                        TimeFormatter = TimeFormatter,
                        ShowDuringUpload = ShowProgressDuringUpload,
                        HideAfterUpload = HideProgressAfterUpload,
                        FileSizeFormatter = FileSizeFormatter,
                        HtmlAttributes = UploadProgressDisplayHtmlAttributes
                    };
                }

                return _uploadProgressDisplay;
            }
        }

        IUploadConnector ISlickUpload.UploadConnector
        {
            get
            {
                if (_uploadConnector == null)
                {
                    _uploadConnector = new UploadConnector()
                    {
                        Id = Id + "_connector",
                        UploadHandlerUrl = UploadHandlerUrl,
                        CompleteHandlerUrl = CompleteHandlerUrl,
                        CompletionMethod = CompletionMethod,
                        CompletionBody = CompletionBody,
                        CompletionContentType = CompletionContentType,

                        // UploadSessionId = UploadSessionId,
                        UploadFormId = UploadFormId,
                        AutoUploadOnSubmit = AutoUploadOnSubmit,
                        AutoCompleteAfterLastFile = AutoCompleteAfterLastFile,
                        ConfirmNavigateDuringUploadMessage = ConfirmNavigateDuringUploadMessage,
                        UploadProfile = UploadProfile,

                        OnClientBeforeSessionStart = OnClientBeforeSessionStart,
                        OnClientUploadSessionStarted = OnClientUploadSessionStarted,
                        OnClientUploadFileStarted = OnClientUploadFileStarted,
                        OnClientUploadFileEnded = OnClientUploadFileEnded,
                        OnClientUploadSessionProgress = OnClientUploadSessionProgress,
                        OnClientBeforeSessionEnd = OnClientBeforeSessionEnd,
                        OnClientUploadSessionEnded = OnClientUploadSessionEnded,

                        Data = Data
                    };
                }

                return _uploadConnector;
            }
        }
    }
}
#endif