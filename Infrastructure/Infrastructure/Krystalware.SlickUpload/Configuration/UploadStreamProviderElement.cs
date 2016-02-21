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
    /// Defines the configuration for an <see cref="IUploadStreamProvider" />.
    /// </summary>
    public class UploadStreamProviderElement : TypeElementBase
    {
        /// <summary>
        /// Constructs and returns the <see cref="IUploadStreamProvider" /> defined by the current configuration.
        /// </summary>
        /// <returns>The <see cref="IUploadStreamProvider" /> defined by the current configuration.</returns>
        public IUploadStreamProvider Create()
        {
            if (string.IsNullOrEmpty(Type) || string.Equals(Type, "file", StringComparison.InvariantCultureIgnoreCase))
                return new FileUploadStreamProvider(this);
            else if (string.Equals(Type, "sqlclient", StringComparison.InvariantCultureIgnoreCase))
                return new SqlClientUploadStreamProvider(this);
            //else if (string.Equals(Type, "sqlfilestream", StringComparison.InvariantCultureIgnoreCase))
            //    return new SqlFileStreamUploadStreamProvider(this);
            else if (string.Equals(Type, "memory", StringComparison.InvariantCultureIgnoreCase))
                return new MemoryUploadStreamProvider(this);
            else if (string.Equals(Type, "s3", StringComparison.InvariantCultureIgnoreCase))
                return new S3UploadStreamProvider(this);
            else if (string.Equals(Type, "null", StringComparison.InvariantCultureIgnoreCase))
                return new NullUploadStreamProvider(this);
            else
            {
                IUploadStreamProvider provider = null;
                Exception innerEx = null;

                try
                {
                    provider = (IUploadStreamProvider)TypeFactory.CreateInstance(Type, new object[] { this });
                }
                catch (Exception ex)
                {
                    innerEx = ex;
                }

                if (provider == null)
                    throw new Exception("Couldn't create UploadStreamProvider for type '" + Type + "'.", innerEx);

                return provider;
            }
        }
    }
}
