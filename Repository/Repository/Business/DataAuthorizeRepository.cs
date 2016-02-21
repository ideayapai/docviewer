using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using Infrasturcture.QueryableExtension;
using Repository.Base;

namespace Repository.Business
{
    public class DataAuthorizeRepository<T, U> : IDataAuthorizeRepository<T>
        where T : EntityBaseByUser, new()
        where U : DocViewerRepositoryContext, new()
    {

        public T Get(Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {

                return db.GetTable<T>().FirstOrDefault(condition);
            }
        }

        public List<TResult> Get<TResult>(string createdBy, Expression<Func<T, TResult>> selector)
        {
            using (var db = new U())
            {
                return
                    db.GetTable<T>()
                        .Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY))
                        .Select(selector)
                        .ToList();
            }
        }

        public virtual List<TResult> GetByUser<TResult>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {
                return
                    db.GetTable<T>()
                        .Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY))
                        .Where(condition).Select(selector).ToList();
            }
        }

        public virtual List<TResult> GetByUser<TResult>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions)
        {
            using (var db = new U())
            {
                db.LoadOptions = loadOptions();
                return
                    db.GetTable<T>()
                        .Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY))
                        .Where(condition).Select(selector).ToList();
            }
        }

        public virtual List<TResult> GetByUser<TResult, TKey>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start,
            int takeCount)
        {
            using (var db = new U())
            {
                return
                    db.GetTable<T>()
                        .Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY))
                        .Where(condition)
                        .OrderByDescending(orderBy)
                        .Skip(start)
                        .Take(takeCount)
                        .Select(selector)
                        .ToList();
            }
        }

        public virtual List<TResult> GetByUser<TResult, TKey>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions,
            Expression<Func<T, TKey>> orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = loadOptions();
                return
                    db.GetTable<T>()
                        .Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY))
                        .Where(condition)
                        .OrderByDescending(orderBy)
                        .Skip(start)
                        .Take(takeCount)
                        .Select(selector)
                        .ToList();
            }
        }

        public virtual List<T> GetUser(string createdBy, Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {
                return
                    db.GetTable<T>()
                        .Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY))
                        .Where(condition)
                        .ToList();
            }
        }

        public virtual List<T> GetUser(string createdBy, Expression<Func<T, bool>> condition, int start, int take)
        {
            using (var db = new U())
            {
                return
                    db.GetTable<T>()
                        .Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY))
                        .Where(condition)
                        .Skip(start)
                        .Take(take)
                        .ToList();
            }
        }

        public virtual List<T> GetByUser<TKey>(string createdBy, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int take)
        {
            using (var db = new U())
            {
                var users = db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME);
                return
                    db.GetTable<T>()
                        .Where(p => users.Contains(p.CREATEDBY))
                        .Where(condition)
                        .OrderByDescending(orderBy)
                        .Skip(start)
                        .Take(take)
                        .ToList();
            }
        }

        public virtual List<T> GetByUser<TKey>(string createdBy, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy)
        {
            using (var db = new U())
            {
                var users = db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME);
                return
                    db.GetTable<T>()
                        .Where(p => users.Contains(p.CREATEDBY))
                        .Where(condition)
                        .OrderByDescending(orderBy)
                        .ToList();
            }
        }

        public virtual List<T> GetByUser(string createdBy, Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                return
                    db.GetTable<T>()
                        .Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY))
                        .Where(condition)
                        .ToList();
            }
        }

        public virtual List<T> GetByUser(string createdBy, Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int take)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                return
                    db.GetTable<T>()
                        .Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY))
                        .Where(condition)
                        .Skip(start)
                        .Take(take)
                        .ToList();
            }
        }

        public virtual List<T> GetByUser<TKey>(string createdBy, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start,
            int take)
        {

            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                return
                    db.GetTable<T>()
                        .Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY))
                        .Where(condition)
                        .OrderByDescending(orderBy)
                        .Skip(start)
                        .Take(take)
                        .ToList();
            }
        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult, TKey>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start,
            int takeCount){
                using (var db = new U())
            {
                var query = db.GetTable<T>().Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY)).Where(condition);
                var items = query.OrderBy(orderBy).Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }

        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, string sort, string orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY)).Where(condition);
                var items = query.BuildOrderClause(sort, orderBy).Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPagerByUser<TKey>(string createdBy, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                var query = db.GetTable<T>().Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY)).Where(condition);
                var items = query.OrderBy(orderBy).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPagerByUser<TKey>(string createdBy, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY)).Where(condition);
                var items = query.OrderBy(orderBy).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult, TKey>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, Expression<Func<T, TKey>> orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY)).Where(condition);
                var items = query.OrderBy(orderBy).Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, string sort, string orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = loadOptions();
                var query = db.GetTable<T>().Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY)).Where(condition);
                var items = query.BuildOrderClause(sort, orderBy).Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult>(string createdBy, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY)).Where(condition);
                var items = query.Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPagerByUser(string createdBy, Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                var query = db.GetTable<T>().Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY)).Where(condition);
                var items = query.Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPagerByUser(string createdBy, Expression<Func<T, bool>> condition, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY)).Where(condition);
                var items = query.Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPagerByUser(string createdBy, Expression<Func<T, bool>> condition, string sort, string orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                var query = db.GetTable<T>().Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY)).Where(condition);
                var items = query.BuildOrderClause(sort, orderBy).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPagerByUser(string createdBy, Expression<Func<T, bool>> condition, string sort, string orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY)).Where(condition);
                var items = query.BuildOrderClause(sort, orderBy).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual int CountByUser(string createdBy, Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {
                return db.GetTable<T>().Where(p => db.GetUsersByUserName(createdBy).Select(i => i.USER_INFO_NAME).Contains(p.CREATEDBY)).Count(condition);
            }
        }
    }
}
