using Dapper;
using LibraryAPI.Models;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LibraryAPI.Wrappers
{
    public interface ISqlWrapper
    {
        IDbConnection CreateConnection();
        //T Query<T>(string sql, object? parameters = null);
        Task<List<T>> QueryAsync<T>(string sql);
        Task ExecuteAsyncWithParameters(string sql, DynamicParameters parameters);
        Task ExecuteAsync<T>(string sql);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql);
        Task<IEnumerable> QueryBook<BookModel>(string sql);
    }
    public interface ISqlWrapperPatron
    {
        Task<IEnumerable> QueryPatron<PatronModel>(string sql);
    }

    public interface ISqlWrapperStaff
    {
        Task<IEnumerable> QueryStaff(string sql);
    }
}
