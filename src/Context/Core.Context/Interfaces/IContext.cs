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
        object GetGlobalResourceObject(string classKey, string resourceKey);
    }
}
