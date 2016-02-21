using System.IO;

namespace Krystalware.SlickUpload.Blobify.Abstract
{
    public abstract class BlobClient<T> where T : BlobInfo, new()
    {
        public int BlockSize { get; set; }

        public abstract void PutBlob(T blobInfo, Stream data);
        public abstract Stream GetPutBlobStream(T blobInfo);
        public abstract void DeleteBlob(string name);
        public abstract Stream GetBlob(string name);

        public void PutBlob(string name, Stream data)
        {
            PutBlob(CreateBlobInfo(name, data), data);
        }

        public Stream GetPutBlobStream(string name, long? length)
        {
            return GetPutBlobStream(CreateBlobInfo(name, length));
        }

        protected T CreateBlobInfo(string name, Stream data)
        {
            long? length;

            try
            {
                length = data.Length;
            }
            catch
            {
                length = null;
            }

            return CreateBlobInfo(name, length);
        }

        protected T CreateBlobInfo(string name, long? length)
        {
            return new T() { Name = name, Length = length };
        }

        //public abstract void PutBlob(string name, string container, Stream data, long length);
        //public abstract void PutBlob(string name, string container, Action<Stream> writeAction);
    }
}
