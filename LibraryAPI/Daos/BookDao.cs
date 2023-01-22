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
        public async Task<IEnumerable<BookModel>> GetBooks()
        {
            var query = "SELECT * FROM Books";
            using (var connection = _context.CreateConnection())
            {
                var books = await connection.QueryAsync<BookModel>(query);
                return books.ToList();
            }
        }
        public async Task AddBook(string bookTitle, string authorFName, string authorLName, string genre, decimal price, string status, int patronId)
        {
            var query = "INSERT INTO Books (bookTitle, authorFName, authorLName, genre, price, status, patronId)" +
                $"VALUES )@titleName, @authorFname, @authorLName, @genre, @price, @status, @patronId);";

            var parameters = new DynamicParameters();
            parameters.Add("@bookTitle", bookTitle, DbType.String);
            parameters.Add("@authorFName", authorFName, DbType.String);
            parameters.Add("@authorLName", authorLName, DbType.String);
            parameters.Add("@genre", genre, DbType.String);
            parameters.Add("@price", price, DbType.Decimal);
            parameters.Add("@status", status, DbType.String);
            parameters.Add("@patronId", patronId, DbType.Int32);
            
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
