using System.IO;

namespace ImageStore.Services.Files
{
    class LocalFile : BaseFile
    {
        private readonly Stream _stream;
        private readonly string _fileName;
        
        public LocalFile(Stream stream)
        {
            _stream = stream;
        }

        public LocalFile(string fileName)
        {
            _fileName = fileName;
            _stream = new FileStream(fileName, FileMode.Open);
        }

        public Stream Stream
        {
            get { return _stream; }
        }

        public string FileName
        {
            get{ return _fileName;}
        }

        public long ContentLength
        {
            get { return _stream.Length; }
        }

        public void Close()
        {
            if (_stream != null)
            {
                _stream.Close();
            }
        }
    }
}
