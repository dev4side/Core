using System;

namespace Core.Context.Entity
{
    public interface IFacadeContextUser
    {
        /// <summary>
        /// The name for the facade user.
        /// </summary>
        /// <returns>A System.String that represent the facade user name.</returns>
        string IdentityName { get; }
        
        /// <summary>
        /// The e-mail address for the facade user.
        /// </summary>
        /// <returns>A System.String that represent the e-mail address for the facade user.</returns>
        string Email { get; set; }
        
        /// <summary>
        /// Gets or sets whether the facade user is approved.
        /// </summary>
        /// <returns>true if the facade user is approved; otherwise, false.</returns>
        bool IsApproved { get; set; }
        
        /// <summary>
        /// Gets the date and time when the facade user was last authenticated.
        /// </summary>
        /// <returns>The date and time when the facade user was last authenticated.</returns>
        DateTime? LastLoginDate { get;}
        
        /// <summary>
        /// Gets the date and time when the facade user was last authenticated or accessed the application.
        /// </summary>
        /// <returns>The date and time when the facade user was last authenticated or accessed the application.</returns>
        DateTime? LastActivityDate { get; }
        
        /// <summary>
        /// Resets a user's password to a new, automatically generated password.
        /// </summary>
        /// <returns>The new password for the facade user.</returns>
        string ResetPassword();
        
        /// <summary>
        /// Updates the facade user in the data store.
        /// </summary>
        void Update();
        
        /// <summary>
        /// Deletes the facade user from the data store.
        /// </summary>
        /// <param name="deleteAllRelatedData">true to delete all data related to the facade user; false to maintain all data related to the facade user.</param>
        void Delete(bool deleteAllRelatedData);
        
        /// <summary>
        /// Updates the password for the facade user in the data store.
        /// </summary>
        /// <param name="oldPassword">The current password for the facade user.</param>
        /// <param name="newPassword">The new password for the facade user.</param>
        /// <returns>true if the update was successful; otherwise, false.</returns>
        bool ChangePassword(string oldPassword, string newPassword);
        
        /// <summary>
        /// Updates question and answer for the facade user in the data store.
        /// </summary>
        /// <param name="password">The current password for the facade user.</param>
        /// <param name="question">The new question for the facade user.</param>
        /// <param name="answer">The answer for the question for the facade user.</param>
        /// <returns>true if the update was successful; otherwise, false.</returns>
        bool ChangePasswordQuestionAndAnswer(string password, string question, string answer);
    }
}
