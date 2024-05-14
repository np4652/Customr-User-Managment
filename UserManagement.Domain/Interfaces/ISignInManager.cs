using UserManagement.Domain.Base;
using Usermanagment.Entities;

namespace UserManagement.Domain.Interfaces
{
    public interface ISignInManager
    {
        Task<IResponse> SignInAsync(ApplicationUser user, string password);
    }
}
