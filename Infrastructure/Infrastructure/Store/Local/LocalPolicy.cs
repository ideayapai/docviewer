using System;
using System.IO;
using System.Web;

namespace Infrasturcture.Store.Local
{
    public class LocalPolicy : IStorePolicy
    {
        public MetaInfo Add(string filePath, string remoteFile)
        {
            if (File.Exists(remoteFile))
            {
                File.Delete(remoteFile);
            }
            File.Copy(filePath, remoteFile);
            return new MetaInfo {fileName = remoteFile, 
                                 MimeType = MimeMapping.GetMimeMapping(remoteFile)};
        }

        public MetaInfo AddStream(Stream stream, string mimetype, string remoteFile)
        {
            // 把 byte[] 写入文件
            using (FileStream fs = new FileStream(remoteFile, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    // 设置当前流的位置为流的开始
                    stream.Seek(0, SeekOrigin.Begin);

                    bw.Write(bytes);
                    bw.Close();
                    fs.Close();

                    return new MetaInfo
                    {
                        fileName = remoteFile,
                        MimeType = MimeMapping.GetMimeMapping(remoteFile)
                    };
                }
            }
           
        }

        public bool Exist(string remoteFile)
        {
            return File.Exists(remoteFile);
        }

        public MetaInfo GetMeta(string remoteFile)
        {
            return new MetaInfo
            {
                fileName = remoteFile,
                MimeType = MimeMapping.GetMimeMapping(remoteFile)
            };
        }

        public void Copy(string localfile, string remoteFile)
        {
            if (File.Exists(localfile))
            {
                File.Delete(localfile);
            }
            File.Copy(remoteFile, localfile);
           
        }

        public byte[] GetBytes(string remoteFile)
        {
            using (FileStream stream = new FileStream(remoteFile, FileMode.Open))
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                // 设置当前流的位置为流的开始
                stream.Seek(0, SeekOrigin.Begin);
                return bytes;
            }
        }

        public void Delete(string remoteFile)
        {
            if (File.Exists(remoteFile))
            {
                File.Delete(remoteFile);
            }
        }

        public void Delete(string[] remoteFiles)
        {
            foreach (var file in remoteFiles)
            {
                Delete(file);
            }
        }
    }
}
