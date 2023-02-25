using Dapper;
using LibraryAPI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Wrappers
{
    public class SqlWrapper : ISqlWrapper, ISqlWrapperPatron 
    {
        public static string ConnectionString;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private IDbConnection _connection;

        public SqlWrapper(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlConnection");
        }
        public IDbConnection CreateConnection()
        {
            var connection = new SqlConnection(_connectionString);
            this._connection = connection;
            return connection;
        }
        public SqlWrapper()
        {
            ConnectionString = "server=VUHL-DRMKKN3\\SQLExpress; database=Library; Integrated Security=True; TrustServerCertificate=True";
        }
        public Task<IEnumerable> QueryBook<BookModel>(string sql)
        {
            throw new System.NotImplementedException();
        }
        public Task<IEnumerable>QueryPatron<PatronModel>(string sql)
        {
            throw new System.NotImplementedException();
        }
        public Task<IEnumerable<PatronModel>> GetListOfAllPatrons()
        {
            throw new System.NotImplementedException();
        }
        
        public async Task ExecuteAsync<T>(string sql)
        {
            await _connection.ExecuteAsync(sql);

        }

        public async Task ExecuteAsyncWithParameters(string sql, DynamicParameters parameters)
        {
            await this.ExecuteAsyncWithParameters(sql, parameters);
        }

        public async Task<List<T>> QueryAsync<T>(string sql)
        {
            var result = await this._connection.QueryAsync<T>(sql);
            return result.ToList();
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql)
        {
            var result = await this._connection.QueryFirstOrDefaultAsync<T>(sql);
            return result;
        }
    }
}
