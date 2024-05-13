using UserManagement.Domain.Base;
using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Interfaces
{
    public interface IUserService : IService<UserRow,UserColumn, UserFilter>
    {
        Task<UserColumn> GetByUserName(string userName);
    }
}
