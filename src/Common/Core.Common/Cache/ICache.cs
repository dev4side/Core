using System;

namespace Core.Common.Cache
{
    public interface ICache
    {
        object GetFromCache(string cacheKey);
        void AddToCache(string cacheKey, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration);
        void RemoveFromCache(string cacheKey);
        object GetFromSession(string sessionKey);
        void AddToSession(string sessionKey, object value);
        void RemoveFromSession(string sessionKey);
        object GetGlobalResourceObject(string classKey, string resourceKey);
    }
}
