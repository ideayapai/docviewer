using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Drawing;
using Krystalware.SlickUpload.Web.Controls.Design;
using System.Collections.Specialized;
using System.Drawing.Design;
using System.ComponentModel.Design;
using Krystalware.SlickUpload.Web.Internal;

namespace Krystalware.SlickUpload.Web.Controls
{
    /// <summary>
    /// Connects an HTML form with out of band uploads.
    /// </summary>
    [
    ToolboxData(@"<{0}:UploadConnector runat=""server"" />"),
    ToolboxBitmap(typeof(UploadConnector), "UploadConnector.png"),
    Description("Connects an HTML form with out of band uploads."),
    DefaultEvent("UploadComplete"),

    //Designer(typeof(FileSelectorDesigner))
    ]
    public class UploadConnector : WebControlBase, IUploadConnector, IPostBackEventHandler
    {
        UploadSession _session;

        /// <summary>
        /// Occurs after an upload is completed.
        /// </summary>
        public event EventHandler<UploadSessionEventArgs> UploadComplete;

        Dictionary<string, string> _data;

        /// <inheritdoc />
        [DefaultValue("~/SlickUpload.axd"),
        Description("The URL to which to POST the upload.")]
        public string UploadHandlerUrl
        {
            get
            {
                return ViewState["UploadHandlerUrl"] as string;
            }
            set
            {
                ViewState["UploadHandlerUrl"] = value;
            }
        }

        /// <inheritdoc />
        public string CompleteHandlerUrl
        {
            get
            {
                return ViewState["CompleteHandlerUrl"] as string;
            }
            set
            {
                ViewState["CompleteHandlerUrl"] = value;
            }
        }

        /// <inheritdoc />
        public string CompletionMethod
        {
            get
            {
                return ViewState["CompletionMethod"] as string;
            }
            set
            {
                ViewState["CompletionMethod"] = value;
            }
        }

        /// <inheritdoc />
        public string CompletionBody
        {
            get
            {
                return ViewState["CompletionBody"] as string;
            }
            set
            {
                ViewState["CompletionBody"] = value;
            }
        }

        /// <inheritdoc />
        public string CompletionContentType
        {
            get
            {
                return ViewState["CompletionContentType"] as string;
            }
            set
            {
                ViewState["CompletionContentType"] = value;
            }
        }

        //public string UploadSessionId
        //{
        //    get
        //    {
        //        return ViewState["UploadSessionId"] as string;
        //    }
        //    set
        //    {
        //        ViewState["UploadSessionId"] = value;
        //    }
        //}

        /// <inheritdoc />
        public string UploadFormId
        {
            get
            {
                return ViewState["UploadFormId"] as string;
            }
            set
            {
                ViewState["UploadFormId"] = value;
            }
        }

        /// <see cref="IUploadConnector.AllowPartialError" copy="true" />
        public bool? AllowPartialError
        {
            get
            {
                return ViewState["AutoUploadOnSubmit"] as bool?;
            }
            set
            {
                ViewState["AutoUploadOnSubmit"] = value;
            }
        }

        /// <see cref="IUploadConnector.AutoUploadOnSubmit" copy="true" />
        public bool AutoUploadOnSubmit
        {
            get
            {
                return (ViewState["AutoUploadOnSubmit"] as bool?) != false;
            }
            set
            {
                ViewState["AutoUploadOnSubmit"] = value;
            }
        }

        /// <see cref="IUploadConnector.AutoCompleteAfterLastFile" copy="true" />
        public bool AutoCompleteAfterLastFile
        {
            get
            {
                return (ViewState["AutoCompleteAfterLastFile"] as bool?) != false;
            }
            set
            {
                ViewState["AutoCompleteAfterLastFile"] = value;
            }
        }

        /// <inheritdoc />
        public string ConfirmNavigateDuringUploadMessage
        {
            get
            {
                return ViewState["ConfirmNavigateDuringUploadMessage"] as string;
            }
            set
            {
                ViewState["ConfirmNavigateDuringUploadMessage"] = value;
            }
        }

        /// <inheritdoc />
        public string UploadProfile
        {
            get
            {
                return ViewState["UploadProfile"] as string;
            }
            set
            {
                ViewState["UploadProfile"] = value;
            }
        }

        /// <inheritdoc />
        public string OnClientBeforeSessionStart
        {
            get
            {
                return ViewState["OnClientBeforeSessionStart"] as string;
            }
            set
            {
                ViewState["OnClientBeforeSessionStart"] = value;
            }
        }

        /// <inheritdoc />
        public string OnClientUploadSessionStarted
        {
            get
            {
                return ViewState["OnClientUploadSessionStarted"] as string;
            }
            set
            {
                ViewState["OnClientUploadSessionStarted"] = value;
            }
        }

        /// <inheritdoc />
        public string OnClientUploadFileStarted
        {
            get
            {
                return ViewState["OnClientUploadFileStarted"] as string;
            }
            set
            {
                ViewState["OnClientUploadFileStarted"] = value;
            }
        }

        /// <inheritdoc />
        public string OnClientUploadFileEnded
        {
            get
            {
                return ViewState["OnClientUploadFileEnded"] as string;
            }
            set
            {
                ViewState["OnClientUploadFileEnded"] = value;
            }
        }

