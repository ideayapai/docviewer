using System.IO;

namespace Infrasturcture.Web.Files
{
    public class LocalFile : IBaseFile
    {
        private readonly string _fileName;
        
        public LocalFile(string fileName)
        {
            _fileName = fileName;
         
        }

        public string FileName
        {
            get{ return _fileName;}
        }

        public long ContentLength
        {
            get
            {
                var fileInfo = new FileInfo(_fileName);
                return fileInfo.Length;
            }
        }

        public void SaveAs(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.Copy(_fileName, path);
     
        }

        public void Close()
        {
           
        }
    }
}
