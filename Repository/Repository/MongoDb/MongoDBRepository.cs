using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq.Expressions;
using MongoDB.Driver;
using Repository.SqlServer;

namespace Repository.MongoDb
{
    public class MongoDBBaseRepository<T, U> : IBaseRepository<T>
        where U : RepositoryContext, new()
        where T : class
    {
        
        //获得数据库cnblogs MongoDatabase db = server.GetDatabase(dbName);
        public T Get(Expression<Func<T, bool>> condition)
        {
            using (var db1 = new U())
            {
                 
               //MongoServer server = MongoServer.Create(strconn);
               //MongoDatabase db = server.GetDatabase(dbName);
               //MongoCollection col = db.GetCollection("Users");
            }
            throw new NotImplementedException();
        }

        public T GetUseExpression(Expression<Func<T, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public T Get(Func<T, bool> condition, Func<DataLoadOptions> howToGetLoadOptions)
        {
            throw new NotImplementedException();
        }

        public List<TResult> Get<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public T GetToSql(DateTime dateTime, string category)
        {
            throw new NotImplementedException();
        }

        public string GetTableName()
        {
            throw new NotImplementedException();
        }

        public List<TResult> Get<TResult>(Func<T, TResult> selector, Func<T, bool> condition)
        {
            throw new NotImplementedException();
        }

        public List<TResult> Get<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions)
        {
            throw new NotImplementedException();
        }

        public List<TResult> GetUseExpression<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll(Expression<Func<T, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAllUseExpression(Expression<Func<T, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAllToSql(string sqlCmd, Func<DataLoadOptions> loadOptions)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAllToSql(string sqlCmd)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAllByExp(Expression<Func<T, bool>> expWhere)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll(Expression<Func<T, bool>> condition, int start, int take)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int take)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll(Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll(Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int take)
        {
            throw new NotImplementedException();
        }

        public List<T> GetAll<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int take)
        {
            throw new NotImplementedException();
        }

        public T GetLast<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> order)
        {
            throw new NotImplementedException();
        }

        public T Add(T entity)
        {
            throw new NotImplementedException();
        }

        public List<T> Add(List<T> entities)
        {
            throw new NotImplementedException();
        }

        public T Add(T entity, Func<DataLoadOptions> howToGetLoadOptions)
        {
            throw new NotImplementedException();
        }

        public T Update(Func<T, bool> condition, T newEntity)
        {
            throw new NotImplementedException();
        }

        public T Update(Func<T, bool> condition, T newEntity, string[] includeProperties)
        {
            throw new NotImplementedException();
        }

        public T Update(Func<T, bool> condition, Dictionary<string, object> updateField)
        {
            throw new NotImplementedException();
        }

        public T Update(Func<T, bool> condition, Action<T> howToUpdateEntity)
        {
            throw new NotImplementedException();
        }

        public List<T> Update(Func<T, bool> condition, Action<IEnumerable<T>> howToUpdateEntity)
        {
            throw new NotImplementedException();
        }

        public List<T> UpdateAll(Func<T, bool> condition, Action<T> howToUpdateEntity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Func<T, bool> condition)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string sqlCmd)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Func<T, bool> condition, Func<T, int> keys, string primaryKey)
        {
            throw new NotImplementedException();
        }

        public int Count(Expression<Func<T, bool>> condition)
        {
            throw new NotImplementedException();
        }

        public int Update(Criteria criteria)
        {
            throw new NotImplementedException();
        }

        public bool Update(string cmdText)
        {
            throw new NotImplementedException();
        }

        public PaginationList<TResult> GetPager<TResult, TKey>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start,
            int takeCount)
        {
            throw new NotImplementedException();
        }

        public PaginationList<TResult> GetPager<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, string sort, string orderBy, int start,
            int takeCount)
        {
            throw new NotImplementedException();
        }

        public PaginationList<TResult> GetPager<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public PaginationList<TResult> GetPager<TResult, TKey>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, Expression<Func<T, TKey>> orderBy,
            int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public PaginationList<TResult> GetPager<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, string sort,
            string orderBy, int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public PaginationList<T> GetPager(Expression<Func<T, bool>> condition, int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public PaginationList<T> GetPager<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public PaginationList<T> GetPager(Expression<Func<T, bool>> condition, string sort, string orderBy, int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public PaginationList<T> GetPager(Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public PaginationList<T> GetPager<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start,
            int takeCount)
        {
            throw new NotImplementedException();
        }

        public PaginationList<T> GetPager(Expression<Func<T, bool>> condition, string sort, string orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start,
            int takeCount)
        {
            throw new NotImplementedException();
        }

        public ConnectionState State { get; private set; }
    }
}
