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
            
            if (_membershipUser == null)
            {
                throw new ContextException("The user returned from the current context is null.");
            }
        }

        public ContextWebUser()
        {
            IPrincipal currentUser = GetCurrentHttpContest().User;
            if (currentUser == null)
            {
                throw new ContextException("The user returned from the current context is null.");
            }
            
            //todo: TOGLIERE IL CONTROLLO SU NT!
            _membershipUser = Membership.GetUser(GetCurrentHttpContest().Request.LogonUserIdentity.Name.Contains("NT") ? "service" : currentUser.Identity.Name);

            if (_membershipUser == null)
            {
                throw new ContextException(
                    String.Format("The user returned from the Membership with identity name {0} is null.", currentUser.Identity.Name));
            }
        }

        #region IContextUser Members

        /// <summary>
        /// The name for the facade user.
        /// </summary>
        /// <returns>A System.String that represent the facade user name.</returns>
        public string IdentityName
        {
            get { return _membershipUser.UserName; }
        }

        /// <summary>
        /// Gets the date and time when the facade user was last authenticated.
        /// </summary>
        /// <returns>The date and time when the facade user was last authenticated.</returns>
        public DateTime? LastLoginDate
        {
            get { return _membershipUser.LastLoginDate; }
        }

        /// <summary>
        /// Gets the date and time when the facade user was last authenticated or accessed the application.
        /// </summary>
        /// <returns>The date and time when the facade user was last authenticated or accessed the application.</returns>
        public DateTime? LastActivityDate
        {
            get { return _membershipUser.LastActivityDate; }
        }

        /// <summary>
        /// The e-mail address for the facade user.
        /// </summary>
        /// <returns>A System.String that represent the e-mail address for the facade user.</returns>
        public string Email
        {
            get { return _membershipUser.Email; }
            set { _membershipUser.Email = value; }
        }

        /// <summary>
        /// Gets or sets whether the facade user is approved.
        /// </summary>
        /// <returns>true if the facade user is approved; otherwise, false.</returns>
        public bool IsApproved
        {
            get { return _membershipUser.IsApproved; }
            set { _membershipUser.IsApproved = value; }
        }

        /// <summary>
        /// Updates question and answer for the facade user in the data store.
        /// </summary>
        /// <param name="password">The current password for the facade user.</param>
        /// <param name="question">The new question for the facade user.</param>
        /// <param name="answer">The answer for the question for the facade user.</param>
        /// <returns>true if the update was successful; otherwise, false.</returns>
        public bool ChangePasswordQuestionAndAnswer(string password, string question, string answer)
        {
           return _membershipUser.ChangePasswordQuestionAndAnswer(password, question, answer);
        }

        /// <summary>
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <returns>The new password for the facade user.</returns>
        public string ResetPassword()
        {
           return _membershipUser.ResetPassword();
        }

        /// <summary>
        /// Updates the facade user in the data store.
        /// </summary>
        public void Update()
        {
            Membership.UpdateUser(_membershipUser);
        }

        /// <summary>
        /// Deletes the facade user from the data store.
        /// </summary>
        /// <param name="deleteAllRelatedData">true to delete all data related to the facade user; false to maintain all data related to the facade user.</param>
        public void Delete(bool deleteAllRelatedData)
        {
            Membership.DeleteUser(_membershipUser.UserName, deleteAllRelatedData);
        }

        /// <summary>
        /// Updates the password for the facade user in the data store.
        /// </summary>
        /// <param name="oldPassword">The current password for the facade user.</param>
        /// <param name="newPassword">The new password for the facade user.</param>
        /// <returns>true if the update was successful; otherwise, false.</returns>
        public bool ChangePassword(string oldPassword, string newPassword)
        {
           return _membershipUser.ChangePassword(oldPassword, newPassword);
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