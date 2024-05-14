using System.ComponentModel.DataAnnotations;
using UserManagement.Entities;


namespace UserManagment.Entities.DTOs
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
    }
}
