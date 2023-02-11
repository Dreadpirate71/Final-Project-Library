using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LibraryAPI
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
        Task<IEnumerable<T>> QueryAsync<T>(string sql);
        Task ExecuteAsyncWithParameters(string sql, DynamicParameters parameters);
        Task ExecuteAsync<T>(string sql);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql);
    }
}
