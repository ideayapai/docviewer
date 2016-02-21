using System.IO;
using System.Web;

namespace Infrasturcture.Store.Files
{
    public class HttpFile: BaseFile
    {
        private readonly HttpPostedFileBase _httpPostedFile;

        public HttpFile(HttpPostedFileBase httpPostedFile)
        {
            _httpPostedFile = httpPostedFile;
        }

        public override string FileName
        {
            get
            {
                return _httpPostedFile.FileName;
            }
        }

        public override Stream FileStream
        {
            get { return _httpPostedFile.InputStream; }
        }

        public override long ContentLength
        {
            get
            {
                return _httpPostedFile.ContentLength;
            }
        }

        public override void SaveAs(string path)
        {
            _httpPostedFile.SaveAs(path);
        }

        public void Close()
        {
            
        }
    }
}
