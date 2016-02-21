using System;
using System.Collections.Generic;
using Infrasturcture.Cache;
using Services.Contracts;

namespace Services.CacheService
{
    public class SpaceCacheService: BaseCachedService
    {
        private readonly SpaceListCacheService _cacheService;

        public SpaceCacheService(ICachePolicy cachePolicy, SpaceListCacheService cacheService)
            : base(cachePolicy)
        {
            _cacheService = cacheService;
        }

        public void Add(SpaceObject space)
        {
            _cacheService.Add(space);
            SetCache(GenerateKey(space.Id.ToString()), space);
        }

        public void Update(SpaceObject space)
        {
            _cacheService.Update(space);
            SetCache(GenerateKey(space.Id.ToString()), space);
        }

        public void UpdateAll(List<SpaceObject> spaces)
        {
            _cacheService.UpdateAll(spaces);
            foreach(var space in spaces)
            {
                SetCache(GenerateKey(space.Id.ToString()), space);
            }
        }

        public new void Delete(string id)
        {
            _cacheService.Delete(id);
            _cachePolicy.Delete(GenerateKey(id));
        }

        public List<SpaceObject> GetOrAdd(Func<List<SpaceObject>> howToGet)
        {
            return _cacheService.GetOrAdd(howToGet);
        }

        public SpaceObject GetOrAdd(string id, SpaceObject document)
        {
            return base.GetOrAdd(GenerateKey(id), document);
        }

        public SpaceObject GetOrAdd(string id, Func<SpaceObject> howToGet)
        {
            return base.GetOrAdd(GenerateKey(id), howToGet);
        }

        private string GenerateKey(string id)
        {
            return "SPACE_CACHE_" + id;
        }
    }
}
