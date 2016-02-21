using System;
using System.Collections.Generic;
using Infrasturcture.Cache;
using Services.Contracts;

namespace Services.CacheService
{
    /// <summary>
    /// 文档缓存
    /// </summary>
    public class DocumentCacheService: BaseCachedService
    {
        private readonly DocumentListCacheService _cacheService;

        public DocumentCacheService(ICachePolicy cachePolicy, DocumentListCacheService cacheService):base(cachePolicy)
        {
            _cacheService = cacheService;
        }

        public void Add(DocumentObject document)
        {
            _cacheService.Add(document);
            SetCache(GenerateKey(document.Id.ToString()), document);
        }

        public void Add(List<DocumentObject> documents)
        {
            foreach (var document in documents)
            {
                _cacheService.Add(document);
                SetCache(GenerateKey(document.Id.ToString()), document);
            }
        }

        public void Update(DocumentObject document)
        {
            _cacheService.Update(document);
            SetCache(GenerateKey(document.Id.ToString()), document);
        }

        public void Update(List<DocumentObject> documents)
        {
            _cacheService.Update(documents);
            foreach(var document in documents)
            {
                SetCache(GenerateKey(document.Id.ToString()), document);
            }
        }

        public new void Delete(string id)
        {
            _cacheService.Delete(id);
            _cachePolicy.Delete(GenerateKey(id));
        }

        public void Delete(string[] ids)
        {
            foreach (var id in ids)
            {
                Delete(id);
            }
        }

        

        public List<DocumentObject> GetOrAdd(Func<List<DocumentObject>> howToGet)
        {
            return _cacheService.GetOrAdd(howToGet);
        }

        public DocumentObject GetOrAdd(string id, DocumentObject document)
        {
            return base.GetOrAdd(GenerateKey(id), document);
        }

        public DocumentObject GetOrAdd(string id, Func<DocumentObject> howToGet)
        {
            return base.GetOrAdd(GenerateKey(id), howToGet);
        }

        private string GenerateKey(string id)
        {
            return "DOCUMENT_CACHE_" + id;
        }
    }
}
