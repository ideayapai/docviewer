using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Krystalware.SlickUpload.Web.Controls;

namespace Krystalware.SlickUpload.Web.Internal
{
    internal static class ComponentHelper
    {
        const int _renderOrderMax = 4;
        const string _lastControlKey = "kw_LastRenderControl";
        const string _scriptListKey = "kw_ScriptList";
        const string _scriptRenderCountKey = "kw_ScriptRenderCount";

        internal static void RegisterComponent(Control control, string clientType, int renderOrder, Dictionary<string, object> settings)
        {
            if (control != null)
            {
                if (control.Page != null && control.Page.Form != null && GetContextItem(_lastControlKey) == null)
                    control.Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);

                SetContextItem(_lastControlKey, control);
            }

            if (clientType != null)
            {
                ComponentScript script = new ComponentScript(control, clientType, settings, renderOrder);

                GetScriptList().Add(script);
            }
        }

        internal static void RegisterInitStatement(string script)
        {       
            GetScriptList().Add(new ScriptElement(null, script, _renderOrderMax));
        }

        private static List<ScriptElement> GetScriptList()
        {
            List<ScriptElement> scriptList = GetContextItem(_scriptListKey) as List<ScriptElement>;

            if (scriptList == null)
            {
                scriptList = new List<ScriptElement>();

                SetContextItem(_scriptListKey, scriptList);
            }

            return scriptList;
        }

        static void Page_PreRenderComplete(object sender, EventArgs e)
        {
            Page page = ((Page)sender);

            if (!HasScriptRendererWithIncludeScripts)
                RegisterClientScriptResource(page, typeof(ComponentHelper), "Krystalware.SlickUpload.Resources.SlickUpload.js");

            if (GetScriptManager(page) == null)
            {
                string script = BuildScript(GetScriptList());

                RegisterStartupScript(page, (Control)GetContextItem(_lastControlKey), typeof(ComponentHelper), "slickupload", script, true);
            }
            else
            {
                Dictionary<Control, List<ScriptElement>> perUpdatePanelBreakdown = new Dictionary<Control, List<ScriptElement>>();

                // Dictionary doesn't support null keys, so we have to do a dummy instead.
                Control noUpdatePanelKey = new Control();

                foreach (ScriptElement el in GetScriptList())
                {
                    Control updatePanel = null;
                    
                    if (el.Control != null)
                        updatePanel = GetParentUpdatePanel(el.Control);

                    if (updatePanel == null)
                        updatePanel = noUpdatePanelKey;

                    List<ScriptElement> updatePanelScriptElementList;

                    if (!perUpdatePanelBreakdown.TryGetValue(updatePanel, out updatePanelScriptElementList))
                        perUpdatePanelBreakdown[updatePanel] = updatePanelScriptElementList = new List<ScriptElement>();

                    updatePanelScriptElementList.Add(el);
                }

                foreach (KeyValuePair<Control, List<ScriptElement>> updatePanelScript in perUpdatePanelBreakdown)
                {
                    string script = BuildScript(updatePanelScript.Value);

                    if (updatePanelScript.Key != noUpdatePanelKey)
                        RegisterStartupScript(page, updatePanelScript.Value[updatePanelScript.Value.Count - 1].Control, typeof(ComponentHelper), "slickupload", script, true);
                    else
                        RegisterStartupScript(page, null, typeof(ComponentHelper), "slickupload", script, true);
                }
            }

            ScriptRenderCount = GetScriptList().Count;
        }

        internal static void RegisterStartupScript(Page page, Control control, Type type, string key, string script, bool addScriptTags)
        {
            Type t;
            object scriptManager = GetScriptManager(page, out t);

            if (scriptManager != null)
                t.InvokeMember("RegisterStartupScript", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, scriptManager, new object[] { control ?? page, type, key, script, addScriptTags });
            else
                page.ClientScript.RegisterStartupScript(typeof(ComponentHelper), "slickupload", script, true);
        }

