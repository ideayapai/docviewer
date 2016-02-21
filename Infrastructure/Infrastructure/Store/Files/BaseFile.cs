using System;
using System.IO;
using System.Web.Security;
using Infrasturcture.Store.Utils;

namespace Infrasturcture.Store.Files
{
    public abstract class BaseFile
    {
        private Guid id;

        public Guid Id
        {
            get
            {
                if (id == default(Guid))
                {
                    id = Guid.NewGuid();
                }
                return id;
            }
        }

        /// <summary>
        /// 文件名
        /// </summary>
        public abstract string FileName { get; }


        public string MimeType
        {
            get { return MimeMapper.GetMimeMapping(FileName); }
        }


        /// <summary>
        /// 文件流
        /// </summary>
        public abstract Stream FileStream { get; }

        /// <summary>
        /// 文件长度
        /// </summary>
        public abstract long ContentLength { get; }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        public abstract void SaveAs(string path);


    }
}
