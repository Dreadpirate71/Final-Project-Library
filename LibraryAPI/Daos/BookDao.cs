using Dapper;
using LibraryAPI.Models;
using LibraryAPI.Wrappers;
using Microsoft.OData.Edm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryAPI.Daos
{
    public class BookDao : IBookDao
    {
        private readonly DapperContext _context;
        private readonly ISqlWrapperBook _sqlWrapper;


        public BookDao(ISqlWrapperBook sqlWrapper)
        {
            this._sqlWrapper = sqlWrapper;
        }
        public BookDao(DapperContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<BookAvailableModel>> GetListOfAllBooks()
        {
            var query = "SELECT * FROM books_Available";
            using var connection = _context.CreateConnection();
            var books = await connection.QueryAsync<BookAvailableModel>(query);
            return books.ToList();
        }
        public async Task AddBook(string bookTitle, string authorFName, string authorLName, string genre, decimal price, string status)
        {
            var query = $"INSERT INTO books_Available (BookTitle, AuthorFName, AuthorLName, Genre, Price, Status)" +
                $"VALUES (@BookTitle, @AuthorFname, @AuthorLName, @Genre, @Price, @Status)";
            
            var parameters = new DynamicParameters();
            parameters.Add("@BookTitle", bookTitle, DbType.String);
            parameters.Add("@AuthorFName", authorFName, DbType.String);
            parameters.Add("@AuthorLName", authorLName, DbType.String);
            parameters.Add("@Genre", genre, DbType.String);
            parameters.Add("@Price", price, DbType.Decimal);
            parameters.Add("@Status", status, DbType.String);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, parameters);
        }
        
        public async Task UpdateBookByTitle(BookAvailableModel updateRequest)
        {
            var query = $"UPDATE books_Available SET BookTitle = @BookTitle, AuthorFName = @AuthorFName, AuthorLName = @AuthorLName, Genre = @Genre," +
                        $"Price = @Price, Status = @Status, WHERE BookTitle = @BookTitle";

            var parameters = new DynamicParameters();
            parameters.Add("@BookTitle", updateRequest.BookTitle, DbType.String);
            parameters.Add("@AuthorFName", updateRequest.AuthorFName, DbType.String);
            parameters.Add("@AuthorLName", updateRequest.AuthorLName, DbType.String);
            parameters.Add("@Genre", updateRequest.Genre, DbType.String);
            parameters.Add("@Price", updateRequest.Price, DbType.Decimal);
            parameters.Add("@Status", updateRequest.Status, DbType.String);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, parameters);
        }

        public async Task<BookAvailableModel> GetBookByTitle(string bookTitle)
        {
            var query = $"SELECT * FROM books_Available WHERE BookTitle = '{bookTitle}'";
            using var connection = _context.CreateConnection();
            var bookByTitle = await connection.QueryFirstOrDefaultAsync<BookAvailableModel>(query);
            return bookByTitle;
        }
        public async Task<BookAvailableModel> GetBookById(int Id)
        {
            var query = $"SELECT * FROM books_Available WHERE Id = '{Id}'";
            using (var connection = _context.CreateConnection())
            {
                var bookById = await connection.QueryFirstOrDefaultAsync<BookAvailableModel>(query);
                return bookById;
            }
        }
        public async Task DeleteBookById(int Id)
        {
            var query = $"DELETE FROM books_Available WHERE Id = '{Id}'";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query);
            }
        }
        public void GetBook()
        {
            _sqlWrapper.QueryBook<BookAvailableModel>("SELECT * FROM books_Available");
        }
        public void UpdateBook()
        {
            _sqlWrapper.QueryBook<BookAvailableModel>($"UPDATE books_Available SET BookTitle = @BookTitle, AuthorFName = @AuthorFName, AuthorLName = @AuthorLName, Genre = @Genre," +
                        $"Price = @Price, Status = @Status WHERE BookTitle = @BookTitle");
        }
        public void AddBook()
        {
            _sqlWrapper.QueryBook<BookAvailableModel>($"INSERT INTO books_Available (BookTitle, AuthorFName, AuthorLName, Genre, Price, Status)" +
                $"VALUES (@BookTitle, @AuthorFname, @AuthorLName, @Genre, @Price, @Status)");
        }
        public void GetBookTitle()
        {
            _sqlWrapper.QueryBook<BookAvailableModel>("SELECT * FROM books_Available WHERE BookTitle = '{bookTitle}'");
        }

        public void DeleteBook()
        {
            _sqlWrapper.QueryBook<BookAvailableModel>("DELETE * FROM books_Available WHERE Id = '{Id}'");
        }
    }
}
