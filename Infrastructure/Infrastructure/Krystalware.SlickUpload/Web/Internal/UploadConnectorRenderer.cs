using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Krystalware.SlickUpload.Configuration;
using Krystalware.SlickUpload.Web.SessionStorage;

namespace Krystalware.SlickUpload.Web.Internal
{
    class UploadConnectorRenderer : ComponentRendererBase<IUploadConnector>
    {
        public override void Render(IUploadConnector uploadConnector, HtmlTextWriter w)
        {
            if (uploadConnector.Control != null && ComponentHelper.GetScriptManager(uploadConnector.Control.Page) != null)
            {
                // TODO: only if scriptmanager exists on page and this control was used as the attach placeholder for scripts
                uploadConnector.AddAttributesToRender(w, "su-uploadconnector", null);
                w.AddStyleAttribute("display", "none");
                w.RenderBeginTag("div");
                w.RenderEndTag();
            }
        }

        protected override Dictionary<string, object> GetSettings(IUploadConnector uploadConnector)
        {
            Dictionary<string, object> settings = new Dictionary<string, object>();

            if (string.IsNullOrEmpty(uploadConnector.Id))
                throw new Exception("UploadConnector Id is required.");

            // Make sure we have a solid session to work with if we're using a session storage provider
            SessionStateSessionStorageProvider sessionProvider = SlickUploadContext.SessionStorageProvider as SessionStateSessionStorageProvider;

            if (sessionProvider == null && SlickUploadContext.SessionStorageProvider is AdaptiveSessionStorageProvider)
                sessionProvider = ((AdaptiveSessionStorageProvider)SlickUploadContext.SessionStorageProvider).InternalProvider as SessionStateSessionStorageProvider;

            if (sessionProvider != null && HttpContext.Current.Session.Count == 0)
            {
                HttpContext.Current.Items["EnsureSessionCreated"] = true;

                //sessionProvider.WriteToSessionState("SlickUploadEnabled", true);
            }

            settings["id"] = uploadConnector.Id;

            string uploadHandlerUrl = !string.IsNullOrEmpty(uploadConnector.UploadHandlerUrl) ? uploadConnector.UploadHandlerUrl : "~/SlickUpload.axd";

            // TODO: check for ~/ first?
            uploadHandlerUrl = VirtualPathUtility.ToAbsolute(uploadHandlerUrl, HttpContext.Current.Request.ApplicationPath);
            uploadHandlerUrl = HttpContext.Current.Response.ApplyAppPathModifier(uploadHandlerUrl);

            settings["uploadHandlerUrl"] = uploadHandlerUrl;

            if (!string.IsNullOrEmpty(uploadConnector.CompleteHandlerUrl))
            {
                string completeHandlerUrl = VirtualPathUtility.ToAbsolute(uploadConnector.CompleteHandlerUrl, HttpContext.Current.Request.ApplicationPath);
                
                completeHandlerUrl = HttpContext.Current.Response.ApplyAppPathModifier(completeHandlerUrl);

                settings["completeHandlerUrl"] = completeHandlerUrl;
            }
            if (!string.IsNullOrEmpty(uploadConnector.CompletionMethod))
                settings["completionMethod"] = uploadConnector.CompletionMethod;
            if (!string.IsNullOrEmpty(uploadConnector.CompletionBody))
                settings["completionBody"] = uploadConnector.CompletionBody;
            if (!string.IsNullOrEmpty(uploadConnector.CompletionContentType))
                settings["completionContentType"] = uploadConnector.CompletionContentType;

            if (uploadConnector is Control)
            {
                Control updatePanel = ComponentHelper.GetParentUpdatePanel((Control)uploadConnector);

                if (updatePanel != null)
                {
                    settings["updatePanelId"] = updatePanel.UniqueID;
                    settings["postbackFunction"] = updatePanel.Page.ClientScript.GetPostBackEventReference((Control)uploadConnector, null);
                }
            }

            // TODO: validate
            settings["uploadForm"] = uploadConnector.UploadFormId;
            if (uploadConnector.AutoUploadOnSubmit != null)
                settings["autoUploadOnSubmit"] = uploadConnector.AutoUploadOnSubmit;
            if (uploadConnector.AutoCompleteAfterLastFile != null)
                settings["autoCompleteAfterLastFile"] = uploadConnector.AutoCompleteAfterLastFile;
            settings["confirmNavigateDuringUploadMessage"] = uploadConnector.ConfirmNavigateDuringUploadMessage;
            settings["uploadProfile"] = uploadConnector.UploadProfile;
            if (uploadConnector.AllowPartialError != null)
                settings["allowPartialError"] = uploadConnector.AllowPartialError.Value;
            else
            {
                UploadProfileElement profile = SlickUploadContext.Config.UploadProfiles.GetUploadProfileElement(uploadConnector.UploadProfile, true);

                if (profile != null)
                {
                    settings["allowPartialError"] = profile.AllowPartialError;

                    if (!string.IsNullOrEmpty(profile.DocumentDomain))
                        settings["documentDomain"] = profile.DocumentDomain;
                }
            }

            if (!string.IsNullOrEmpty(uploadConnector.OnClientBeforeSessionStart))
                settings["onBeforeSessionStart"] = uploadConnector.OnClientBeforeSessionStart;
            if (!string.IsNullOrEmpty(uploadConnector.OnClientUploadSessionStarted))
                settings["onUploadSessionStarted"] = uploadConnector.OnClientUploadSessionStarted;
            if (!string.IsNullOrEmpty(uploadConnector.OnClientUploadFileStarted))
                settings["onUploadFileStarted"] = uploadConnector.OnClientUploadFileStarted;
            if (!string.IsNullOrEmpty(uploadConnector.OnClientUploadFileEnded))
                settings["onUploadFileEnded"] = uploadConnector.OnClientUploadFileEnded;
            if (!string.IsNullOrEmpty(uploadConnector.OnClientUploadSessionProgress))
                settings["onUploadSessionProgress"] = uploadConnector.OnClientUploadSessionProgress;
            if (!string.IsNullOrEmpty(uploadConnector.OnClientBeforeSessionEnd))
                settings["onBeforeSessionEnd"] = uploadConnector.OnClientBeforeSessionEnd;
            if (!string.IsNullOrEmpty(uploadConnector.OnClientUploadSessionEnded))
                settings["onUploadSessionEnded"] = uploadConnector.OnClientUploadSessionEnded;

            if (uploadConnector.Data != null && uploadConnector.Data.Count > 0)
                settings["data"] = uploadConnector.Data;

            return settings;
        }
    }
}
