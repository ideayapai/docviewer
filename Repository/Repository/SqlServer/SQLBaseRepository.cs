using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using Common.Logging;
using Infrasturcture.QueryableExtension;

namespace Repository.SqlServer
{
    public class SQLBaseRepository<T, U> : IBaseRepository<T>  where U:RepositoryContext, new() where T : class
    {
        private ILog _logger = LogManager.GetCurrentClassLogger();
        
        #region Get method

        public virtual T Get(Func<T, bool> condition)
        {
            using (var db = new U())
            {
              
                return db.GetTable<T>().FirstOrDefault(condition);
            }
        }

        public T Get(Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {

                return db.GetTable<T>().FirstOrDefault(condition);
            }
        }

        public T GetUseExpression(Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {

                return db.GetTable<T>().FirstOrDefault(condition);
            }
        }

        public virtual T Get(Func<T, bool> condition, Func<DataLoadOptions> howToGetLoadOptions)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                return db.GetTable<T>().FirstOrDefault(condition);
            }
        }

        public T GetToSql(DateTime dateTime, string category)
        {
            string tableName = typeof(T).Name;
            string sqlCmd = "select * from " + tableName + " where datetime='" + dateTime + "' and category='" + category + "'";
            using (var db = new U())
            {
                return db.ExecuteQuery<T>(sqlCmd).First();
            }
        }

        public string GetTableName()
        {
            return typeof (T).Name;
        }

        public virtual List<TResult> Get<TResult>(Func<T, TResult> selector)
        {
            using (var db = new U())
            {
                return db.GetTable<T>().Select(selector).ToList();
            }
        }

        public virtual List<TResult> Get<TResult>(Func<T, TResult> selector, Func<T, bool> condition)
        {
            using (var db = new U())
            {
                return db.GetTable<T>().Where(condition).Select(selector).ToList();
            }
        }

        public List<TResult> Get<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions)
        {
            using (var db = new U())
            {
                db.LoadOptions = loadOptions();
                return db.GetTable<T>().Where(condition).Select(selector).ToList();
            }
        }

        public List<TResult> GetUseExpression<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {
                return db.GetTable<T>().Where(condition).Select(selector).ToList();
            }
        }

        public virtual List<T> GetAll(Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {
                var a = db.GetTable<T>();
                var b = a.Where(condition);
                var c = b.ToList();
                return c;
            }
        }


        public virtual List<T> GetAll(Func<T, bool> condition)
        {
            using (var db = new U())
            {
                return db.GetTable<T>().Where(condition).ToList();
            }
        }

        public List<T> GetAllUseExpression(Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {
                return db.GetTable<T>().Where(condition).ToList();
            }
        }

        public List<T> GetAllToSql(string sqlCmd, Func<DataLoadOptions> loadOptions)
        {
            using (var db = new U())
            {
                db.LoadOptions = loadOptions();
                return db.ExecuteQuery<T>(sqlCmd).ToList();
            }
        }

        public virtual List<T> GetAllByExp(Expression<Func<T, bool>> expWhere)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(expWhere);
                return query.ToList();
            }
        }


        public List<T> GetAllToSql(string sqlCmd, params object[] parameters)
        {
            return Select(sqlCmd, parameters);
        }

        public List<T> GetAllToSql(string sqlCmd)
        {
            using (var db = new U())
            {
                return db.ExecuteQuery<T>(sqlCmd).ToList();
            }
        }

        public virtual List<T> GetAll(Expression<Func<T, bool>> condition, int start, int take)
        {
            using (var db = new U())
            {
                return db.GetTable<T>().Where(condition).Skip(start).Take(take).ToList();
            }
        }

        public List<T> GetAll<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int take)
        {
            using (var db = new U())
            {
                return db.GetTable<T>().Where(condition).OrderByDescending(orderBy).Skip(start).Take(take).ToList();
            }
        }

        public List<T> GetAll<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy)
        {
            using (var db = new U())
            {
                return db.GetTable<T>().Where(condition).OrderByDescending(orderBy).ToList();
            }
        }

        public virtual List<T> GetAll(Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                return db.GetTable<T>().Where(condition).ToList();
            }
        }

        public virtual List<T> GetAll(Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int take)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                return db.GetTable<T>().Where(condition).Skip(start).Take(take).ToList();
            }
        }

        public List<T> GetAll<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int take)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                return db.GetTable<T>().Where(condition).OrderByDescending(orderBy).Skip(start).Take(take).ToList();
            }
        }

        public T GetLast<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> order)
        {
            using (var db = new U())
            {
                return db.GetTable<T>().Where(condition).OrderByDescending(order).Take(1).FirstOrDefault();
            }
        }

        #endregion

        #region Add method

        public virtual T Add(T entity)
        {
            using (var db = new U())
            {
                db.GetTable<T>().InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity;
            }
        }

        public virtual List<T> Add(List<T> entities)
        {
            using (var db = new U())
            {
                foreach (var entity in entities)
                {
                    try
                    {
                        db.GetTable<T>().InsertOnSubmit(entity);
                        db.SubmitChanges();
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e.Message);
                    }
                }
                return entities;
            }
        }

        public virtual T Add(T entity, Func<DataLoadOptions> howToGetLoadOptions)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                db.GetTable<T>().InsertOnSubmit(entity);
                db.SubmitChanges();
                return entity;
            }
        }

        #endregion

        #region Update method

        public virtual T Update(Func<T, bool> condition, T newEntity)
        {
            using (var db = new U())
            {
                var tEntity = db.GetTable<T>().First(condition);
                db.UpdateEntity(tEntity, newEntity);
                db.SubmitChanges();
                return tEntity;
            }
        }

        public virtual T Update(Func<T, bool> condition, T newEntity, string[] includeProperties)
        {
            using (var db = new U())
            {
                var tEntity = db.GetTable<T>().First(condition);
                db.UpdateEntity(tEntity, newEntity, includeProperties);
                db.SubmitChanges();
                return tEntity;
            }
        }

        public virtual T Update(Func<T, bool> condition, Dictionary<string, object> updateField)
        {
            using (var db = new U())
            {
                var tEntity = db.GetTable<T>().First(condition);
                db.UpdateEntity(tEntity, updateField);
                db.SubmitChanges();
                return tEntity;
            }
        }

        public virtual List<T> Update(Func<T, bool> condition, Action<IEnumerable<T>> howToUpdateEntity)
        {
            using (var db = new U())
            {
                var tEntity = db.GetTable<T>().Where(condition);
                howToUpdateEntity(tEntity);
                db.SubmitChanges();
                return tEntity.ToList();
            }
        }

        public virtual T Update(Func<T, bool> condition, Action<T> howToUpdateEntity)
        {
            using (var db = new U())
            {
                var tEntity = db.GetTable<T>().FirstOrDefault(condition);
                if (tEntity != null)
                {
                    howToUpdateEntity(tEntity);
                    db.SubmitChanges();
                }
                return tEntity;
            }
        }


        public virtual List<T> UpdateAll(Func<T, bool> condition, Action<T> howToUpdateEntity)
        {
            using (var db = new U())
            {
                var tEntity = db.GetTable<T>().Where(condition);
                foreach (var entity in tEntity)
                {
                    howToUpdateEntity(entity);
                }
                db.SubmitChanges();
                return tEntity.ToList();
            }
        }

        #endregion

        #region Delete method

        public virtual bool Delete(Func<T, bool> condition)
        {
            using (var db = new U())
            {
                
                var entity = db.GetTable<T>().Where(condition);
                if (entity != null)
                {
                    db.GetTable<T>().DeleteAllOnSubmit(entity);
                    db.SubmitChanges();
                    return true;
                }
                return false;
                
            }
        }

        public virtual bool Delete(Func<T, bool> condition, Func<T, int> keys, string primaryKey)
        {
            if (String.IsNullOrEmpty(primaryKey))
                return false;
            using (var db = new U())
            {
                var ids = db.GetTable<T>().Where(condition).Select(keys).ToList();
                if (ids.Count != 0)
                {
                    var tbName = typeof(T).Name;
                    var keysStr = string.Empty;
                    ids.ForEach(p => keysStr += p + ",");
                    var commend = "delete from " + tbName + " where " + primaryKey + " in(" + keysStr.TrimEnd(',') + ")";
                    db.ExecuteCommand(commend);
                    return true;
                }
                return false;
            }
        }

        public bool Delete(string sqlCmd)
        {
            using (var db = new U())
            {
                if (db.ExecuteCommand(sqlCmd) > 0)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        public virtual int Count(Func<T, bool> condition)
        {
            using (var db = new U())
            {
                return db.GetTable<T>().Count(condition);
            }
        }

        public int Update(Criteria criteria)
        {
            using (var db = new U())
            {
                return db.ExecuteCommand(criteria.GetUpdateString());
            }
        }

        public bool Update(string cmdText)
        {
            using (var db = new U())
            {
                if (db.ExecuteCommand(cmdText) > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public ConnectionState State
        {
            get
            {
                using (var db = new U())
                {
                    return db.Connection.State;
                }
            }
        }

        public List<T> Select(string sqlCmd, params object[] parameters)
        {
            using (var db = new U())
            {
                return db.ExecuteQuery<T>(sqlCmd, parameters).ToList();
            }
        }

        public string ConnectionString
        {
            get;
            set;
        }

        public List<TResult> Get<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {
                return db.GetTable<T>().Where(condition).Select(selector).ToList();
            }
        }

        public virtual int Count(Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {
                return db.GetTable<T>().Count(condition);
            }
        }

        public virtual PaginationList<TResult> GetPager<TResult, TKey>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start,
            int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(condition);
                var items = query.OrderBy(orderBy).Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }


        public virtual PaginationList<TResult> GetPager<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, string sort, string orderBy, int start,
            int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(condition);
                var items = query.BuildOrderClause(sort, orderBy).Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }



        public virtual PaginationList<TResult> GetPager<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(condition);
                var items = query.Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<TResult> GetPager<TResult, TKey>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, Expression<Func<T, TKey>> orderBy,
            int start, int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = loadOptions();
                var query = db.GetTable<T>().Where(condition);
                var items = query.Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<TResult> GetPager<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions,
            string sort, string orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = loadOptions();
                var query = db.GetTable<T>().Where(condition);
                var items = query.BuildOrderClause(sort, orderBy).Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPager(Expression<Func<T, bool>> condition, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(condition);
                var items = query.Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPager<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(condition);
                var items = query.OrderBy(orderBy).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPager(Expression<Func<T, bool>> condition, string sort, string orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(condition);
                var items = query.BuildOrderClause(sort, orderBy).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }


        public virtual PaginationList<T> GetPager(Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                var query = db.GetTable<T>().Where(condition);
                var items = query.Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPager<TKey>(Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                var query = db.GetTable<T>().Where(condition);
                var items = query.OrderBy(orderBy).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPager(Expression<Func<T, bool>> condition, string sort, string orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start,
            int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                var query = db.GetTable<T>().Where(condition);
                var items = query.BuildOrderClause(sort, orderBy).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult, TKey>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition,
            Expression<Func<T, TKey>> orderBy, int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, string sort,
            string orderBy, int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, int start,
            int takeCount)
        {
            throw new NotImplementedException();
        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult, TKey>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition,
            Func<DataLoadOptions> loadOptions, Expression<Func<T, TKey>> orderBy, int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions,
            string sort, string orderBy, int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public virtual PaginationList<T> GetPagerByUser(string userName, Expression<Func<T, bool>> condition, int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public virtual PaginationList<T> GetPagerByUser<TKey>(string userName, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public virtual PaginationList<T> GetPagerByUser(string userName, Expression<Func<T, bool>> condition, string sort, string orderBy, int start,
            int takeCount)
        {
            throw new NotImplementedException();
        }

        public virtual PaginationList<T> GetPagerByUser(string userName, Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public virtual PaginationList<T> GetPagerByUser<TKey>(string userName, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions,
            int start, int takeCount)
        {
            throw new NotImplementedException();
        }

        public virtual PaginationList<T> GetPagerByUser(string userName, Expression<Func<T, bool>> condition, string sort, string orderBy,
            Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount)
        {
            throw new NotImplementedException();
        }
    }
}
