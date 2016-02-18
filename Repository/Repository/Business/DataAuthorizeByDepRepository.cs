using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using Infrasturcture.QueryableExtension;
using Repository.Base;

namespace Repository.Business
{
    public class DataAuthorizeByDepRepository<T, U> : IDataAuthorizeByDepRepository<T>
        where T : EntityBaseByDep, new()
        where U : DocViewerRepositoryContext, new()
    {
        public List<TResult> Get<TResult>(string userName, Expression<Func<T, TResult>> selector)
        {
            using (var db = new U())
            {
                return
                    db.GetTable<T>()
                        .Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID))
                        .Select(selector)
                        .ToList();
            }
        }

        public virtual List<TResult> GetByUser<TResult>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {
                return
                    db.GetTable<T>()
                        .Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID))
                        .Where(condition).Select(selector).ToList();
            }
        }

        public virtual List<TResult> GetByUser<TResult>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions)
        {
            using (var db = new U())
            {
                db.LoadOptions = loadOptions();
                return
                    db.GetTable<T>()
                        .Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID))
                        .Where(condition).Select(selector).ToList();
            }
        }

        public virtual List<TResult> GetByUser<TResult, TKey>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start,
            int takeCount)
        {
            using (var db = new U())
            {
                return
                    db.GetTable<T>()
                        .Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID))
                        .Where(condition)
                        .OrderByDescending(orderBy)
                        .Skip(start)
                        .Take(takeCount)
                        .Select(selector)
                        .ToList();
            }
        }

        public virtual List<TResult> GetByUser<TResult, TKey>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions,
            Expression<Func<T, TKey>> orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = loadOptions();
                return
                    db.GetTable<T>()
                        .Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID))
                        .Where(condition)
                        .OrderByDescending(orderBy)
                        .Skip(start)
                        .Take(takeCount)
                        .Select(selector)
                        .ToList();
            }
        }

        public virtual List<T> GetUser(string userName, Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {
                return
                    db.GetTable<T>()
                        .Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID))
                        .Where(condition)
                        .ToList();
            }
        }

        public virtual List<T> GetUser(string userName, Expression<Func<T, bool>> condition, int start, int take)
        {
            using (var db = new U())
            {
                return
                    db.GetTable<T>()
                        .Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID))
                        .Where(condition)
                        .Skip(start)
                        .Take(take)
                        .ToList();
            }
        }

        public virtual List<T> GetByUser<TKey>(string userName, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int take)
        {
            using (var db = new U())
            {
                var users = db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID);
                return
                    db.GetTable<T>()
                        .Where(p => users.Contains(p.DEPID))
                        .Where(condition)
                        .OrderByDescending(orderBy)
                        .Skip(start)
                        .Take(take)
                        .ToList();
            }
        }

        public virtual List<T> GetByUser<TKey>(string userName, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy)
        {
            using (var db = new U())
            {
                var users = db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID);
                return
                    db.GetTable<T>()
                        .Where(p => users.Contains(p.DEPID))
                        .Where(condition)
                        .OrderByDescending(orderBy)
                        .ToList();
            }
        }

        public virtual List<T> GetByUser(string userName, Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                return
                    db.GetTable<T>()
                        .Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID))
                        .Where(condition)
                        .ToList();
            }
        }

        public virtual List<T> GetByUser(string userName, Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int take)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                return
                    db.GetTable<T>()
                        .Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID))
                        .Where(condition)
                        .Skip(start)
                        .Take(take)
                        .ToList();
            }
        }

        public virtual List<T> GetByUser<TKey>(string userName, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start,
            int take)
        {

            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                return
                    db.GetTable<T>()
                        .Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID))
                        .Where(condition)
                        .OrderByDescending(orderBy)
                        .Skip(start)
                        .Take(take)
                        .ToList();
            }
        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult, TKey>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start,
            int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID)).Where(condition);
                var items = query.OrderBy(orderBy).Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }

        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, string sort, string orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID)).Where(condition);
                var items = query.BuildOrderClause(sort, orderBy).Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPagerByUser<TKey>(string userName, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                var query = db.GetTable<T>().Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID)).Where(condition);
                var items = query.OrderBy(orderBy).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPagerByUser<TKey>(string userName, Expression<Func<T, bool>> condition, Expression<Func<T, TKey>> orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID)).Where(condition);
                var items = query.OrderBy(orderBy).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult, TKey>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, Expression<Func<T, TKey>> orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID)).Where(condition);
                var items = query.OrderBy(orderBy).Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, Func<DataLoadOptions> loadOptions, string sort, string orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = loadOptions();
                var query = db.GetTable<T>().Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID)).Where(condition);
                var items = query.BuildOrderClause(sort, orderBy).Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<TResult> GetPagerByUser<TResult>(string userName, Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> condition, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID)).Where(condition);
                var items = query.Select(selector).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<TResult>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPagerByUser(string userName, Expression<Func<T, bool>> condition, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                var query = db.GetTable<T>().Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID)).Where(condition);
                var items = query.Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPagerByUser(string userName, Expression<Func<T, bool>> condition, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID)).Where(condition);
                var items = query.Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPagerByUser(string userName, Expression<Func<T, bool>> condition, string sort, string orderBy, Func<DataLoadOptions> howToGetLoadOptions, int start, int takeCount)
        {
            using (var db = new U())
            {
                db.LoadOptions = howToGetLoadOptions();
                var query = db.GetTable<T>().Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID)).Where(condition);
                var items = query.BuildOrderClause(sort, orderBy).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual PaginationList<T> GetPagerByUser(string userName, Expression<Func<T, bool>> condition, string sort, string orderBy, int start, int takeCount)
        {
            using (var db = new U())
            {
                var query = db.GetTable<T>().Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID)).Where(condition);
                var items = query.BuildOrderClause(sort, orderBy).Skip(start).Take(takeCount).ToList();
                var rs = new PaginationList<T>(items) { TotalCount = query.Count(), PageSize = takeCount };
                return rs;
            }
        }

        public virtual int CountByUser(string userName, Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {
                return db.GetTable<T>().Where(p => db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID).Contains(p.DEPID)).Count(condition);
            }
        }

        public virtual List<T> GetByUser(string userName, Expression<Func<T, bool>> condition)
        {
            using (var db = new U())
            {
                var users = db.GetDepsByUserName(userName).Select(i => i.DEPT_INFO_ID);
                return
                    db.GetTable<T>()
                        .Where(p => users.Contains(p.DEPID))
                        .Where(condition)
                        .ToList();
            }
        }
    }
}
