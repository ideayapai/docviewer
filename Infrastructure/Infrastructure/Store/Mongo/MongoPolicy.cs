using System;
using System.IO;
using Common.Logging;
using Infrasturcture.Store.Utils;
using MongoDB.Driver.GridFS;

namespace Infrasturcture.Store.Mongo
{
    public class MongoPolicy : IStorePolicy
    {
        private readonly MongoContext _context = new MongoContext();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();


        public MongoPolicy()
        {
            
        }

        public MongoPolicy(MongoContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 添加本地文件
        /// </summary>
        /// <param name="filePath">本地文件路径</param>
        /// <param name="remoteFile">服务Id</param>
        /// <returns></returns>
        public MetaInfo Add(string filePath, string remoteFile)
        {
            try
            {
                _logger.DebugFormat("Add File filePath:{0}, remoteId:{1}", filePath, remoteFile);

                MongoGridFSCreateOptions option = new MongoGridFSCreateOptions
                {
                    Id = remoteFile,
                    UploadDate = DateTime.Now,
                    ContentType = MimeMapper.GetMimeMapping(filePath),
                };

                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    MongoGridFS fs = new MongoGridFS(_context.DataBase);

                    var info = fs.Upload(stream, remoteFile, option);
                    return new MetaInfo
                    {
                        fileName = remoteFile,
                        MD5 = info.MD5,
                        MimeType = info.ContentType,
                    };
                }
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw;
            }
        }


        public Stream OpenStream(string remoteFile, string mimetype)
        {
            _logger.DebugFormat("Open MongoDB By Stream, Id {0}.", remoteFile);
            
            Stream stream = null;
            
            try
            {
                MongoGridFSCreateOptions option = new MongoGridFSCreateOptions
                {
                    Id = remoteFile,
                    UploadDate = DateTime.Now,
                    ContentType = mimetype,
                };

                MongoGridFS fs = new MongoGridFS(_context.DataBase);
                stream = fs.OpenWrite(remoteFile, option);
                return stream;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                if (stream != null)
                {
                    stream.Close();
                }
                throw;
            }

        }


        /// <summary>
        /// 流模式添加文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="remoteFile"></param>
        /// <returns></returns>
        public MetaInfo AddStream(Stream stream, string mimetype, string remoteFile)
        {
            _logger.DebugFormat("Add File By Stream, Id {0}.",remoteFile);

            try
            {
                MongoGridFSCreateOptions option = new MongoGridFSCreateOptions
                {
                    Id = remoteFile,
                    UploadDate = DateTime.Now,
                    ContentType = mimetype,
                };

                MongoGridFS fs = new MongoGridFS(_context.DataBase);
             
                var info = fs.Upload(stream, remoteFile, option);
                return new MetaInfo
                {
                    fileName = remoteFile,
                    MD5 = info.MD5,
                    MimeType = info.ContentType,
                };

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw;
            }
   
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <returns></returns>
        public bool Exist(string remoteFile)
        {
            _logger.DebugFormat("Exist, Id {0}.", remoteFile);

            try
            {
              
                MongoGridFS fs = new MongoGridFS(_context.DataBase);
                return fs.Exists(remoteFile);
              
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 获取元数据信息
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <returns></returns>
        public MetaInfo GetMeta(string remoteFile)
        {
            _logger.DebugFormat("GetMeta By Id{0}.", remoteFile);

            MongoGridFS fs = new MongoGridFS(_context.DataBase);
            if (!fs.Exists(remoteFile))
            {
                return null;
            }

            var info = fs.FindOne(remoteFile);
            return new MetaInfo
            {
                fileName = remoteFile,
                MD5 = info.MD5,
                MimeType = info.ContentType,
            };
        }

        /// <summary>
        /// 下载文档到本地
        /// </summary>
        /// <param name="localfile"></param>
        /// <param name="remoteFile"></param>
        public void Copy(string localfile, string remoteFile)
        {
            _logger.DebugFormat("Copy {0} to {1}.", localfile, remoteFile);

            try
            {
                MongoGridFS fs = new MongoGridFS(_context.DataBase);
                fs.Download(localfile, remoteFile);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 获取流
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <returns></returns>
        public byte[] GetBytes(string remoteFile)
        {
            _logger.DebugFormat("Get Stream by Id {0}", remoteFile);

            try
            {
                MongoGridFS fs = new MongoGridFS(_context.DataBase);
                byte[] bytes;
                using (MongoGridFSStream gfs = fs.OpenRead(remoteFile))
                {

                    bytes = new Byte[gfs.Length];
                    gfs.Read(bytes, 0, bytes.Length);
                }
                return bytes;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="remoteFile"></param>
        public void Delete(string remoteFile)
        {
            _logger.DebugFormat("Delete File, Id:{0}", remoteFile);
            
            try
            {
                MongoGridFS fs = new MongoGridFS(_context.DataBase);
                fs.Delete(remoteFile);
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                _logger.Error(ex.StackTrace);
                throw;
            }
        }

        /// <summary>
        /// 批量删除文件
        /// </summary>
        /// <param name="files"></param>
        public void Delete(string[] remoteFiles)
        {
            _logger.Debug("Delete Files");

            try
            {
                MongoGridFS fs = new MongoGridFS(_context.DataBase);
                foreach (string item in remoteFiles)
                {
                    fs.Delete(item);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                _logger.Error(ex.StackTrace);
                throw;
            }  
        }
       
    }
}
