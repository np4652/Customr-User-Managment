using System.Data;

namespace UserManagement.Domain.Base
{
    public interface IDbContext
    {
        Task<int> ExecuteAsync(string sp, object param = null, CommandType commandType = CommandType.StoredProcedure);
        Task<T> GetAsync<T>(string sp, object parms = null, CommandType commandType = CommandType.Text);
        Task<IEnumerable<T>> GetAllAsync<T>(string sp, object parms = null, CommandType commandType = CommandType.Text);
    }
}
