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

        public IFacadeContextUser GetCurrentUser()
        {
            return new ContextWebUser();
        }

        public IFacadeContextUser GetUserByIdentityName(string identityName)
        {
            return new ContextWebUser(identityName);
        }

        public ICache GetCurrentCache()
        {
            return _currentCache;
        }

        public bool IsCurrentUserAuthenticated()
        {
            IPrincipal currentUser = GetCurrentHttpContest().User;
            return currentUser != null && currentUser.Identity.IsAuthenticated;
        }

        /// <summary>
        /// Determines whether the current principal belongs to the specified role.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool IsCurrentUserInRole(string role)
        {
            IPrincipal currentUser = GetCurrentHttpContest().User;
            return currentUser != null && currentUser.IsInRole(role);
        }

        public object GetGlobalResourceObject(string classKey, string resourceKey)
        {
            return HttpContext.GetGlobalResourceObject(classKey, resourceKey);
        }

        #endregion

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