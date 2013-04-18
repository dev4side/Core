using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Core.Context.Entity;
using Core.Context.Exceptions;

namespace Core.Context.Web.Entity
{
    public class ContextWebUser : IFacadeContextUser
    {
        private readonly MembershipUser _membershipUser;

        public ContextWebUser(string identityName)
        {
            _membershipUser = Membership.GetUser(identityName);
            if(_membershipUser == null)
                throw new ContextException("The user returned from the current context is null.");
        }

        public ContextWebUser()
        {
            IPrincipal currentUser = GetCurrentHttpContest().User;
            if (currentUser == null)
                throw new ContextException("The user returned from the current context is null.");
            //todo: TOGLIERE IL CONTROLLO SU NT!
            _membershipUser = Membership.GetUser(GetCurrentHttpContest().Request.LogonUserIdentity.Name.Contains("NT") ? "service" : currentUser.Identity.Name);
            if (_membershipUser == null)
                throw new ContextException(String.Format("The user returned from the Membership with identity name {0} is null.", currentUser.Identity.Name));
        }

        #region IContextUser Members

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        public string IdentityName
        {
            get { return _membershipUser.UserName; }
        }


        public DateTime? LastLoginDate
        {
            get { return _membershipUser.LastLoginDate; }
        }

        public DateTime? LastActivityDate
        {
            get { return _membershipUser.LastActivityDate; }
        }

        public string Email
        {
            get { return _membershipUser.Email; }
            set { _membershipUser.Email = value; }
        }

        public bool IsApproved
        {
            get { return _membershipUser.IsApproved; }
            set { _membershipUser.IsApproved = value; }
        }

        public bool ChangePasswordQuestionAndAnswer(string password, string question, string answer)
        {
           return _membershipUser.ChangePasswordQuestionAndAnswer(password, question, answer);
        }
        
        public string ResetPassword()
        {
           return _membershipUser.ResetPassword();
        }

        public void Update()
        {
            Membership.UpdateUser(_membershipUser);
        }

        public void Delete(bool deleteAllRelatedData)
        {
            Membership.DeleteUser(_membershipUser.UserName, deleteAllRelatedData);
        }

        public bool ChangePassword(string oldPAssword, string newPassword)
        {
           return _membershipUser.ChangePassword(oldPAssword, newPassword);
        }

        #endregion

        private HttpContext GetCurrentHttpContest()
        {
            HttpContext currentHttpContest = HttpContext.Current;
            if (currentHttpContest == null)
                throw new ContextException("Missing Current Http Contest.");
            return currentHttpContest;
        }
    }
}