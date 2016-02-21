using System.Collections.Generic;

namespace Services.CacheService
{
    public static class MemoryContainer
    {
        private static Dictionary<string, object> _container = new Dictionary<string, object>();

        public static void Push(string key, object obj)
        {
            if (_container.ContainsKey(key))
            {
                _container[key] = obj;
            }
            else
            {
                _container.Add(key, obj);
            }
        }

        public static object Pop(string key)
        {
            if (_container.ContainsKey(key))
            {
                var obj = _container[key];
                _container.Remove(key);
                return obj;
            }
            return null;
        }
    }
}