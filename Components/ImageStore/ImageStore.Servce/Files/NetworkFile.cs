using System.IO;
using System.Web;

namespace ImageStore.Services.Files
{
    internal class NetworkFile: BaseFile
    {
        private readonly HttpPostedFileBase _httpPostedFile;

        public NetworkFile(HttpPostedFileBase httpPostedFile)
        {
            _httpPostedFile = httpPostedFile;
        }

        public Stream Stream
        {
            get
            {
                return _httpPostedFile.InputStream;
            }
        }

        public string FileName
        {
            get
            {
                return _httpPostedFile.FileName;
            }
        }

        public long ContentLength
        {
            get
            {
                return _httpPostedFile.ContentLength;
            }
        }

        public void Close()
        {
            
        }
    }
}
