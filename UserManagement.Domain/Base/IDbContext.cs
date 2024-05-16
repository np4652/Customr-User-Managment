using System.Data;

namespace UserManagement.Domain.Base
{
    public interface IDbContext
    {
        IDbConnection Connection { get; }
        Task<int> ExecuteAsync(string sp, object param, IDbTransaction transaction, CommandType commandType = CommandType.StoredProcedure);
        Task<int> ExecuteAsync(string sp, object param = null, CommandType commandType = CommandType.StoredProcedure);
        Task<T> GetAsync<T>(string sp, object parms = null, CommandType commandType = CommandType.Text);
        Task<IEnumerable<T>> GetAllAsync<T>(string sp, object parms = null, CommandType commandType = CommandType.Text);
    }
}
