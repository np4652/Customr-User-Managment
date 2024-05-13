using System.ComponentModel.DataAnnotations;
using UserManagement.Domain.Base;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;

namespace UserManagment.WebAPI.Modals
{
    public class UserSignupRequest
    {
        public UserRole Role { get; set; }
        [Required, MaxLength(80)]
        public string? FirstName { get; set; }
        [Required, MaxLength(80)]
        public string? LastName { get; set; }
        [Required, MaxLength(10)]
        public string? MobileNo { get; set; }
        [Required, MaxLength(80)]
        public string? Email { get; set; }
        [Required, MinLength(8), MaxLength(15)]
        public string? Password { get; set; }

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
