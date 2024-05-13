using UserManagement.Domain.Entities;

namespace UserManagment.WebAPI.Modals
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
        public AuthenticateResponse(UserColumn user, string token)
        {
            user = user ?? new UserColumn();
            Id = user.Id;
            UserName = user.UserName;
            Role = user.Role.ToString();
            Token = token;
        }
    }
}
