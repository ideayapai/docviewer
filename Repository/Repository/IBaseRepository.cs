using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq.Expressions;

namespace Repository
{
    public interface IBaseRepository<T>
    {
        /// <summary>
        /// 根据给定条件获取唯一实体
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <summary>
        /// 根据给定条件获取唯一实体
        /// </summary>
        /// <param name="condition">筛选条件</param>
        T Get(Expression<Func<T, bool>> condition);


        T GetUseExpression(Expression<Func<T, bool>> condition);

        /// <summary>
        /// 根据给定条件获取唯一实体
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <param name="howToGetLoadOptions">级联查询项</param>
        T Get(Func<T, bool> condition, Func<DataLoadOptions> howToGetLoadOptions);

        List<TResult> Get<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition);

        T GetToSql(DateTime dateTime, string category);

        string GetTableName();

     

        /// <summary>
        /// 获取某字段列表
        /// </summary>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="selector">需要检索的字段</param>
        /// <param name="condition">检索条件</param>
        List<TResult> Get<TResult>(Func<T, TResult> selector, Func<T, bool> condition);

        List<TResult> Get<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions);

        List<TResult> GetUseExpression<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition);

        /// <summary>
        /// 根据给定条件获取实体集
        /// </summary>
        /// <param name="condition">筛选条件</param>
        List<T> GetAll(Expression<Func<T, bool>> condition);


        List<T> GetAllUseExpression(Expression<Func<T, bool>> condition);

        /// <summary>
        /// 根据给定条件获取实体集
        /// </summary>
        /// <param name="condition">筛选条件</param>
        List<T> GetAllToSql(string sqlCmd, Func<DataLoadOptions> loadOptions);

        /// <summary>
        /// 根据给定条件获取实体集
        /// </summary>
        /// <param name="condition">筛选条件</param>
        List<T> GetAllToSql(string sqlCmd);

        List<T> GetAllByExp(Expression<Func<T, bool>> expWhere);


        /// <summary>
        /// 根据给定条件获取特定范围的实体集
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <param name="start">开始位置</param>
        /// <param name="take">检索条件</param>
        List<T> GetAll(Expression<Func<T, bool>> condition, int start, int take);

        /// <summary>
        /// 根据给定条件获取特定范围的实体集
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <param name="orderBy">排序</param>
        /// <param name="start">开始位置</param>
        /// <param name="take">检索条件</param>
        List<T> GetAll<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int take);

        List<T> GetAll<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy);

        /// <summary>
        /// 根据给定条件及级联条件获取实体集
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <param name="howToGetLoadOptions">级联查询项</param>
        List<T> GetAll(Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions);

        /// <summary>
        /// 根据给定条件及级联条件获取特定范围的实体集
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <param name="howToGetLoadOptions">级联查询项</param>
        /// <param name="start">开始位置</param>
        /// <param name="take">结束位置</param>
        List<T> GetAll(Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int take);

        /// <summary>
        /// 根据给定条件及级联条件获取特定范围的实体集
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <param name="orderBy">排序</param>
        /// <param name="howToGetLoadOptions">级联查询项</param>
        /// <param name="start">开始位置</param>
        /// <param name="take">结束位置</param>
        List<T> GetAll<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int take);

        /// <summary>
        /// 获取最后一条数据
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        T GetLast<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> order);

        /// <summary>
        /// 插入新实体
        /// </summary>
        /// <param name="entity">新实体</param>
        T Add(T entity);

        /// <summary>
        /// 插入实体集
        /// </summary>
        /// <param name="entities">新实体集</param>
        List<T> Add(List<T> entities);

        /// <summary>
        /// 插入新实体，并根据级联条件返回插入后的实体
        /// </summary>
        /// <param name="entity">新实体</param>
        /// <param name="howToGetLoadOptions">级联查询项</param>
        T Add(T entity, Func<DataLoadOptions> howToGetLoadOptions);

        /// <summary>
        /// 更新实体，此方法更新整体实体
        /// </summary>
        /// <param name="condition">获取唯一实体的条件</param>
        /// <param name="newEntity">包含新数据的实体</param>
        T Update(Func<T, bool> condition, T newEntity);

        /// <summary>
        /// 更新实体，此方法只更新指定字段
        /// </summary>
        /// <param name="condition">获取唯一实体的条件</param>
        /// <param name="newEntity">包含新数据的实体</param>
        /// <param name="includeProperties">需要更新的字段列表</param>
        T Update(Func<T, bool> condition, T newEntity, string[] includeProperties);

        /// <summary>
        /// 更新实体，此方法采用给定的键/值更新实体
        /// </summary>
        /// <param name="condition">获取唯一实体的条件</param>
        /// <param name="updateField">包含新数据的键/值列表</param>
        T Update(Func<T, bool> condition, Dictionary<string, object> updateField);

        /// <summary>
        /// 更新实体，此方法采用自定义方法更新实体
        /// </summary>
        /// <param name="condition">获取唯一实体的条件</param>
        /// <param name="howToUpdateEntity">自定义更新实体的方法</param>
        T Update(Func<T, bool> condition, Action<T> howToUpdateEntity);

        /// <summary>
        /// 更新实体，此方法采用自定义方法更新实体
        /// </summary>
        /// <param name="condition">获取唯一实体的条件</param>
        /// <param name="howToUpdateEntity">自定义更新实体的方法</param>
        List<T> Update(Func<T, bool> condition, Action<IEnumerable<T>> howToUpdateEntity);

        /// <summary>
        /// 采用相同的值更新符合条件的所有项
        /// </summary>
        /// <param name="condition">更新条件</param>
        /// <param name="howToUpdateEntity">设置新值的方法</param>
        List<T> UpdateAll(Func<T, bool> condition, Action<T> howToUpdateEntity);


        /// <summary>
        /// 删除一条或多条数据
        /// </summary>
        /// <param name="condition">筛选条件</param>
        bool Delete(Func<T, bool> condition);

        /// <summary>
        /// 根据sql语句删除一条或多条数据
        /// </summary>
        /// <param name="sqlCmd"></param>
        /// <returns></returns>
        bool Delete(string sqlCmd);

        /// <summary>
        /// 删除一条或多条数据，此方法直接执行脚本删除数据。脚本形如：delete from tb where tb.id in(1,2,3)
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <param name="keys">主键</param>
        /// <param name="primaryKey">主键名</param>
        bool Delete(Func<T, bool> condition, Func<T, int> keys, string primaryKey);

        /// <summary>
        /// 统计复合给定条件的记录数
        /// </summary>
        /// <param name="condition">筛选条件</param>
        int Count(Expression<Func<T, bool>> condition);

        int Update(Criteria criteria);

        bool Update(string cmdText);

        #region Pager

        /// <summary>
        /// 获取某字段列表
        /// </summary>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <typeparam name="TKey">排序的属性</typeparam>
        /// <param name="selector">需要检索的字段</param>
        /// <param name="condition">检索条件</param>
        /// <param name="orderBy">排序</param>
        /// <param name="start"></param>
        /// <param name="takeCount"></param>
        PaginationList<TResult> GetPager<TResult, TKey>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int takeCount);

        PaginationList<TResult> GetPager<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, string sort, string orderBy, int start, int takeCount);

        PaginationList<TResult> GetPager<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, int start, int takeCount);

        PaginationList<TResult> GetPager<TResult, TKey>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, Expression<Func<T, TKey>> orderBy, int start, int takeCount);

        PaginationList<TResult> GetPager<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, string sort, string orderBy, int start, int takeCount);


        /// <summary>
        /// 根据给定条件获取特定范围的实体集
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <param name="start">开始位置</param>
        /// <param name="takeCount">获取的数量</param>
        PaginationList<T> GetPager(Expression<Func<T, bool>> condition, int start, int takeCount);

        /// <summary>
        /// 根据给定条件获取特定范围的实体集
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <param name="orderBy">排序</param>
        /// <param name="start">开始位置</param>
        /// <param name="takeCount">获取的数量</param>
        PaginationList<T> GetPager<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int takeCount);

        PaginationList<T> GetPager(Expression<Func<T, bool>> condition, string sort, string orderBy, int start, int takeCount);


        /// <summary>
        /// 根据给定条件及级联条件获取特定范围的实体集
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <param name="howToGetLoadOptions">级联查询项</param>
        /// <param name="start">开始位置</param>
        /// <param name="takeCount">获取的数量</param>
        PaginationList<T> GetPager(Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount);

        /// <summary>
        /// 根据给定条件及级联条件获取特定范围的实体集
        /// </summary>
        /// <param name="condition">筛选条件</param>
        /// <param name="orderBy">排序</param>
        /// <param name="howToGetLoadOptions">级联查询项</param>
        /// <param name="start">开始位置</param>
        /// <param name="takeCount">获取的数量</param>
        PaginationList<T> GetPager<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount);

        PaginationList<T> GetPager(Expression<Func<T, bool>> condition, string sort, string orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount);


        #endregion

        ConnectionState State { get; }
    }
}