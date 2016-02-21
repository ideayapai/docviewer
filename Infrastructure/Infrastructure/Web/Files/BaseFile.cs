namespace Infrasturcture.Web.Files
{
    public interface IBaseFile
    {
        /// <summary>
        /// 文件名
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// 文件长度
        /// </summary>
        long ContentLength { get; }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        void SaveAs(string path);

        /// <summary>
        /// 关闭文件
        /// </summary>
        void Close();

    }
}
