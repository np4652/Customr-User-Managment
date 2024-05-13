using Dapper;
using System.Data;
using System.Data.SqlClient;
using UserManagement.Domain.Base;

namespace UserManagement.Infrastructure
{
    public class DbContext: IDbContext
    {
        private readonly string _connectionString;
        private IDbConnection DB { get; set; }
        public DbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        
        protected IDbConnection Connection
        {
            get
            {
                if (DB == null || DB.State != ConnectionState.Open)
                {
                    DB = new SqlConnection(_connectionString);
                    DB.Open();
                }

                return DB;
            }
        }

        public async Task<int> ExecuteAsync(string sp, object param = null, CommandType commandType = CommandType.StoredProcedure)
        {
            int i = -1;
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    i = await db.ExecuteAsync(sp, param, commandType: commandType);
                }
            }
            catch (Exception ex)
            {
                var w32ex = ex as SqlException;
                if (w32ex == null)
                {
                    w32ex = ex.InnerException as SqlException;
                }
                if (w32ex != null)
                {
                    i = w32ex.Number;
                }
                //saveLog(new LogRequest
                //{
                //    Exception = ex.ToString(),
                //    Level = "LogError",
                //    Logger = this.GetType().Name,
                //    Msg = ex.Message,
                //    Trace = "ExecuteAsync",
                //    URL = string.Empty
                //});
            }
            return i;
        }
        
        public async Task<T> GetAsync<T>(string sp, object parms = null, CommandType commandType = CommandType.Text)
        {
            T result;
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    return await db.QueryFirstOrDefaultAsync<T>(sp, parms, commandType: commandType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>(string sp, object parms = null, CommandType commandType = CommandType.Text)
        {
            T result;
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    return await db.QueryAsync<T>(sp, parms, commandType: commandType);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
