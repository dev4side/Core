using System;
using Core.Common.Cache;
using Core.Context.Entity;

namespace Core.Context.Interfaces
{
    public interface IContext
    {
        bool IsCurrentUserAuthenticated();
        bool IsCurrentUserInRole(string role);
        IFacadeContextUser GetCurrentUser();
        IFacadeContextUser GetUserByIdentityName(string identityName);
        
        ICache GetCurrentCache();


        //object GetFromCache(string cacheKey);
        //void AddToCache(string cacheKey, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration);
        //void RemoveFromCache(string cacheKey);
        //object GetFromSession(string sessionKey);
        //void AddToSession(string sessionKey, object value);
        //void RemoveFromSession(string sessionKey);
        object GetGlobalResourceObject(string classKey, string resourceKey);
    }
}
