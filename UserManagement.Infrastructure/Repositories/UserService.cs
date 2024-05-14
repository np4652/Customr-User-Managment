using UserManagement.Domain.Base;
using UserManagement.Domain.Interfaces;
using Usermanagment.Entities;
using _query = SQLQuery.User;

namespace UserManagement.Infrastructure.Repositories
{
    public class UserService : IUserService
    {
        private readonly IDbContext _context;
        public UserService(IDbContext context)
        {
            _context = context;
        }
        public async Task<IResponse> Add(IServiceRequest<UserRow> entities)
        {
            return await _context.GetAsync<Response>(_query.Create, entities.param);
        }

        public async Task<IResponse> Delete(IServiceRequest<int> entities)
        {
            return await _context.GetAsync<IResponse>(_query.Delete, entities);
        }

        public async Task<IEnumerable<UserColumn>> GetAll(IServiceRequest<UserFilter> filter)
        {
            return await _context.GetAllAsync<UserColumn>(_query.GetAll, filter);
        }

        public async Task<UserColumn> GetById(IServiceRequest<int> filter)
        {
            return await _context.GetAsync<UserColumn>(_query.Get, new { id = filter.param });
        }

        public async Task<UserColumn> GetByUserName(string userName)
        {
            return await _context.GetAsync<UserColumn>(_query.GetByUserName, new { userName });
        }
    }
}
