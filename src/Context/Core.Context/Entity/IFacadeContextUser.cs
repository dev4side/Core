using System;

namespace Core.Context.Entity
{
    public interface IFacadeContextUser
    {
        string IdentityName { get; }
        string Email { get; set; }
        bool IsApproved { get; set; }
        DateTime? LastLoginDate { get;}
        DateTime? LastActivityDate { get; }
        string ResetPassword();
        void Update();
        void Delete(bool deleteAllRelatedData);
        bool ChangePassword(string oldPAssword, string newPassword);
        bool ChangePasswordQuestionAndAnswer(string password, string question, string answer);
    }
}
