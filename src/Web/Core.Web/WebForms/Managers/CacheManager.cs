using System;
using System.Web;
using System.Web.Caching;

namespace Core.Web.WebForms.Managers
{
    public class CacheManager<T>
    {
        public string CacheKey { get; set; }
        public int CacheDuration { get; set; }

        /// <summary>
        /// Init the manager, specifying a key and the duration of caching in minutes
        /// </summary>
        /// <param name="key">Cache key used to insert, retrieve or delete the object from the cache</param>
        /// <param name="duration">Duration in minutes</param>
        public CacheManager(string key, int duration)
        {
            CacheKey = key;
            CacheDuration = duration;
        }

        public bool Exists()
        {
            return (HttpContext.Current.Cache[CacheKey] != null);
        }

        public T Grab()
        {
            return (T)HttpContext.Current.Cache[CacheKey];
        }

        public void Insert(T obj, CacheItemPriority priority)
        {
            DateTime expiration = DateTime.Now.AddMinutes(CacheDuration);
            HttpContext.Current.Cache.Add(CacheKey, obj, null, expiration, TimeSpan.Zero, priority, null);
        }

        public void Clear()
        {
            HttpContext.Current.Cache.Remove(this.CacheKey);
        }
    }
}
