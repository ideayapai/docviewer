using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.UI;

namespace Krystalware.SlickUpload.Web.Internal
{
    class ComponentScript : ScriptElement
    {
        public ComponentScript(Control control, string clientType, Dictionary<string, object> settings, int renderOrder)
            : base(control, RenderScript(clientType, settings), renderOrder)
        { }

        static string RenderScript(string clientType, Dictionary<string, object> settings)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("var component = new " + clientType + "({");

            bool isFirst = true;

            foreach (KeyValuePair<string, object> setting in settings)
            {
                if (!isFirst)
                    sb.AppendLine(",");
                else
                    isFirst = false;

                string value;

                if (setting.Value == null)
                    value = "null";
                else if (setting.Value is string)
                    value = "\"" + ((string)setting.Value).Replace("\r", "\\r").Replace("\n", "\\n").Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
                else if (setting.Value is LiteralString)
                    value = ((LiteralString)setting.Value).Value;
                else if (setting.Value is bool)
                    value = setting.Value.ToString().ToLower();
                else if (setting.Value is float)
                    value = ((float)setting.Value).ToString("##0.00", CultureInfo.InvariantCulture);
                else if (setting.Value is Dictionary<string, string>)
                    value = JsonHelper.Serialize((Dictionary<string, string>)setting.Value);
                else
                    value = setting.Value.ToString();

                sb.Append(setting.Key + ":" + value);
            }

            sb.AppendLine("});");

            return sb.ToString();
        }
    }
}
