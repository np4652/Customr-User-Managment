using UserManagement.Domain.Base;
using UserManagement.Entities;

namespace Usermanagment.Entities
{
    public class ApplicationUser
    {
        public int Id { get; internal set; }
        public string UserName { get; internal set; }
        public string PasswordHash { get;internal set; }
        public string PasswordSalt { get; internal set; }
    }

    public class UserRow : IRow
    {
        public int Id { get; set; }
        public UserRole Role { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public bool GAuthRequired { get; set; }
        public string GAuthAccountKey { get; set; }
    }

    public class UserColumn : IColumn
    {
        public int Id { get; set; }
        public UserRole Role { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public bool GAuthRequired { get; set; }
        public string GAuthAccountKey { get; set; }
        public ApplicationUser ToApplicationUser()
        {
            return new ApplicationUser
            {
                Id = Id,
                UserName = UserName,
                PasswordHash = PasswordHash,
                PasswordSalt = PasswordSalt,
            };
        }
    }

    public class UserFilter : IFilter
    {

    }
}