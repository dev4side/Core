using System;
using System.Security.Principal;
using System.Web;
using Core.Common.Cache;
using Core.Context.Entity;
using Core.Context.Exceptions;
using Core.Context.Interfaces;
using Core.Context.Web.Entity;
using Core.Kernel;

namespace Core.Context.Web
{
    public class WebContext : IContext
    {
        private readonly ICache _currentCache;

        public WebContext()
        {
            _currentCache = ObjectFactory.Get<ICache>();
        }

        #region IContext Members

        /// <summary>
        /// Gets the current facade user in the context.
        /// </summary>
        /// <returns></returns>
        public IFacadeContextUser GetCurrentUser()
        {
            return new ContextWebUser();
        }

        /// <summary>
        /// Gets the facade user in the context by identity..
        /// </summary>
        /// <param name="identityName">A System.String identity for the facade user to retrieve.</param>
        /// <returns>The facade user.</returns>
        public IFacadeContextUser GetUserByIdentityName(string identityName)
        {
            return new ContextWebUser(identityName);
        }

        /// <summary>
        /// Retrieves the current cache object.
        /// </summary>
        /// <returns>The cache object.</returns>
        public ICache GetCurrentCache()
        {
            return _currentCache;
        }

        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        /// <returns>true if the user was authenticated; otherwise, false.</returns>
        public bool IsCurrentUserAuthenticated()
        {
            IPrincipal currentUser = GetCurrentHttpContest().User;
            return currentUser != null && currentUser.Identity.IsAuthenticated;
        }

        /// <summary>
        /// Determines whether the current user belongs to the specified role.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <returns>true if the user belongs to the specified role; otherwise, false.</returns>
        public bool IsCurrentUserInRole(string role)
        {
            IPrincipal currentUser = GetCurrentHttpContest().User;
            return currentUser != null && currentUser.IsInRole(role);
        }

        /// <summary>
        /// Retrieves the specified resource from the global resource object.
        /// </summary>
        /// <param name="classKey">A string that represents the System.Web.Compilation.ResourceExpressionFields.ClassKey property 
        /// of the requested resource object.</param>
        /// <param name="resourceKey">A string that represents the System.Web.Compilation.ResourceExpressionFields.ResourceKey 
        /// property of the requested resource object.</param>
        /// <returns>An System.Object that represents the requested application-level resource object, or null if a resource object 
        /// is not found or if a resource object is found but it does not have the requested property.</returns>
        public object GetGlobalResourceObject(string classKey, string resourceKey)
        {
            return HttpContext.GetGlobalResourceObject(classKey, resourceKey);
        }

        #endregion

        /// <summary>
        /// Retrieves the specified resource from the global resource object.
        /// </summary>
        /// <returns>An System.Web.HttpContext that Encapsulates all HTTP-specific information about an individual HTTP request. It throws exception
        /// if the current http context is null.</returns>
        private HttpContext GetCurrentHttpContest()
        {
            HttpContext currentHttpContest = HttpContext.Current;

            if (currentHttpContest == null)
            {
                throw new ContextException("Missing Current Http Contest.");
            }

            return currentHttpContest;
        }        
    }
}