using System;
using System.Collections.Generic;
using Infrasturcture.Cache;
using Infrasturcture.DomainObjects;
using Services.Contracts;

namespace Services.CacheService
{
    public class DocumentListCacheService: BaseCachedService
    {
        public DocumentListCacheService(ICachePolicy cachePolicy)
            : base(cachePolicy)
        {
        }

        public void Add(DocumentObject document)
        {
            List<DocumentObject> documents = GetCache<List<DocumentObject>>(GenerateKey());
            if (documents != null && documents.Count > 0)
            {
                var result = documents.Find(f => f.Id.ToString() == document.Id.ToString());
                if (result == null)
                {
                    documents.Add(document);
                    _cachePolicy.Set(GenerateKey(), documents);
                }
            }
        }

        public void Update(List<DocumentObject> documents)
        {
            List<DocumentObject> cache = GetCache<List<DocumentObject>>(GenerateKey());
            if (cache != null && cache.Count > 0)
            {
                for (int i = 0; i < cache.Count; ++i)
                {
                    var document = documents.Find(f => f.Id.ToString() == cache[i].Id.ToString());
                    if (document != null)
                    {
                        cache[i] = document.ToObject<DocumentObject>();
                    }
                }
                _cachePolicy.Set(GenerateKey(), cache);
            }
        }
        public void Update(DocumentObject document)
        {
            List<DocumentObject> documents = GetCache<List<DocumentObject>>(GenerateKey());
            if (documents != null)
            {
                for(int i = 0; i < documents.Count; ++i)
                {
                    if (documents[i].Id.ToString() == document.Id.ToString())
                    {
                        documents[i] = document.ToObject<DocumentObject>();
                        _cachePolicy.Set(GenerateKey(), documents);
                        break;
                    }
                }
            }
        }

        public new void Delete(string id)
        {
            var documents = GetCache<List<DocumentObject>>(GenerateKey());
            if (documents != null && documents.Count > 0)
            {
                var result = documents.Find(f => f.Id.ToString() == id);
                if (result != null)
                {
                    documents.Remove(result);
                    _cachePolicy.Set(GenerateKey(), documents);
                }     
            }
                   
        }

        public List<DocumentObject> GetOrAdd(Func<List<DocumentObject>> howToGet)
        {
            return GetOrAdd(GenerateKey(), howToGet);
        }

        private string GenerateKey()
        {
            return "ALL_DOCUMENT_CACHE";
        }
    }
}