        /// <inheritdoc />
        public string OnClientUploadSessionProgress
        {
            get
            {
                return ViewState["OnClientUploadSessionProgress"] as string;
            }
            set
            {
                ViewState["OnClientUploadSessionProgress"] = value;
            }
        }

        /// <inheritdoc />
        public string OnClientBeforeSessionEnd
        {
            get
            {
                return ViewState["OnClientBeforeSessionEnd"] as string;
            }
            set
            {
                ViewState["OnClientBeforeSessionEnd"] = value;
            }
        }

        /// <inheritdoc />
        public string OnClientUploadSessionEnded
        {
            get
            {
                return ViewState["OnClientUploadSessionEnded"] as string;
            }
            set
            {
                ViewState["OnClientUploadSessionEnded"] = value;
            }
        }

        /// <inheritdoc />
        [TypeConverter(typeof(StringToDictionaryConverter))]
        [Editor(typeof(UITypeEditor), typeof(UITypeEditor))]
        public Dictionary<string, string> Data
        {
            get
            {
                // TODO: figure out how to make this work with viewstate
                if (_data == null)
                    _data = new Dictionary<string, string>();

                return _data;
            }
            set
            {
                _data = value;
            }
        }
        
        /// <summary>
        /// Returns the current <see cref="UploadSession" />, or null if there is no current session.
        /// </summary>
        [Browsable(false),
        Description("The current upload session.")]
        public UploadSession UploadSession
        {
            get
            {
                return _session;
            }
        }

        /// <summary>
        /// Returns the current collection of <see cref="UploadedFile" />s, or null if there is no current session.
        /// </summary>
        [Browsable(false),
        Description("The current collection of uploaded files.")]
        public ICollection<UploadedFile> UploadedFiles
        {
            get
            {
                if (UploadSession != null)
                    return UploadSession.UploadedFiles;
                else
                    return null;
            }
        }
        
        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            OnUploadComplete();

            // Required for subsequent postbacks in ASP.NET AJAX
            // TODO: pass this in to the uploadconnector so it can be used to submit if no other element is clicked to upload and .start() is called instead
            if (ComponentHelper.GetScriptManager(Page) != null)
                //string postBackFunction = "function() {" + Page.ClientScript.GetPostBackEventReference(this, null) + "}";
                Page.ClientScript.GetPostBackEventReference(this, null);

            base.OnLoad(e);
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
            // TODO: tackle multiple connectors once and for all
            /*IList<WebControlBase> uploadConnectors = ComponentHelper.GetRegistryList<UploadConnector>(Page);

            if (uploadConnectors.Count > 1)
            {
                bool hasOtherConnector = false;

                foreach (WebControlBase control in uploadConnectors)
                {
                    if (control.Visible && control != this && !(control.ClientID.EndsWith(ClientID) || ClientID.EndsWith(control.ClientID)))
                        hasOtherConnector = true;
                }

                //if (hasOtherConnector)
                //    throw new InvalidOperationException("Multiple SlickUpload or UploadConnector control instances detected. There can only be one instance per page.");
            }*/

            base.OnPreRender(e);
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            OnUploadComplete();
        }

        void OnUploadComplete()
        {
            _session = SlickUploadContext.CurrentUploadSession;

            if (_session != null && UploadComplete != null && (string.IsNullOrEmpty(_session.SourceUploadConnectorId) || ClientID == _session.SourceUploadConnectorId))
            {
                string key = "kw_HasOnCompleteFired" + _session.UploadSessionId;
                bool hasOnCompleteFired = Context.Items[key] is bool ? (bool)Context.Items[key] : false;

                if (!hasOnCompleteFired)
                {
                    Context.Items[key] = true;

                    UploadComplete(this, new UploadSessionEventArgs(_session));

                    ComponentHelper.EnsureParentUpdatePanelUpdated(this);
                }
            }
        }
        
        string IUploadConnector.UploadFormId
        {
            get
            {
                if (!string.IsNullOrEmpty(UploadFormId))
                    return UploadFormId;
                else if (Page.Form != null)
                {
                    if (string.IsNullOrEmpty(Page.Form.ClientID))
                        Page.Form.ID = Page.Form.UniqueID ?? "aspnetForm";

                    return Page.Form.ClientID;
                }
                else
                    throw new InvalidOperationException("UploadConnector must be placed inside a form tag with runat=server.");
            }
        }

        /*Dictionary<string, string> IUploadConnector.Data
        {
            get            
            {
                return null;
                if (string.IsNullOrEmpty(Data))
                    return null;

                try
                {
                    Dictionary<string, string> data = new Dictionary<string, string>();

                    // TODO: make this more resiliant
                    foreach (string part in Data.Split(','))
                    {
                        if (part.Trim().Length > 0)
                        {
                            string[] values = part.Split('=', ':');

                            data[values[0].Trim()] = values[1].Trim();
                        }
                    }

                    return data;
                }
                catch
                {
                    throw new FormatException("Couldn't parse UploadConnector.Data property value \"" + Data + "\". Value must be a dictionary string in the format of \"key=value,key2=value2\".");
                }
            }
        }*/

        bool? IUploadConnector.AutoUploadOnSubmit { get { return AutoUploadOnSubmit; } }
        bool? IUploadConnector.AutoCompleteAfterLastFile { get { return AutoCompleteAfterLastFile; } }
    }
}