        internal static void EnsureParentUpdatePanelUpdated(Control control)
        {
            Control updatePanel = GetParentUpdatePanel(control);

            if (updatePanel != null)
            {
                Type t = updatePanel.GetType();

                if ((int)t.InvokeMember("UpdateMode", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, updatePanel, null) == 1)
                    updatePanel.GetType().InvokeMember("Update", BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod, null, updatePanel, null);
            }
        }

        internal static Control GetParentUpdatePanel(Control control)
        {
            Control parent = control.Parent;

            while (parent != null && !(parent.GetType().Name == "UpdatePanel"))
                parent = parent.Parent;

            return parent;
        }

        internal static void RegisterClientScriptResource(Page page, Type type, string resource)
        {
            if (page.Form != null)
            {
                string key = type.FullName;

                string url = page.ClientScript.GetWebResourceUrl(type, resource);

                object scriptManager = GetScriptManager(page);

                if (scriptManager != null && GetIsInAsyncPostBack(page) && !page.ClientScript.IsClientScriptIncludeRegistered(key))
                {
                    RegisterAspNetAjaxClientScriptInclude(page, type, key, url);
                }
                else
                {
                    page.ClientScript.RegisterClientScriptInclude(key, url);
                }
            }
        }

        internal static bool GetIsInAsyncPostBack(Page page)
        {
            try
            {
                object scriptManager = GetScriptManager(page);

                if (scriptManager != null)
                    return (bool)scriptManager.GetType().InvokeMember("IsInAsyncPostBack", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, scriptManager, null);
            }
            catch { }

            return false;
        }

        internal static void RegisterAspNetAjaxClientScriptInclude(Page page, Type type, string key, string url)
        {
            Type t;
            object scriptManager = GetScriptManager(page, out t);

            if (scriptManager != null)
                t.InvokeMember("RegisterClientScriptInclude", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, scriptManager, new object[] { page, type, key, url });
        }
        
        internal static object GetScriptManager(Page page)
        {
            Type t;

            return GetScriptManager(page, out t);
        }

        internal static object GetScriptManager(Page page, out Type t)
        {
            object scriptManager = null;

            try
            {
                scriptManager = GetScriptManager("System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", page, out t);
            }
            catch
            {
                t = null;
            }

            if (scriptManager == null)
            {
                try
                {
                    scriptManager = GetScriptManager("System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", page, out t);
                }
                catch
                {
                    t = null;
                }
            }

            return scriptManager;
        }

        static object GetScriptManager(string assemblyName, Page page, out Type t)
        {
            Assembly a = Assembly.Load(assemblyName);

            if (a != null)
            {
                t = a.GetType("System.Web.UI.ScriptManager");

                return t.InvokeMember("GetCurrent", BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { page });
            }
            else
            {
                t = null;

                return null;
            }
        }

        private static string BuildScript(List<ScriptElement> scripts)
        {
            StringBuilder sb = new StringBuilder();

            // TODO: add manual setting
            if (HttpContext.Current != null && HttpContext.Current.IsDebuggingEnabled)
                sb.AppendLine("kw.debug = true;");

            if (scripts.Count > 0)
            {
                sb.AppendLine("kw._registerInit(function() {");

                for (int i = 0; i <= _renderOrderMax; i++)
                {
                    foreach (ScriptElement script in scripts)
                    {
                        if (script.RenderOrder == i)
                            sb.AppendLine(script.Script);
                    }
                }

                sb.AppendLine("});");
            }

            return sb.ToString();
        }

        internal static void OnRender(Control control, HtmlTextWriter writer)
        {
            if (control == null || (control.Page != null && control.Page.Form == null && control == GetContextItem(_lastControlKey)))
            {
                RenderScripts(writer, true);
            }
        }

