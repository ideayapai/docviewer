using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;

namespace Krystalware.SlickUpload.Web.Internal
{
    abstract class ComponentRendererBase<T> : IComponentRenderer
        where T : IRenderableComponent
    {
        readonly string _clientType;
        readonly int _renderOrder;

        public ComponentRendererBase()
        {
            if (typeof(T).IsAssignableFrom(typeof(IUploadConnector)))
            {
                _renderOrder = 0;
                _clientType = "kw.UploadConnector";
            }
            else if (typeof(T).IsAssignableFrom(typeof(IFileSelector)))
            {
                _renderOrder = 1;
                _clientType = "kw.FileSelector";
            }
            else if (typeof(T).IsAssignableFrom(typeof(IUploadProgressDisplay)))
            {
                _renderOrder = 1;
                _clientType = "kw.UploadProgressDisplay";
            }
            else if (typeof(T).IsAssignableFrom(typeof(IFileList)))
            {
                _renderOrder = 2;
                _clientType = "kw.FileList";
            }
            else if (typeof(T).IsAssignableFrom(typeof(ISlickUpload)))
            {
                _renderOrder = 3;
                _clientType = "kw.SlickUpload";
            }
            else if (typeof(T).IsAssignableFrom(typeof(IMarkerComponent)))
                _renderOrder = 4;
        }

        public virtual void Register(T component)
        {
            Dictionary<string, object> settings = GetSettings(component);

            if (settings != null)
                ComponentHelper.RegisterComponent(component.Control, _clientType, _renderOrder, settings);
        }
        
        public abstract void Render(T component, HtmlTextWriter w);

        protected abstract Dictionary<string, object> GetSettings(T component);

        void IComponentRenderer.Register(IRenderableComponent component)
        {
            Register((T)component);
        }

        void IComponentRenderer.Render(IRenderableComponent component, HtmlTextWriter w)
        {
            Render((T)component, w);

            if (component.Control != null)
                ComponentHelper.OnRender(component.Control, w);
        }

        Type controlType = typeof(Control);

        protected bool IsDesignMode(T control)
        {
            if (control is Control)
                return controlType.InvokeMember("DesignMode", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, control, null) as bool? == true;
            else
                return false;
        }
    }
}
