using System.IO;
using System.Web;

namespace Infrasturcture.Web.Files
{
    public class HttpFile: IBaseFile
    {
        private readonly HttpPostedFileBase _httpPostedFile;

        public HttpFile(HttpPostedFileBase httpPostedFile)
        {
            _httpPostedFile = httpPostedFile;
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

        public void SaveAs(string path)
        {
            _httpPostedFile.SaveAs(path);
        }

        public void Close()
        {
            
        }
    }
}
