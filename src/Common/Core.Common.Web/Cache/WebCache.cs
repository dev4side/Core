using System;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using Core.Common.Cache;
using Core.Common.Cache.Exceptions;

namespace Core.Common.Web.Cache
{
    public class WebCache : ICache
    {
        public object GetFromCache(string cacheKey)
        {
            System.Web.Caching.Cache currentCache = GetCurrentHttpContest().Cache;
            return currentCache == null ? null : currentCache[cacheKey];
        }

        public void AddToCache(string cacheKey, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache currentCache = GetCurrentHttpContest().Cache;
            if (currentCache == null) return;
            currentCache.Add(cacheKey, value, null, absoluteExpiration, slidingExpiration, CacheItemPriority.Default, null);
        }

        public void RemoveFromCache(string cacheKey)
        {
            System.Web.Caching.Cache currentCache = GetCurrentHttpContest().Cache;
            if (currentCache == null) return;
            currentCache.Remove(cacheKey);
        }

        public object GetFromSession(string sessionKey)
        {
            HttpSessionState currentSession = GetCurrentHttpContest().Session;
            return currentSession == null ? null : currentSession[sessionKey];
        }

        public void AddToSession(string sessionKey, object value)
        {
            HttpSessionState currentSession = GetCurrentHttpContest().Session;
            if (currentSession == null) return;
            currentSession.Add(sessionKey, value);
        }

        public void RemoveFromSession(string sessionKey)
        {
            HttpSessionState currentSession = GetCurrentHttpContest().Session;
            if (currentSession == null) return;
            currentSession.Remove(sessionKey);
        }

        public object GetGlobalResourceObject(string classKey, string resourceKey)
        {
            return HttpContext.GetGlobalResourceObject(classKey, resourceKey);
        }

        private HttpContext GetCurrentHttpContest()
        {
            HttpContext currentHttpContest = HttpContext.Current;
            if (currentHttpContest == null)
                throw new CacheException("Missing Current Http Contest.");
            return currentHttpContest;
        }
    }
}
