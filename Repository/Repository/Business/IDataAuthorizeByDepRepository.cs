using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq.Expressions;

namespace Repository.Business
{
    public interface IDataAuthorizeByDepRepository<T>
    {

        List<TResult> Get<TResult>(string userName, Expression<Func<T, TResult>> selector);

        List<TResult> GetByUser<TResult>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition);

        List<TResult> GetByUser<TResult>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions);

        List<TResult> GetByUser<TResult, TKey>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int takeCount);

        List<TResult> GetByUser<TResult, TKey>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, Expression<Func<T, TKey>> orderBy, int start, int takeCount);

        List<T> GetUser(string userName, Expression<Func<T, bool>> condition);


        List<T> GetUser(string userName, Expression<Func<T, bool>> condition, int start, int take);

        List<T> GetByUser<TKey>(string userName, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int take);

        List<T> GetByUser<TKey>(string userName, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy);

        List<T> GetByUser(string userName, Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions);


        List<T> GetByUser(string userName, Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int take);


        List<T> GetByUser<TKey>(string userName, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int take);


        /// <summary>
        /// 根据当前用户递归查询
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="userName"></param>
        /// <param name="selector"></param>
        /// <param name="condition"></param>
        /// <param name="orderBy"></param>
        /// <param name="start"></param>
        /// <param name="takeCount"></param>
        /// <returns></returns>
        PaginationList<TResult> GetPagerByUser<TResult, TKey>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int takeCount);

        PaginationList<TResult> GetPagerByUser<TResult>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, string sort, string orderBy, int start, int takeCount);

        PaginationList<TResult> GetPagerByUser<TResult>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, int start, int takeCount);

        PaginationList<TResult> GetPagerByUser<TResult, TKey>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, Expression<Func<T, TKey>> orderBy, int start, int takeCount);

        PaginationList<TResult> GetPagerByUser<TResult>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, string sort, string orderBy, int start, int takeCount);



        PaginationList<T> GetPagerByUser(string userName, Expression<Func<T, bool>> condition, int start, int takeCount);


        PaginationList<T> GetPagerByUser<TKey>(string userName, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int takeCount);

        PaginationList<T> GetPagerByUser(string userName, Expression<Func<T, bool>> condition, string sort, string orderBy, int start, int takeCount);



        PaginationList<T> GetPagerByUser(string userName, Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount);


        PaginationList<T> GetPagerByUser<TKey>(string userName, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount);

        PaginationList<T> GetPagerByUser(string userName, Expression<Func<T, bool>> condition, string sort, string orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount);

        int CountByUser(string userName, Expression<Func<T, bool>> condition);

        List<T> GetByUser(string userName, Expression<Func<T, bool>> condition);
    }
}
