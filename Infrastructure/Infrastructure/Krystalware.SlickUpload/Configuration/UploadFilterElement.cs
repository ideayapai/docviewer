using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections;
using Krystalware.SlickUpload.Storage;
using System.Web;

namespace Krystalware.SlickUpload.Configuration
{
    /// <summary>
    /// Defines the configuration for an <see cref="IUploadFilter"/>.
    /// </summary>
    public class UploadFilterElement : TypeElementBase
    {
        /// <summary>
        /// Constructs and returns the <see cref="IUploadFilter" /> defined by the current configuration.
        /// </summary>
        /// <returns>The <see cref="IUploadFilter" /> defined by the current configuration.</returns>
        public IUploadFilter Create()
        {
            if (!string.IsNullOrEmpty(Type))
            {
                IUploadFilter filter = null;
                Exception innerEx = null;

                try
                {
                    filter = (IUploadFilter)TypeFactory.CreateInstance(Type, new object[] { this });
                }
                catch (Exception ex)
                {
                    innerEx = ex;
                }

                if (filter == null)
                    throw new Exception("Couldn't create UploadFilter for type '" + Type + "'.", innerEx);

                return filter;               
            }
            else
                return null;
        }
    }
}
