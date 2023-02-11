using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LibraryAPI
{
    public class DapperContext : IDapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private IDbConnection _connection;

        public DapperContext(IConfiguration configuration)
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

        public async Task ExecuteAsync<T>(string sql)
        {
            await _connection.ExecuteAsync(sql);
            
        }

        public async Task ExecuteAsyncWithParameters(string sql, DynamicParameters parameters)
        {
            await this.ExecuteAsyncWithParameters(sql, parameters);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql)
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
