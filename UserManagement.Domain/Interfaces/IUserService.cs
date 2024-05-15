using UserManagement.Domain.Base;
using Usermanagment.Entities;

namespace UserManagement.Domain.Interfaces
{
    public interface IUserService : IService<UserRow,UserColumn, UserFilter>
    {
        Task<UserColumn> GetByUserName(string userName);
        Task<IResponse> SetGAuthAccountKey(string userName, string gAuthAccountKey);
        Task<IResponse> SetGAuthRequired(string userName, bool enable);
    }
}
