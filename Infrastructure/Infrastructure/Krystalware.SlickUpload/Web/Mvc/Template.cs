using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;

namespace Krystalware.SlickUpload.Web.Mvc
{
    /// <summary>
    /// Represents a renderable element template.
    /// </summary>
    public class Template
    {
        string _string;
        ITemplate _template;
        Action _action;
        Action<HtmlTextWriter> _actionWriter;

#if !NET35
        public delegate void Action();
        public delegate void Action<T>(T param);
        public delegate TResult Func<T, TResult>(T param);
#endif

        Template()
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="Template" /> class that outputs the specified <see cref="String" />.
        /// </summary>
        /// <param name="value">The <see cref="String" /> to output.</param>
        public Template(string value)
        {
            _string = value;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Template" /> class that executes the specified <see cref="Action" /> delegate to render.
        /// </summary>
        /// <param name="value">The <see cref="Action" /> delegate to render.</param>
        public Template(Action value)
        {
            _action = value;
        }

        internal static Template CreateTemplate(Action<HtmlTextWriter> value)
        {
            Template t = new Template();

            t._actionWriter = value;

            return t;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Template" /> class that executes the specified Razor action to render.
        /// </summary>
        /// <param name="value">The Razor action to render.</param>
        public Template(Func<object, object> value)
        {
            _string = value(null).ToString();
        }

        internal Template(ITemplate value)
        {
            _template = value;
        }

        /// <summary>
        /// Renders this template to the specified <see cref="HtmlTextWriter" />
        /// </summary>
        /// <param name="writer">The <see cref="HtmlTextWriter" /> to which to render.</param>
        public void Render(HtmlTextWriter writer)
        {
            if (_string != null)
                writer.Write(_string);
            else if (_action != null)
                _action();
            else if (_actionWriter != null)
                _actionWriter(writer);
            else if (_template != null)
            {
                PlaceHolder ph = new PlaceHolder();

                _template.InstantiateIn(ph);

                // TODO?
                //using (HtmlTextWriter w = new HtmlTextWriter(writer))
                    ph.RenderControl(writer);
            }
        }
    }
}
