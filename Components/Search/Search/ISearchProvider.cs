using System.Collections.Generic;
using Search.QueryBuilder;

namespace Search
{
    public interface ISearchProvider
    {
        /// <summary>
        /// 注册读取文档内容接口
        /// </summary>
        /// <param name="reader"></param>
        void setFileReader(IFileContentReader reader);

        /// <summary>
        /// 设置索引目录
        /// </summary>
        string IndexDirectory { get; set; }

        /// <summary>
        /// 查询关键字的接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryString"></param>
        /// <returns></returns>
        List<T> Query<T>(string queryString) where T : class, new();

        /// <summary>
        /// 查询的接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        List<T> Query<T>(BaseQuery query) where T : class, new();

        /// <summary>
        /// 查询分页的接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryString"></param>
        /// <param name="start"></param>
        /// <param name="takeSize"></param>
        /// <returns></returns>
        List<T> Query<T>(string queryString, int? start, int? takeSize) where T : class, new();
        
        /// <summary>
        /// 查询范围内的接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryString"></param>
        /// <param name="range"></param>
        /// <param name="start"></param>
        /// <param name="takeSize"></param>
        /// <returns></returns>
        List<T> Query<T>(BaseQuery query, int? start, int? takeSize) where T : class, new();

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        void Add<T>(T source) where T : class, new();

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        void Add<T>(T source, bool rewrite) where T : class, new();

        /// <summary>
        /// 批量添加索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        void AddList<T>(IList<T> source) where T : class, new();
        
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sources"></param>
        /// <param name="rewrite"></param>
        void AddList<T>(IList<T> sources, bool rewrite) where T : class, new();

        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Delete<T>(T source) where T : class;

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        bool DeleteList<T>(IList<T> source) where T : class;
    }
}
