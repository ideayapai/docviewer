using System;
using BeIT.MemCached;
using Common.Logging;

namespace Infrasturcture.Cache
{
    public class MemcachedCachePolicy : ICachePolicy
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        private readonly MemcachedClient _cache;

        public MemcachedCachePolicy()
        {
            _cache = MemcachedClient.GetInstance("MemcachedConfig");
            
            _cache.MaxPoolSize = 10000;
        }

        public void Set<T>(string key, T value)
        {
            _cache.Set(key, value);
        }

        public void Set<T>(string key, T value, DateTime dt)
        {
            _cache.Set(key, value, dt);
        }

        public T Get<T>(string key)
        {
            try
            {
                return (T)_cache.Get(key);
            }
            catch (Exception e)
            {
                _logger.Debug("Get _cache for key failed, key[" + key + "]", e);
                return default(T);
            }
        }

        public void Set(string key, object value)
        {
            _cache.Set(key, value);
        }

        public void Set(string key, object value, DateTime dt)
        {
            _cache.Set(key, value, dt);
        }

        public object Get(string key)
        {
            return _cache.Get(key);
        }

        public void Delete(string key)
        {
            _cache.Delete(key);
        }

        public void FlushAll()
        {
            _cache.FlushAll();
        }
    }
}
