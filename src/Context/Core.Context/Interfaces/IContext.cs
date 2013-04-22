using Core.Common.Cache;
using Core.Context.Entity;

namespace Core.Context.Interfaces
{
    public interface IContext
    {
        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        /// <returns>true if the user was authenticated; otherwise, false.</returns>
        bool IsCurrentUserAuthenticated();
        
        /// <summary>
        /// Determines whether the current user belongs to the specified role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>true if the user belongs to the specified role; otherwise, false.</returns>
        bool IsCurrentUserInRole(string role);
        
        /// <summary>
        /// Gets the current facade user in the context.
        /// </summary>
        /// <returns></returns>
        IFacadeContextUser GetCurrentUser();
        
        /// <summary>
        /// Gets the facade user in the context by identity..
        /// </summary>
        /// <param name="identityName">A System.String identity for the facade user to retrieve.</param>
        /// <returns>The facade user.</returns>
        IFacadeContextUser GetUserByIdentityName(string identityName);
        
        /// <summary>
        /// Retrieves the current cache object.
        /// </summary>
        /// <returns>The cache object.</returns>
        ICache GetCurrentCache();

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
