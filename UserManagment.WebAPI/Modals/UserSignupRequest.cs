using UserManagement.Domain.Interfaces;
using Usermanagment.Entities;

namespace UserManagment.WebAPI.Modals
{
    public class UserSignupRequest: Entities.DTOs.UserSignupRequest
    {
        public UserRow ToUserRow(IPasswordManager passwordManager)
        {
            var passwordHash = passwordManager.HashPassword(Password, out string salt).Result;
            return new UserRow
            {
                FirstName = FirstName,
                LastName = LastName,
                MobileNo = MobileNo,
                Email = Email,
                UserName = Email,
                PasswordHash = passwordHash,
                PasswordSalt = salt
            };
        }
    }
}
