using System;

namespace Infrasturcture.Cache
{
    public interface ICachePolicy
    {
        void Set<T>(string key, T value);

        void Set<T>(string key, T value, DateTime dt);

        T Get<T>(string key);

        void Set(string key, object value);

        void Set(string key, object value, DateTime dt);

        object Get(string key);

        void Delete(string key);

        void FlushAll();

    }
}
