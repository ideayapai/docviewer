using System;
using System.Configuration;

namespace DocViewerdesktop.Configuration
{
    public class UploadSection : ConfigurationSection
    {
        [ConfigurationProperty("uploadurl", IsRequired = true)]
        public string UploadUrl
        {
            get { return base["uploadurl"].ToString(); }
            set { base["uploadurl"] = value; }
        }

        [ConfigurationProperty("username", IsRequired = true)]
        public string UserName
        {
            get { return base["username"].ToString(); }
            set { base["username"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return base["password"].ToString(); }
            set { base["password"] = value; }
        }
    }
}
