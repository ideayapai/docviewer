using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web;
using Krystalware.SlickUpload.Web;
using System.Web.Configuration;

namespace Krystalware.SlickUpload.Configuration
{
    /// <summary>
    /// Defines the configuration for a collection of <see cref="UploadProfileElement" /> elements.
    /// </summary>
    public class UploadProfileCollection : ConfigurationElementCollection
    {
        static string _defaultProfile;

        /// <summary>
        /// Gets or sets the name of the default profile.
        /// </summary>
        [ConfigurationProperty("defaultProfile", DefaultValue=null)]
        public string DefaultProfile
        {
            get
            {
                return !string.IsNullOrEmpty((string)base["defaultProfile"]) ? (string)base["defaultProfile"] :_defaultProfile;
            }
            set
            {
                base["defaultProfile"] = value;
            }
        }

        internal UploadProfileElement GetUploadProfileElement(string uploadProfile, bool throwIfNotExists)
        {
            // TODO: figure out why this doesn't work with merged config
            if (string.IsNullOrEmpty(uploadProfile))
                uploadProfile = DefaultProfile;

            if (string.IsNullOrEmpty(uploadProfile))
            {
                if (Count == 1)
                    return this[0];
                else if (throwIfNotExists)
                    throw new Exception("Either defaultProfile or a per-request uploadProfile must be specified when more than one uploadProfile exists.");
            }
            else
            {
                UploadProfileElement profile = this[uploadProfile];

                if (profile != null)
                    return profile;
                else if (throwIfNotExists)
                    throw new Exception("Couldn't find uploadProfile '" + uploadProfile + "'.");
            }

            return null;
        }

        // TODO: see if there is a better way
        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            string defaultProfile = reader.GetAttribute("defaultProfile");

            if (!string.IsNullOrEmpty(defaultProfile))
                _defaultProfile = defaultProfile;

            base.DeserializeElement(reader, serializeCollectionKey);
        }

        // This makes DefaultProfile actually work: http://www.frankwisniewski.net/2011/12/how-to-use-a-configurationelementcollection-with-custom-attributes/
        // Nope, actually this is teh brokez
        /*protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            if (name.Equals("defaultProfile", StringComparison.InvariantCultureIgnoreCase))
            {
                DefaultProfile = value;

                return true;
            }
            else
            {
                return base.OnDeserializeUnrecognizedAttribute(name, value);
            }
        }*/

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new UploadProfileElement();
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return base.CreateNewElement(elementName);
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((UploadProfileElement)element).Name ?? "null";
        }

        /// <summary>
        /// Gets or sets a value at the specified index in the <see cref="UploadProfileElement" /> collection.
        /// </summary>
        /// <param name="index">The index of the <see cref="UploadProfileElement" /> to return.</param>
        /// <returns>The specified <see cref="UploadProfileElement" />.</returns>
        public UploadProfileElement this[int index]
        {
            get
            {
                return (UploadProfileElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Gets an item from the collection.
        /// </summary>
        /// <param name="name">The name of the  <see cref="UploadProfileElement" /> object within the collection.</param>
        /// <returns>A <see cref="UploadProfileElement" /> object contained in the collection.</returns>
        public new UploadProfileElement this[string name]
        {
            get
            {
                return (UploadProfileElement)BaseGet(name);
            }
        }

        /// <summary>
        /// Returns the index of the specified <see cref="UploadProfileElement" /> within the collection.
        /// </summary>
        /// <param name="element">The <see cref="UploadProfileElement" /> for which to return the specified index location.</param>
        /// <returns>The index of the specified <see cref="UploadProfileElement" />; otherwise, -1.</returns>
        public int IndexOf(UploadProfileElement element)
        {
            return BaseIndexOf(element);
        }

        /// <summary>
        /// Adds a <see cref="UploadProfileElement" /> object to the collection.
        /// </summary>
        /// <param name="element">The <see cref="UploadProfileElement" /> to add.</param>
        public void Add(UploadProfileElement element)
        {
            if (element != null)
            {
                //element.UpdatePropertyCollection();

                BaseAdd(element);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        /// <summary>
        /// Removes the <see cref="UploadProfileElement" /> at the specified index location.
        /// </summary>
        /// <param name="index">The index location of the <see cref="UploadProfileElement" /> to remove. </param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        /// Removes a <see cref="UploadProfileElement" /> from the collection.
        /// </summary>
        /// <param name="name">The name of the <see cref="UploadProfileElement" /> to remove.</param>
        public void Remove(string name)
        {
            BaseRemove(name);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }
    }
}
