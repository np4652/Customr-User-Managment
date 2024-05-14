using UserManagement.Domain.Base;
using UserManagement.Domain.Interfaces;
using UserManagement.Entities;
using Usermanagment.Entities;

namespace UserManagement.Infrastructure.Services
{
    public class SignInManager : PasswordManager, ISignInManager
    {
        public async Task<IResponse> SignInAsync(ApplicationUser user, string password)
        {
            IResponse res = new Response();
            if (await VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                res.StatusCode = ResponseStatus.Success;
                res.ResponseText = ResponseStatus.Success.ToString();
            }
            return res;
        }
    }
}