        internal static void RenderScripts(HtmlTextWriter writer, bool scriptInclude)
        {
            if (scriptInclude && !HasRenderedScriptInclude)
            {
                string scriptUrl = SlickUploadContext.Config.ScriptUrl;

                // TODO: optimize Page reference
                if (string.IsNullOrEmpty(scriptUrl))
                {
                    // TODO: fix this
                    if (HttpContext.Current != null)                    
                        scriptUrl = new Page().ClientScript.GetWebResourceUrl(typeof(ComponentHelper), "Krystalware.SlickUpload.Resources.SlickUpload.js");
                }

                writer.AddAttribute("type", "text/javascript");
                writer.AddAttribute("src", scriptUrl);
                writer.RenderBeginTag("script");
                writer.RenderEndTag();

                HasRenderedScriptInclude = true;
            }

            List<ScriptElement> scriptList = GetScriptList();

            if (scriptList.Count > ScriptRenderCount)
            {
                List<ScriptElement> newScriptList;

                if (ScriptRenderCount > 0)
                {
                    newScriptList = new List<ScriptElement>();

                    for (int i = ScriptRenderCount; i < scriptList.Count; i++)
                        newScriptList.Add(scriptList[i]);
                }
                else
                    newScriptList = scriptList;

                string script = BuildScript(newScriptList);

                writer.AddAttribute("type", "text/javascript");
                writer.RenderBeginTag("script");
                writer.WriteLine("//<![CDATA[");
                writer.Write(script);
                writer.WriteLine("//]]>");
                writer.RenderEndTag();
            }

            ScriptRenderCount = GetScriptList().Count;
        }

        internal static bool HasRenderedScriptInclude
        {
            get
            {
                bool? value = GetContextItem("scriptinclude") as bool?;

                return value ?? false;
            }
            set
            {
                SetContextItem("scriptinclude", value);
            }
        }

        internal static bool HasScriptRendererWithIncludeScripts
        {
            get
            {
                bool? value = GetContextItem("hasScriptRendererWithIncludeScripts") as bool?;

                return value ?? false;
            }
            set
            {
                SetContextItem("hasScriptRendererWithIncludeScripts", value);
            }
        }


        internal static int ScriptRenderCount
        {
            get
            {
                int? value = GetContextItem(_scriptRenderCountKey) as int?;

                return value ?? 0;
            }
            set
            {
                SetContextItem(_scriptRenderCountKey, value);
            }
        }

