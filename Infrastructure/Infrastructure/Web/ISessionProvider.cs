using System;

namespace Infrasturcture.Web
{
    public interface ISessionProvider
    {
        object Get(string key);
        void Set(string key, object value);
        void Abandon();
        void RemoveAll();
        string SessionId();
        T Cache<T>(string key, string databaseEntry, string tableName, Func<T> howToGet) where T : class;
        T Cache<T>(string key, DateTime absoluteExpireDate, Func<T> howToGet) where T : class;
    }

    
}
