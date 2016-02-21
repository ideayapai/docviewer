using System.IO;

namespace Infrasturcture.Store
{
    public interface IStorePolicy
    {
        /// <summary>
        /// 添加本地文件
        /// </summary>
        /// <param name="filePath"></param>
        MetaInfo Add(string filePath, string remoteFile);

        /// <summary>
        /// 流模式添加文件
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <param name="stream"></param>
        MetaInfo AddStream(Stream stream, string mimetype, string remoteFile);

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <returns></returns>
        bool Exist(string remoteFile);

        /// <summary>
        /// 获取Meta属性
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <returns></returns>
        MetaInfo GetMeta(string remoteFile);

        /// <summary>
        /// 下载文档到本地
        /// </summary>
        /// <param name="localfile"></param>
        /// <param name="remoteFile"></param>
        void Copy(string localfile, string remoteFile);

        /// <summary>
        /// 获取流
        /// </summary>
        /// <param name="remoteFile"></param>
        /// <returns></returns>
        byte[] GetBytes(string remoteFile);


        
        /// <summary>
        /// 删除某个文件
        /// </summary>
        /// <param name="remoteFile"></param>
        void Delete(string remoteFile);

        /// <summary>
        /// 批量删除文件
        /// </summary>
        /// <param name="files"></param>
        void Delete(string[] remoteFiles);
    }
}
