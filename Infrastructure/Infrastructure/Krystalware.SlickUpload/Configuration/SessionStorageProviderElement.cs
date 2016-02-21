using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections;
using Krystalware.SlickUpload.Storage;
using Krystalware.SlickUpload.Web.SessionStorage;

namespace Krystalware.SlickUpload.Configuration
{
    /// <summary>
    /// Defines configuration of the SlickUpload session storage provider.
    /// </summary>
    public class SessionStorageProviderElement : TypeElementBase
    {
        readonly ConfigurationProperty _propUpdateInterval;
        readonly ConfigurationProperty _propStaleTimeout;

        /// <summary>
        /// Gets or sets the session update interval, in seconds.
        /// </summary>
        public int UpdateInterval
        {
            get
            {
                return (int)base[_propUpdateInterval];
            }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("UpdateInterval must be greater than 1 second.");

                base[_propUpdateInterval] = value;
            }
        }

        /// <summary>
        /// Gets or sets the session timeout, in seconds.
        /// </summary>
        public int StaleTimeout
        {
            get
            {
                return (int)base[_propStaleTimeout];
            }
            set
            {
                if (value < 60)
                    throw new ArgumentOutOfRangeException("StaleTimeout must be greater than 60 seconds.");

                base[_propStaleTimeout] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionStorageProviderElement" /> class.
        /// </summary>
        public SessionStorageProviderElement()
            : base()
        {
            _propUpdateInterval = new ConfigurationProperty("updateInterval", typeof(int), 1, ConfigurationPropertyOptions.None);
            _propStaleTimeout = new ConfigurationProperty("staleTimeout", typeof(int), 30 * 60, ConfigurationPropertyOptions.None);

            PropertiesInternal.Add(_propUpdateInterval);
            PropertiesInternal.Add(_propStaleTimeout);
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override bool IsImplementedProperty(string name)
        {
            return base.IsImplementedProperty(name) || name == "updateInterval" || name == "staleTimeout";
        }

        /// <summary>
        /// Constructs and returns the <see cref="ISessionStorageProvider" /> defined by the current configuration.
        /// </summary>
        /// <returns>The <see cref="ISessionStorageProvider" /> defined by the current configuration.</returns>
        public ISessionStorageProvider Create()
        {
            if (string.IsNullOrEmpty(Type) || string.Equals(Type, "adaptive", StringComparison.InvariantCultureIgnoreCase))
                return new AdaptiveSessionStorageProvider(this);
            else if (string.Equals(Type, "inproc", StringComparison.InvariantCultureIgnoreCase))
                return new InProcSessionStorageProvider(this);
            else if (string.Equals(Type, "sessionstate", StringComparison.InvariantCultureIgnoreCase))
                return new SessionStateSessionStorageProvider(this);
            else if (string.Equals(Type, "sqlclient", StringComparison.InvariantCultureIgnoreCase))
                return new SqlClientSessionStorageProvider(this);
            else
            {
                ISessionStorageProvider provider = null;
                Exception innerEx = null;

                try
                {
                    provider = (ISessionStorageProvider)TypeFactory.CreateInstance(Type, new object[] { this });
                }
                catch (Exception ex)
                {
                    innerEx = ex;
                }

                if (provider == null)
                    throw new Exception("Couldn't create SessionStorageProvider for type '" + Type + "'.", innerEx);

                return provider;
            }
        }
    }
}
