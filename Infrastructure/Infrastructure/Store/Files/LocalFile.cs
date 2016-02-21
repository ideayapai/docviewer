using System.IO;

namespace Infrasturcture.Store.Files
{
    public class LocalFile : BaseFile
    {
        private readonly string _fileName;
        
        public LocalFile(string fileName)
        {
            _fileName = fileName;
         
        }

        public override string FileName
        {
            get{ return _fileName;}
        }

        public override Stream FileStream
        {
            get
            {
                
                return File.OpenRead(_fileName);
               
                //return new FileStream(_fileName, FileMode.Open);
            }
            
        }

        public override long ContentLength
        {
            get
            {
                var fileInfo = new FileInfo(_fileName);
                return fileInfo.Length;
            }
        }

        public override void SaveAs(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.Copy(_fileName, path);
     
        }
    }
}
