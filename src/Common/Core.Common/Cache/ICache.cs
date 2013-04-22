using System;

namespace Core.Common.Cache
{
    /// <summary>
    /// Implements the cache for an application. 
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Retrieves the specified item from the cache object.
        /// </summary>
        /// <param name="cacheKey">A System.String identifier for the cache item to retrieve.</param>
        /// <returns>The retrieved cache item, or null if the key is not found.</returns>
        object GetFromCache(string cacheKey);

        /// <summary>
        /// Inserts an object into the cache object.
        /// </summary>
        /// <param name="cacheKey">A System.String identifier key that is used to reference the object.</param>
        /// <param name="value">The object to insert into the cache.</param>
        /// <param name="absoluteExpiration">The time at which the inserted object expires and is removed from the cache. 
        /// To avoid possible issues with local time such as changes from standard time to daylight saving time, use 
        /// System.DateTime.UtcNow instead of System.DateTime.Now for this parameter value.</param>
        /// <param name="slidingExpiration">The interval between the time that the cached object was last accessed and the 
        /// time at which that object expires. If this value is the equivalent of 20 minutes, the object will expire and be 
        /// removed from the cache 20 minutes after it was last accessed.</param>
        void AddToCache(string cacheKey, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration);
        
        /// <summary>
        /// Removes the specified item from the application's cache object.
        /// </summary>
        /// <param name="cacheKey">A System.String identifier for the cache item to remove.</param>
        void RemoveFromCache(string cacheKey);
        
        /// <summary>
        /// Retrieves the specified item from the session object.
        /// </summary>
        /// <param name="sessionKey">A System.String identifier for the session item to retrieve.</param>
        /// <returns>The retrieved cache item, or null if the key is not found.</returns>
        object GetFromSession(string sessionKey);
        
        /// <summary>
        /// Inserts an object into the session object.
        /// </summary>
        /// <param name="sessionKey">A System.String identifier key that is used to reference the object.</param>
        /// <param name="value">The object to insert into the session.</param>
        void AddToSession(string sessionKey, object value);
        
        /// <summary>
        /// Removes the specified item from the application's session object.
        /// </summary>
        /// <param name="sessionKey">A System.String identifier for the session item to remove.</param>
        void RemoveFromSession(string sessionKey);
        
        /// <summary>
        /// Retrieves the specified resource from the global resource object.
        /// </summary>
        /// <param name="classKey">A string that represents the System.Web.Compilation.ResourceExpressionFields.ClassKey property 
        /// of the requested resource object.</param>
        /// <param name="resourceKey">A string that represents the System.Web.Compilation.ResourceExpressionFields.ResourceKey 
        /// property of the requested resource object.</param>
        /// <returns>An System.Object that represents the requested application-level resource object, or null if a resource object 
        /// is not found or if a resource object is found but it does not have the requested property.</returns>
        object GetGlobalResourceObject(string classKey, string resourceKey);
    }
}