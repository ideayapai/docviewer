using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Infrasturcture.Web
{
    public static class WebConfigUtils
    {
        public static string Host
        {
            get
            {
                return GetValue("Host");
            }
        }
        private static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
