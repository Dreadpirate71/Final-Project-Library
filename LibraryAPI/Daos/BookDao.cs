using Dapper;
using LibraryAPI.Models;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Daos
{
    public class BookDao
    {
        private readonly DapperContext _context;
        public BookDao(DapperContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Book>> GetBooks()
        {
            var query = "SELECT * FROM Books";
            using (var connection = _context.CreateConnection())
            {
                var books = await connection.QueryAsync<Book>(query);
                return books.ToList();
            }
        }
        public async Task AddBook(string titleName, string authorFname, string authorLName, string type, decimal price, string Status, int patronId)
        {
            var query = "INSERT INTO Books (titleName, authorFname, authorLName, type, price, Status, patronId)" +
                $"VALUES )@titleName, @authorFname, @authorLName, @type, @price, @Status, @patronId);";

            var parameters = new DynamicParameters();
            parameters.Add("@titleName", titleName, DbType.String);
            parameters.Add("@authorFname", authorFname, DbType.String);
            parameters.Add("@authorLName", authorLName, DbType.String);
            parameters.Add("@type", type, DbType.String);
            parameters.Add("@price", price, DbType.Decimal);
            parameters.Add("@Status", Status, DbType.String);
            parameters.Add("@patronId", patronId, DbType.Int32);
            
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
