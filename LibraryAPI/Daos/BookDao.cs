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
        private readonly ISqlWrapper _sqlWrapper;
        private readonly IDapperContext _dapperContext;


        public BookDao(ISqlWrapper sqlWrapper)
        {
            this._sqlWrapper = sqlWrapper;
        }
        public BookDao(DapperContext context)
        {
            _context = context;
        }
        public BookDao(IDapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public async Task<IEnumerable<BookModel>> GetListOfAllBooks()
        {
            var query = "SELECT * FROM Books";
            using var connection = _context.CreateConnection();
            var books = await connection.QueryAsync<BookModel>(query);
            return books.ToList();
        }

        public async Task<IEnumerable<BookModel>> GetListOfAllAvailableBooks()
        {
            var query = "SELECT * FROM Books WHERE Status = 'In'";
            using var connection = _context.CreateConnection();
            var books = await connection.QueryAsync<BookModel>(query);
            return books.ToList();
        }
        public async Task AddBook(string bookTitle, string authorFName, string authorLName, string genre, decimal price, string status, string checkOutDate, int patronId)
        {
            var query = $"INSERT INTO Books (BookTitle, AuthorFName, AuthorLName, Genre, Price, Status, CheckOutDate, PatronId)" +
                $"VALUES (@BookTitle, @AuthorFname, @AuthorLName, @Genre, @Price, @Status, @CheckOutDate, @PatronId)";

            var parameters = new DynamicParameters();
            parameters.Add("@BookTitle", bookTitle, DbType.String);
            parameters.Add("@AuthorFName", authorFName, DbType.String);
            parameters.Add("@AuthorLName", authorLName, DbType.String);
            parameters.Add("@Genre", genre, DbType.String);
            parameters.Add("@Price", price, DbType.Decimal);
            parameters.Add("@Status", status, DbType.String);
            parameters.Add("@CheckOutDate", checkOutDate, DbType.String);
            parameters.Add("@PatronId", 1003, DbType.Int32);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, parameters);
        }

        public async Task UpdateBookByTitle(BookModel updateRequest)
        {
            var query = $"UPDATE Books SET BookTitle = @BookTitle, AuthorFName = @AuthorFName, AuthorLName = @AuthorLName, Genre = @Genre," +
                        $"Price = @Price, Status = @Status, CheckOutDate = @CheckOutDate, PatronId = @PatronId WHERE BookTitle = @BookTitle";

            var parameters = new DynamicParameters();
            parameters.Add("@BookTitle", updateRequest.BookTitle, DbType.String);
            parameters.Add("@AuthorFName", updateRequest.AuthorFName, DbType.String);
            parameters.Add("@AuthorLName", updateRequest.AuthorLName, DbType.String);
            parameters.Add("@Genre", updateRequest.Genre, DbType.String);
            parameters.Add("@Price", updateRequest.Price, DbType.Decimal);
            parameters.Add("@Status", updateRequest.Status, DbType.String);
            parameters.Add("@CheckOutDate", updateRequest.CheckOutDate, DbType.String);
            parameters.Add("@PatronId", updateRequest.PatronId, DbType.Int32);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, parameters);
        }

        public async Task<BookModel> GetBookByTitle(string bookTitle)
        {
            bookTitle = bookTitle.Replace("'", "''");
            var query = $"SELECT * FROM Books WHERE BookTitle = '{bookTitle}'";
            using var connection = _context.CreateConnection();
            var bookByTitle = await connection.QueryFirstOrDefaultAsync<BookModel>(query);
            return bookByTitle;
        }
        public async Task<BookModel> GetBookById(int Id)
        {
            var query = $"SELECT * FROM Books WHERE Id = '{Id}'";
            using (var connection = _context.CreateConnection())
            {
                var bookById = await connection.QueryFirstOrDefaultAsync<BookModel>(query);
                return bookById;
            }
        }
        public async Task DeleteBookById(int Id)
        {
            var query = $"DELETE FROM Books WHERE Id = '{Id}'";
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query);
            }
        }
        public async Task<int> GetTotalOfCheckedOutBooks(int patronId)
        {
            var query = $"SELECT * FROM Books WHERE PatronId = '{patronId}'";
            using var connection = _context.CreateConnection();
            var booksOut = await connection.QueryAsync<BookModel>(query);
            var numberBooksOut = booksOut.ToList().Count();
            return numberBooksOut;
        }
        public async Task<IEnumerable<BookModel>> GetListOfBooksCheckedOut(int patronId)
        {
            var query = $"SELECT * FROM Books WHERE PatronId = '{patronId}'";
            using var connection = _context.CreateConnection();
            var booksOut = await connection.QueryAsync<BookModel>(query);
            return booksOut.ToList();
        }
        public void GetBook()
        {
            _sqlWrapper.QueryBook<BookModel>("SELECT * FROM Books");
        }
        public void UpdateBook()
        {
            _sqlWrapper.QueryBook<BookModel>($"UPDATE Books SET BookTitle = @BookTitle, AuthorFName = @AuthorFName, AuthorLName = @AuthorLName, Genre = @Genre," +
                        $"Price = @Price, Status = @Status, CheckOutDate = @CheckOutDate, PatronId = @PatronId WHERE BookTitle = @BookTitle");
        }
        public void AddBook()
        {
            _sqlWrapper.QueryBook<BookModel>($"INSERT INTO Books (BookTitle, AuthorFName, AuthorLName, Genre, Price, Status, CheckOutDate, PatronId)" +
                $"VALUES (@BookTitle, @AuthorFname, @AuthorLName, @Genre, @Price, @Status, @CheckOutDate, @PatronId)");
        }
        public void GetBookTitle()
        {
            _sqlWrapper.QueryBook<BookModel>("SELECT * FROM Books WHERE BookTitle = '{bookTitle}'");
        }

        public void DeleteBook()
        {
            _sqlWrapper.QueryBook<BookModel>("DELETE FROM Books WHERE Id = '{Id}'");
        }
        public async Task<IEnumerable<BookModel>> GetBookByGenre(string Genre)
        {
            var query = $"SELECT * FROM Books WHERE Genre = '{Genre}'";
            using var connection = _context.CreateConnection();
            var books = await connection.QueryAsync<BookModel>(query);
            return books.ToList();
        }
        public async Task <IEnumerable<string>> GetListOfGenres()
        {
            var query = $"SELECT DISTINCT Genre FROM Books";
            using var connection = _context.CreateConnection();
            var genres = await connection.QueryAsync<string>(query);
            return genres.ToList();
        }
    }
}
