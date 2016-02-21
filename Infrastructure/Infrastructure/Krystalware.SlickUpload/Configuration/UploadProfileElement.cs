using System;
using System.Collections.Generic;
using System.Text;
using Krystalware.SlickUpload.Storage;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections;

namespace Krystalware.SlickUpload.Configuration
{
    /// <summary>
    /// Defines the configuration for an upload profile.
    /// </summary>
    public class UploadProfileElement : ConfigurationElement
    {
        //readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        readonly ConfigurationProperty _propName;
        readonly ConfigurationProperty _propDocumentDomain;
        readonly ConfigurationProperty _propUploadStreamProvider;
        readonly ConfigurationProperty _propUploadFilter;
        readonly ConfigurationProperty _propMaxRequestLength;
        readonly ConfigurationProperty _propExecutionTimeout;
        readonly ConfigurationProperty _propAllowPartialError;

        /// <summary>
        /// Gets or sets the name of this upload profile.
        /// </summary>
        public string Name
        {
            get
            {
                return (string)base[_propName];
            }
            set
            {
                base[_propName] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value to set for document.domain on ajax handlers, or null to leave it as the default.
        /// </summary>
        public string DocumentDomain
        {
            get
            {
                return (string)base[_propDocumentDomain];
            }
            set
            {
                base[_propDocumentDomain] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies whether to allow an upload session to complete successfully even when some of the files errored.
        /// </summary>
        public bool AllowPartialError
        {
            get
            {
                return (bool)base[_propAllowPartialError];
            }
            set
            {
                base[_propAllowPartialError] = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum request length for this upload profile, in kilobytes.
        /// </summary>
        public int MaxRequestLength
        {
            get
            {
                return (int)base[_propMaxRequestLength];
            }
            set
            {
                base[_propMaxRequestLength] = value;
            }
        }

        /// <summary>
        /// Gets the maximum request length for this upload profile, in bytes.
        /// </summary>
        public long MaxRequestLengthBytes
        {
            get
            {
                return (long)MaxRequestLength * 1024;
            }
        }

        /// <summary>
        /// Gets or sets the execution timeout for this upload profile, in seconds.
        /// </summary>
        public int ExecutionTimeout
        {
            get
            {
                return (int)base[_propExecutionTimeout];
            }
            set
            {
                base[_propExecutionTimeout] = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IUploadStreamProvider" /> configured for this upload profile.
        /// </summary>
        public UploadStreamProviderElement UploadStreamProvider
        {
            get
            {
                return (UploadStreamProviderElement)base[_propUploadStreamProvider];
            }
            set
            {
                base[_propUploadStreamProvider] = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IUploadFilter" /> configured for this upload profile.
        /// </summary>
        public UploadFilterElement UploadFilter
        {
            get
            {
                return (UploadFilterElement)base[_propUploadFilter];
            }
            set
            {
                base[_propUploadFilter] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadProfileElement" /> class.
        /// </summary>
        public UploadProfileElement()
        {
            _propName = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsKey);
            _propDocumentDomain = new ConfigurationProperty("documentDomain", typeof(string), null, ConfigurationPropertyOptions.None);
            _propMaxRequestLength = new ConfigurationProperty("maxRequestLength", typeof(int), 1024 * 1024 * 2, ConfigurationPropertyOptions.None);
            _propExecutionTimeout = new ConfigurationProperty("executionTimeout", typeof(int), 24 * 60 * 60, ConfigurationPropertyOptions.None);
            _propUploadStreamProvider = new ConfigurationProperty("uploadStreamProvider", typeof(UploadStreamProviderElement), null, ConfigurationPropertyOptions.None);
            _propUploadFilter = new ConfigurationProperty("uploadFilter", typeof(UploadFilterElement), null, ConfigurationPropertyOptions.None);
            _propAllowPartialError = new ConfigurationProperty("allowPartialError", typeof(bool), true, ConfigurationPropertyOptions.None);

            Properties.Add(_propName);
            Properties.Add(_propDocumentDomain);
            Properties.Add(_propMaxRequestLength);
            Properties.Add(_propExecutionTimeout);
            Properties.Add(_propUploadStreamProvider);
            Properties.Add(_propUploadFilter);
            Properties.Add(_propAllowPartialError);
        }
    }
}
