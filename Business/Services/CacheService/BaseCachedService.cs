using System;
using System.Collections;
using Infrasturcture.Cache;

namespace Services.CacheService
{
    public abstract class BaseCachedService
    {
        protected readonly ICachePolicy _cachePolicy;

        protected BaseCachedService()
        {

        }

        protected BaseCachedService(ICachePolicy cachePolicy)
        {
            _cachePolicy = cachePolicy;
        }

        protected T GetCache<T>(string key) where T : class
        {
            return _cachePolicy.Get<T>(key);
        }

        protected void SetCache<T>(string key, T items)
        {
            _cachePolicy.Set(key, items);
        }

        protected void Delete(string key)
        {
            _cachePolicy.Delete(key);
        }

        protected T GetOrAdd<T>(string key, T loadedItems) where T : class
        {
            var items = _cachePolicy.Get<T>(key);

            if (items == null || IsCollectionEmpty(items))
            {
                if (loadedItems != null)
                {
                    _cachePolicy.Set(key, loadedItems);
                }
                return loadedItems;
            }

            return items;
        }

        protected T GetOrAdd<T>(string key, Func<T> howToGet) where T : class
        {
            var items = _cachePolicy.Get<T>(key);

            if (items == null || IsCollectionEmpty(items))
            {
                var loadedItems = howToGet();
                if (loadedItems != null)
                {
                    _cachePolicy.Set(key, loadedItems);    
                }
                return loadedItems;
            }

            return items;
        }

        private bool IsCollectionEmpty<T>(T items) where T : class
        {
            var collection = items as ICollection;
            return collection != null && collection.Count == 0;
        }

        protected T GetOrAdd<T>(string key, Func<T> howToGet, DateTime dt) where T : class
        {
            var items = _cachePolicy.Get<T>(key);
            if (items == null)
            {
                var loadedItems = howToGet();
                _cachePolicy.Set(key, loadedItems, dt);
                return loadedItems;
            }

            return items;
        }
    }
}
