
using UserManagement.Domain.Base;
using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Interfaces
{
    public interface ISignInManager
    {
        Task<IResponse> SignInAsync(ApplicationUser user, string password);
    }
}
