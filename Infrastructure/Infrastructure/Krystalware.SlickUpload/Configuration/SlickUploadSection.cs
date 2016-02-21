using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web;
using Krystalware.SlickUpload.Web;

namespace Krystalware.SlickUpload.Configuration
{
    /// <summary>
    /// Defines configuration settings for SlickUpload.
    /// </summary>
    public class SlickUploadSection : ConfigurationSection
    {
        readonly ConfigurationProperty _propUploadProfiles;
        readonly ConfigurationProperty _propSessionStorageProvider;
        //readonly ConfigurationProperty _propResourceRoot;
        readonly ConfigurationProperty _propHandleRequests;
        readonly ConfigurationProperty _propScriptUrl;
        readonly ConfigurationProperty _propBrandLocation;
        readonly ConfigurationProperty _propAttachEvent;

        /// <summary>
        /// Gets a <see cref="UploadProfileCollection" /> of <see cref="UploadProfileElement" /> objects that define the configured upload profiles.
        /// </summary>
        public UploadProfileCollection UploadProfiles
        {
            get { return (UploadProfileCollection)base[_propUploadProfiles]; }
        }

        /// <summary>
        /// Gets an <see cref="SessionStorageProviderElement" /> that defines the session storage provider configuration.
        /// </summary>
        public SessionStorageProviderElement SessionStorageProvider
        {
            get
            {
                return (SessionStorageProviderElement)base[_propSessionStorageProvider];
            }
            set
            {
                base[_propSessionStorageProvider] = value;
            }
        }

        /*//[ConfigurationProperty("resourceRoot")]
        public string ResourceRoot
        {
            get
            {
                return (string)base[_propResourceRoot];
            }
            set
            {
                base[_propResourceRoot] = value;
            }
        }*/

        /// <summary>
        /// Gets or sets a boolean that specifies whether SlickUpload should handle requests.
        /// </summary>
        public bool HandleRequests
        {
            get
            {
                return (bool)base[_propHandleRequests];
            }
            set
            {
                base[_propHandleRequests] = value;
            }
        }

        /// <summary>
        /// Gets or sets a string that specifies the URL to the SlickUpload javascript. If null, SlickUpload will use a WebResource.axd reference to its internal script.
        /// </summary>
        public string ScriptUrl
        {
            get
            {
                return (string)base[_propScriptUrl];
            }
            set
            {
                base[_propScriptUrl] = value;
            }
        }

        /// <summary>
        /// Gets or sets a string that specifies whether to intercept the upload at the BeginRequest or the PreRequestHandlerExecute event.
        /// </summary>
        public string AttachEvent
        {
            get
            {
                return (string)base[_propAttachEvent];
            }
            set
            {
                base[_propAttachEvent] = value;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="BrandLocation"/> that specifies where to display the branding for community licensed installations.
        /// </summary>
        public BrandLocation BrandLocation
        {
            get
            {
                return (BrandLocation)base[_propBrandLocation];
            }
            set
            {
                base[_propBrandLocation] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SlickUploadSection" /> class.
        /// </summary>
        public SlickUploadSection()
        {
            _propUploadProfiles = new ConfigurationProperty("uploadProfiles", typeof(UploadProfileCollection), null, ConfigurationPropertyOptions.None);
            _propSessionStorageProvider = new ConfigurationProperty("sessionStorageProvider", typeof(SessionStorageProviderElement), null, ConfigurationPropertyOptions.None);
            //_propResourceRoot = new ConfigurationProperty("resourceRoot", typeof(string), null, ConfigurationPropertyOptions.None);
            _propHandleRequests = new ConfigurationProperty("handleRequests", typeof(bool), false, ConfigurationPropertyOptions.None);
            _propScriptUrl = new ConfigurationProperty("scriptUrl", typeof(string), null, ConfigurationPropertyOptions.None);
            _propAttachEvent = new ConfigurationProperty("attachEvent", typeof(string), null, ConfigurationPropertyOptions.None);
            _propBrandLocation = new ConfigurationProperty("brandLocation", typeof(BrandLocation), BrandLocation.Inline, ConfigurationPropertyOptions.None);

            Properties.Add(_propUploadProfiles);
            Properties.Add(_propSessionStorageProvider);
            //Properties.Add(_propResourceRoot);
            Properties.Add(_propHandleRequests);
            Properties.Add(_propScriptUrl);
            Properties.Add(_propAttachEvent);
            Properties.Add(_propBrandLocation);
        }
    }
}
