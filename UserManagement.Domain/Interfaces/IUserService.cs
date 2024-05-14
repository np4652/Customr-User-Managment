using UserManagement.Domain.Base;
using Usermanagment.Entities;

namespace UserManagement.Domain.Interfaces
{
    public interface IUserService : IService<UserRow,UserColumn, UserFilter>
    {
        Task<UserColumn> GetByUserName(string userName);
    }
}