        internal static object GetContextItem(string key)
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.Items[key];
            else
                // TODO: should we dummy up one?
                return null;
        }

        internal static void SetContextItem(string key, object value)
        {
            if (HttpContext.Current != null)
                HttpContext.Current.Items[key] = value;

            // TODO: should we dummy up one?
        }

        internal static void EnsureScriptsRendered()
        {
            List<ScriptElement> scriptList = GetScriptList();

            if (scriptList != null && scriptList.Count > ScriptRenderCount)
                throw new InvalidOperationException("Krystalware components were rendered on the page but the resulting script was never rendered. After all components have been rendered, call <% Html.KrystalwareWebForms(new KrystalwareScriptRenderer()); %> for ASP.NET MVC WebForms pages, or @Html.KrystalwareRazor(new KrystalwareScriptRenderer()) for Razor pages to render script."); 
        }

        internal static void AddAttributesToRender(HtmlTextWriter w, string id, string className, string defaultStyle, object attributes)
        {
            if (!string.IsNullOrEmpty(id))
                w.AddAttribute(HtmlTextWriterAttribute.Id, id);

            if (attributes != null || !string.IsNullOrEmpty(className))
            {
                string existingClassName = null;
                string existingStyle = null;

                if (attributes != null)
                {
                    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(attributes);

                    foreach (PropertyDescriptor property in properties)
                    {
                        if (property.Name.Equals("class", StringComparison.InvariantCultureIgnoreCase))
                            existingClassName = property.GetValue(attributes).ToString();
                        else if (property.Name.Equals("style", StringComparison.InvariantCultureIgnoreCase))
                            existingStyle = property.GetValue(attributes).ToString();
                        else
                            w.AddAttribute(property.Name, property.GetValue(attributes).ToString());
                    }
                }

                if (!string.IsNullOrEmpty(existingClassName))
                    className = className != null ? className + " " + existingClassName : existingClassName;

                if (!string.IsNullOrEmpty(className))
                    w.AddAttribute("class", className);

                if (string.IsNullOrEmpty(existingStyle))
                    existingStyle = defaultStyle;

                if (!string.IsNullOrEmpty(existingStyle))
                    w.AddAttribute("style", existingStyle);
            }
        }

        internal static void AddCssClass(WebControl control, string cssClass)
        {
            if (string.IsNullOrEmpty(control.CssClass))
                control.CssClass = cssClass;
            else if (Array.IndexOf<string>(control.CssClass.Split(' '), cssClass) == -1)
                control.CssClass += " " + cssClass;
        }

        internal static Control FindControlRecursive(Control root, string id)
        {
            // BAD (doesn't find in repeater): We use UniqueID here first, then ClientID so we don't call EnsureID for each control and break GridView etc.
            // (root.UniqueID.Replace('$', '_').EndsWith(id)
            // We check to see if id is possible before checking ClientID so we don't call EnsureID for each control and break GridView etc.
            if (!string.IsNullOrEmpty(root.ID) && (root.ID == id || (id.IndexOf(root.ID) != -1 && root.ClientID == id)))
                return root;

            foreach (Control c in root.Controls)
            {
                Control t = FindControlRecursive(c, id);

                if (t != null)
                    return t;
            }

            return null;
        }

        internal static List<Control> FindControlsRecursive(Control root, Type type)
        {
            List<Control> controls = new List<Control>();

            FindControlsRecursive(controls, root, type);

            return controls;
        }

        internal static void FindControlsRecursive(List<Control> controls, Control root, Type type)
        {
            if (root.GetType() == type)
                controls.Add(root);

            foreach (Control c in root.Controls)
                FindControlsRecursive(controls, c, type);
        }

        static Dictionary<string, WebControlBase> GetRegistry(Page page)
        {
            Dictionary<string, WebControlBase> registry = page.Items[typeof(ComponentHelper)] as Dictionary<string, WebControlBase>;

            if (registry == null)
                page.Items[typeof(ComponentHelper)] = registry = new Dictionary<string, WebControlBase>();

            return registry;
        }

        internal static void AddToRegistry(WebControlBase control)
        {
            //if (!string.IsNullOrEmpty(control.ID))
            //{
                Dictionary<string, WebControlBase> registry = GetRegistry(control.Page);

                registry[control.ClientID] = control;
            //}
        }

        internal static bool IsControlOnPage(Control c)
        {
            Control parent = c.Parent;
            Type pageType = typeof(Page);

            while (true)
            {
                if (parent == null)
                    return false;
                else if (pageType.IsAssignableFrom(parent.GetType()))
                    return true;

                parent = parent.Parent;
            }
        }

        internal static IList<WebControlBase> GetRegistryList<T>(Page page) where T : WebControlBase
        {
            List<WebControlBase> controls = new List<WebControlBase>();

            foreach (WebControlBase control in GetRegistry(page).Values)
            {
                if (control is T && IsControlOnPage(control))
                    controls.Add(control);
            }

            return controls;
        }

        internal static string FindRegistryControlID<T>(Control sourceControl, string id) where T : WebControlBase
        {
            T control = null;

            if (!string.IsNullOrEmpty(id))
            {
                control = ComponentHelper.FindControlRecursive(sourceControl.Page, id) as T;

                if (control == null)
                    throw new InvalidOperationException("Couldn't find " + typeof(T).Name + " with ID of '" + id + "' for " + sourceControl.GetType().Name + " '" + sourceControl.ClientID + "'.");
            }

            if (control == null)
            {
                IList<WebControlBase> controlList = ComponentHelper.GetRegistryList<T>(sourceControl.Page);

                if (controlList.Count == 1)
                    control = controlList[0] as T;
                else if (controlList.Count > 1)
                    throw new InvalidOperationException("Multiple " + typeof(T).Name + " instances detected. " + sourceControl.GetType().Name + "." + typeof(T).Name + "Id must be set.");
            }

            if (control != null)
                return control.ClientID;
            else
                throw new InvalidOperationException("No " + typeof(T).Name + " found for " + sourceControl.GetType().Name + " '" + sourceControl.ClientID + "'.");
        }
    }
}
