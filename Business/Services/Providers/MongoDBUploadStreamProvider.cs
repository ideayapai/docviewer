using System;
using System.IO;
using Infrasturcture.Store.Mongo;
using Infrasturcture.Utils;
using Krystalware.SlickUpload;
using Krystalware.SlickUpload.Configuration;
using Krystalware.SlickUpload.Storage;

namespace Services.Providers
{
    public class MongoDBUploadStreamProvider : UploadStreamProviderBase
    {
        private static readonly MongoPolicy MongoPolicy = new MongoPolicy();
        private string storedir;

        /// <summary>
        /// 获取文档存储路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetStorePath(string fileName)
        {
            fileName = DateTime.Now.ToString("yyyy_mm_dd_hh_mm_ssss") + "_" + Path.GetFileName(fileName);
            fileName = StringUtils.RemoveAllEmpty(fileName);
            
            if (!storedir.EndsWith("\\"))
            {
                storedir += "\\";
            }

            return storedir + fileName;
        }

        public MongoDBUploadStreamProvider(UploadStreamProviderElement settings) : base(settings)
        {
            storedir = Settings.Parameters["storedir"];
            if (string.IsNullOrWhiteSpace(storedir))
            {
                throw new Exception("StoreDir must be specified for SlickUpload MongoDB provider");
            }
        }

        public override Stream GetWriteStream(UploadedFile file)
        {
            Stream s = null;

            try
            {
                string fileName = GetStorePath(file.ClientName);
                file.ServerLocation = fileName;
                //mongoPolicy.AddStream(file.ServerLocation, file.ContentType, file.ServerLocation);
                s = MongoPolicy.OpenStream(fileName, file.ContentType);
                return s;
            
            }
            catch (Exception e)
            {
                if (s != null)
                {
                    CloseWriteStream(file, s, false);
                    RemoveOutput(file);
                }
                throw;
            }

        }

        public override void RemoveOutput(UploadedFile file)
        {
            //MongoPolicy.Delete(file.ServerLocation);
            // If it's a valid upload
            if (!string.IsNullOrEmpty(file.ServerLocation) && MongoPolicy.Exist(file.ServerLocation))
            {
                MongoPolicy.Delete(file.ServerLocation);
            }
        }

        public override Stream GetReadStream(UploadedFile file)
        {
            //mongoPolicy.GetBytes(file.ServerLocation);
            throw new NotImplementedException();
        }
    }
}
