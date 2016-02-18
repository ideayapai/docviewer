using System;
using System.Collections.Generic;
using Infrasturcture.Cache;
using Infrasturcture.DomainObjects;
using Services.Contracts;

namespace Services.CacheService
{
    public class SpaceListCacheService: BaseCachedService
    {
        public SpaceListCacheService(ICachePolicy cachePolicy)
            : base(cachePolicy)
        {
        }

        public void Add(SpaceObject space)
        {
            List<SpaceObject> cache = GetCache<List<SpaceObject>>(GenerateKey());
            if (cache != null && cache.Count > 0)
            {
                var result = cache.Find(f => f != null && f.Id.ToString() == space.Id.ToString());
                if (result == null)
                {
                    cache.Add(space);
                    _cachePolicy.Set(GenerateKey(), cache);
                }
            }
        }

        public void UpdateAll(List<SpaceObject> spaces)
        {
            List<SpaceObject> cache = GetCache<List<SpaceObject>>(GenerateKey());
            if (cache != null && cache.Count > 0)
            {
                for (int i = 0; i < cache.Count; ++i)
                {
                    var space = spaces.Find(f => f.Id.ToString() == cache[i].Id.ToString());
                    if (space != null)
                    {
                        cache[i] = space.ToObject<SpaceObject>();
                    }
                 
                }
                _cachePolicy.Set(GenerateKey(), cache);
            }
           
        }

        public void Update(SpaceObject space)
        {
            List<SpaceObject> cache = GetCache<List<SpaceObject>>(GenerateKey());
            if (cache != null && cache.Count > 0)
            {
                for (int i = 0; i < cache.Count; ++i)
                {
                    if (cache[i] != null && cache[i].Id.ToString() == space.Id.ToString())
                    {
                        cache[i] = space.ToObject<SpaceObject>();
                        _cachePolicy.Set(GenerateKey(), cache);
                        break;
                    }
                }
            }
        }

        public new void Delete(string id)
        {
            var cache = GetCache<List<SpaceObject>>(GenerateKey());
            if (cache != null && cache.Count > 0)
            {
                var result = cache.Find(f => f!= null && f.Id.ToString() == id);
                if (result != null)
                {
                    cache.Remove(result);
                    _cachePolicy.Set(GenerateKey(), cache);
                } 
            }     
        }

        public List<SpaceObject> GetOrAdd(Func<List<SpaceObject>> howToGet)
        {
            return GetOrAdd(GenerateKey(), howToGet);
        }

        private string GenerateKey()
        {
            return "ALL_SPACE_CACHE";
        }
    }
}
