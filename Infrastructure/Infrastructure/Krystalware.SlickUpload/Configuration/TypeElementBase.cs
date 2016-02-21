using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections;
using Krystalware.SlickUpload.Storage;

namespace Krystalware.SlickUpload.Configuration
{
    /// <summary>
    /// Defines an abstract base class for type configuration elements.
    /// </summary>
    public abstract class TypeElementBase : ConfigurationElement
    {
        readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        readonly ConfigurationProperty _propName;
        readonly ConfigurationProperty _propType;

        /// <summary>
        /// Gets the internal <see cref="ConfigurationPropertyCollection" /> for this element.
        /// </summary>
        protected ConfigurationPropertyCollection PropertiesInternal { get { return _properties; } }

        NameValueCollection _propertyNames;

        /// <summary>
        /// Gets or sets the name of this element.
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
        /// Gets or sets a type reference for this element.
        /// </summary>
        public string Type
        {
            get
            {
                return (string)base[_propType];
            }
            set
            {
                base[_propType] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeElementBase" /> class.
        /// </summary>
        protected TypeElementBase()
        {
            _propName = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsKey);
            _propType = new ConfigurationProperty("type", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

            _properties.Add(_propName);
            _properties.Add(_propType);
        }

        private string GetProperty(string name)
        {
            if (this._properties.Contains(name))
            {
                ConfigurationProperty property = this._properties[name];

                if (property != null)
                    return (string)base[property];
            }

            return null;
        }

        private bool SetProperty(string name, string value)
        {
            ConfigurationProperty property = null;

            if (_properties.Contains(name))
            {
                property = _properties[name];
            }
            else
            {
                property = new ConfigurationProperty(name, typeof(string), null);

                _properties.Add(property);
            }

            if (property != null)
            {
                base[property] = value;

                return true;
            }

            return false;
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override bool IsModified()
        {
            if (!this.UpdatePropertyCollection())
                return base.IsModified();

            return true;
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            ConfigurationProperty property = new ConfigurationProperty(name, typeof(string), value);

            _properties.Add(property);

            base[property] = value;

            Parameters[name] = value;

            return true;
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override void Reset(ConfigurationElement parentElement)
        {
            UploadStreamProviderElement settings = parentElement as UploadStreamProviderElement;

            if (settings != null)
                settings.UpdatePropertyCollection();

            base.Reset(parentElement);
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode)
        {
            UploadStreamProviderElement settings = parentElement as UploadStreamProviderElement;
            
            if (settings != null)
                settings.UpdatePropertyCollection();
            
            UploadStreamProviderElement settings2 = sourceElement as UploadStreamProviderElement;

            if (settings2 != null)
                settings2.UpdatePropertyCollection();

            base.Unmerge(sourceElement, parentElement, saveMode);

            UpdatePropertyCollection();
        }

        internal bool UpdatePropertyCollection()
        {
            bool isModified = false;
            ArrayList newPropertyNames = null;

            if (this._propertyNames != null)
            {
                foreach (ConfigurationProperty property in this._properties)
                {
                    if (IsImplementedProperty(property.Name) || _propertyNames.Get(property.Name) != null)
                        continue;

                    if (newPropertyNames == null)
                        newPropertyNames = new ArrayList();

                    newPropertyNames.Add(property.Name);

                    isModified = true;
                }

                if (newPropertyNames != null)
                {
                    foreach (string name in newPropertyNames)
                        _properties.Remove(name);
                }

                foreach (string name in _propertyNames)
                {
                    string oldVal = _propertyNames[name];
                    string newVal = GetProperty(name);

                    if (newVal == null || oldVal != newVal)
                    {
                        SetProperty(name, oldVal);

                        isModified = true;
                    }
                }
            }

            return isModified;
        }

        /// <summary>
        /// Returns a boolean that specifies whether the specified property name is implemented directly by this type element.
        /// </summary>
        /// <param name="name">The name of the property to check.</param>
        /// <returns>A boolean that specifies whether the specified property name is implemented directly by this type element.</returns>
        protected virtual bool IsImplementedProperty(string name)
        {
            return name == "name" || name == "type";
        }

        /// <summary>
        /// Gets a collection of the properties configured for this element.
        /// </summary>
        public NameValueCollection Parameters
        {
            get
            {
                if (_propertyNames == null)
                {
                    lock (this)
                    {
                        if (_propertyNames == null)
                        {
                            _propertyNames = new NameValueCollection(StringComparer.InvariantCulture);

                            foreach (ConfigurationProperty property in this._properties)
                            {
                                if (!IsImplementedProperty(property.Name))
                                    this._propertyNames.Add(property.Name, (string)base[property]);
                            }
                        }
                    }
                }

                return _propertyNames;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                UpdatePropertyCollection();

                return _properties;
            }
        }
    }
}
