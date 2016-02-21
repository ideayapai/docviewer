using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq.Expressions;

namespace Repository.Business
{
    public interface IDataAuthorizeRepository<T>
    {
        T Get(Expression<Func<T, bool>> condition);

        List<TResult> Get<TResult>(string createdBy, Expression<Func<T, TResult>> selector);

        List<TResult> GetByUser<TResult>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition);

        List<TResult> GetByUser<TResult>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions);

        List<TResult> GetByUser<TResult, TKey>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int takeCount);

        List<TResult> GetByUser<TResult, TKey>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, Expression<Func<T, TKey>> orderBy, int start, int takeCount);

        List<T> GetUser(string createdBy, Expression<Func<T, bool>> condition);


        List<T> GetUser(string createdBy, Expression<Func<T, bool>> condition, int start, int take);

        List<T> GetByUser<TKey>(string createdBy, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int take);

        List<T> GetByUser<TKey>(string createdBy, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy);

        List<T> GetByUser(string createdBy, Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions);


        List<T> GetByUser(string createdBy, Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int take);


        List<T> GetByUser<TKey>(string createdBy, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int take);


        /// <summary>
        /// 根据当前用户递归查询
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="createdBy"></param>
        /// <param name="selector"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="start"></param>
        /// <param name="takeCount"></param>
        /// <returns></returns>
        PaginationList<TResult> GetPagerByUser<TResult, TKey>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int takeCount);

        PaginationList<TResult> GetPagerByUser<TResult>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, string sort, string orderBy, int start, int takeCount);

        PaginationList<TResult> GetPagerByUser<TResult>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, int start, int takeCount);

        PaginationList<TResult> GetPagerByUser<TResult, TKey>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, Expression<Func<T, TKey>> orderBy, int start, int takeCount);

        PaginationList<TResult> GetPagerByUser<TResult>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, string sort, string orderBy, int start, int takeCount);



        PaginationList<T> GetPagerByUser(string createdBy, Expression<Func<T, bool>> condition, int start, int takeCount);


        PaginationList<T> GetPagerByUser<TKey>(string createdBy, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int takeCount);

        PaginationList<T> GetPagerByUser(string createdBy, Expression<Func<T, bool>> condition, string sort, string orderBy, int start, int takeCount);



        PaginationList<T> GetPagerByUser(string createdBy, Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount);


        PaginationList<T> GetPagerByUser<TKey>(string createdBy, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount);

        PaginationList<T> GetPagerByUser(string createdBy, Expression<Func<T, bool>> condition, string sort, string orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount);

        int CountByUser(string createdBy, Expression<Func<T, bool>> condition);
    }
}
