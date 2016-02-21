using System.Collections.Generic;

namespace Krystalware.SlickUpload.Blobify.Abstract
{
    public abstract class BlobInfo
    {
        Dictionary<string, string> _metadata;

        public string Name { get; set; }
        public long? Length { get; internal set; }

        public Dictionary<string, string> Metadata
        {
            get
            {
                if (_metadata == null)
                    _metadata = new Dictionary<string, string>();

                return _metadata;
            }
            set
            {
                _metadata = value;
            }
        }
    }
}
