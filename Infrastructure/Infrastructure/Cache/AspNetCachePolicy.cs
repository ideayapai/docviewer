using System;
using System.Web;
using Common.Logging;

namespace Infrasturcture.Cache
{
    public class AspNetCachePolicy: ICachePolicy
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public void Set<T>(string key, T value)
        {
        
            HttpContext.Current.Cache.Insert(key, value);
        }

        

        public void Set<T>(string key, T value, DateTime dt)
        {
            HttpContext.Current.Cache.Insert(key, value);
        }

        public T Get<T>(string key)
        {
            return (T)HttpContext.Current.Cache.Get(key);
        }

        public void Set(string key, object value)
        {
            HttpContext.Current.Cache.Insert(key, value);
        }

        public void Set(string key, object value, DateTime dt)
        {
            HttpContext.Current.Cache.Insert(key, value);
        }

        public object Get(string key)
        {
            return HttpContext.Current.Cache.Get(key);
        }

        public void Delete(string key)
        {
            HttpContext.Current.Cache.Remove(key);
        }

        public void FlushAll()
        {
            throw new NotImplementedException("AspNetCache FlushAll is not implemented.");
        }
    }
}
