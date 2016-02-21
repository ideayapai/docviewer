using System;
using System.Web;
using System.Web.Caching;

namespace Infrasturcture.Web
{
    public class DefaultSessionProvider : ISessionProvider
    {
        public object Get(string key)
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                return null;
            }

            return HttpContext.Current.Session[key];
        }

        public void Set(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

        public void Abandon()
        {
            HttpContext.Current.Session.Abandon();
        }

        public void RemoveAll()
        {
            HttpContext.Current.Session.RemoveAll();
        }

        public string SessionId()
        {

            return HttpContext.Current.Session.SessionID;
        }

        public T Cache<T>(string key, string databaseEntry, string tableName, Func<T> howToGet) where T : class
        {
            if (HttpContext.Current == null || HttpContext.Current.Cache == null)
            {
                return null;
            }
            var cache = HttpContext.Current.Cache;
            if (cache[key] == null)
            {
                cache.Add(key, howToGet(), new SqlCacheDependency(databaseEntry, tableName),
                          System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration,
                          CacheItemPriority.Normal, null);
            }
            return (T)cache[key];
        }

        public T Cache<T>(string key, DateTime absoluteExpireDate, Func<T> howToGet) where T : class
        {
            if (HttpContext.Current == null || HttpContext.Current.Cache == null)
            {
                return null;
            }
            var cache = HttpContext.Current.Cache;
            if (cache[key] == null)
            {
                cache.Add(key, howToGet.Invoke(), null, absoluteExpireDate, TimeSpan.Zero, CacheItemPriority.Normal, null);
            }
            return (T)cache[key];
        }
    }
}
