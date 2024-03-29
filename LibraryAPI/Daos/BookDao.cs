﻿using Dapper;
using LibraryAPI.Models;
using LibraryAPI.Wrappers;
using Microsoft.OData.Edm;
using Org.BouncyCastle.Math.Field;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


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
        public async Task AddBook(string bookTitle, string authorFName, string authorLName, string genre, decimal price)
        {
            var query = $"INSERT INTO Books (BookTitle, AuthorFName, AuthorLName, Genre, Price, Status, CheckOutDate, PatronId)" +
                $"VALUES (@BookTitle, @AuthorFname, @AuthorLName, @Genre, @Price, @Status, @CheckOutDate, @PatronId)";

            var parameters = new DynamicParameters();
            parameters.Add("@BookTitle", bookTitle, DbType.String);
            parameters.Add("@AuthorFName", authorFName, DbType.String);
            parameters.Add("@AuthorLName", authorLName, DbType.String);
            parameters.Add("@Genre", genre, DbType.String);
            parameters.Add("@Price", price, DbType.Decimal);
            parameters.Add("@Status", "In", DbType.String);
            parameters.Add("@CheckOutDate", null, DbType.Date);
            parameters.Add("@PatronId", 1003, DbType.Int32);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, parameters);
        }

        public async Task UpdateBookById(BookModel updateRequest)
        {
            var query = $"UPDATE Books SET BookTitle = @BookTitle, AuthorFName = @AuthorFName, AuthorLName = @AuthorLName, Genre = @Genre," +
                        $"Price = @Price, Status = @Status, CheckOutDate = @CheckOutDate, PatronId = @PatronId WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", updateRequest.Id, DbType.Int32);
            parameters.Add("@BookTitle", updateRequest.BookTitle, DbType.String);
            parameters.Add("@AuthorFName", updateRequest.AuthorFName, DbType.String);
            parameters.Add("@AuthorLName", updateRequest.AuthorLName, DbType.String);
            parameters.Add("@Genre", updateRequest.Genre, DbType.String);
            parameters.Add("@Price", updateRequest.Price, DbType.Decimal);
            parameters.Add("@Status", updateRequest.Status, DbType.String);
            if (updateRequest.CheckOutDate == null)
            { parameters.Add("@CheckOutDate", null, DbType.Date); }
            else
            { parameters.Add("@CheckOutDate", updateRequest.CheckOutDate.ToString(), DbType.Date); }
            parameters.Add("@PatronId", updateRequest.PatronId, DbType.Int32);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, parameters);
        }
        public async Task BookWaitList(int patronId, string bookTitle, string authorFName, string authorLName)
        {
            //bookTitle = bookTitle.Replace("'", "''");
            var query = $"INSERT INTO BookRequests(PatronId, BookTitle, AuthorFName, AuthorLName, WaitList)" +
                        $"VALUES (@PatronId, @BookTitle, @AuthorFName, @AuthorLName, @WaitList)";

            var parameters = new DynamicParameters();
            parameters.Add("@PatronId", patronId, DbType.Int32);
            parameters.Add("@BookTitle", bookTitle, DbType.String);
            parameters.Add("@AuthorFName", authorFName, DbType.String);
            parameters.Add("@AuthorLName", authorLName, DbType.String);
            parameters.Add("@WaitList", "Yes", DbType.String);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(query, parameters);
        }
        
        
        public async Task BookRequestList(int patronId, string bookTitle, string authorFName, string authorLName)
        {
            var query = $"INSERT INTO BookRequests(PatronId, BookTitle, AuthorFName, AuthorLName, WaitList)" +
                        $"VALUES (@PatronId, @BookTitle, @AuthorFName, @AuthorLName, @WaitList)";

            var parameters = new DynamicParameters();
            parameters.Add("@PatronId", patronId, DbType.Int32);
            parameters.Add("@BookTitle", bookTitle, DbType.String);
            parameters.Add("@AuthorFName", authorFName, DbType.String);
            parameters.Add("@AuthorLName", authorLName, DbType.String);
            parameters.Add("@WaitList", "No", DbType.String);

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
        public async Task DeleteWaitListBook(int patronId, string bookTitle)
        {
            bookTitle = bookTitle.Replace("'", "''");
            var query = $"DELETE FROM BookRequests WHERE PatronId = '{patronId}' AND BookTitle = '{bookTitle}'";
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
            var query = $"SELECT * FROM Books WHERE PatronId = '{patronId}' AND Status = 'Out'";
            using var connection = _context.CreateConnection();
            var booksOut = await connection.QueryAsync<BookModel>(query);
            return booksOut.ToList();
        }

        
        public async Task<IEnumerable<BookModel>> GetOverdueBooks()
        {
            var query = $"SELECT * FROM Books WHERE CheckOutDate < DATEADD(DAY, -14, GETDATE())";
            using var connection = _context.CreateConnection();
            var books = await connection.QueryAsync<BookModel>(query);
            return books.ToList();

        }
        public async Task<BookModel> GetBookByTitleAndId(string bookTitle, int patronId)
        {
            bookTitle = bookTitle.Replace("'", "''");
            var query = $"SELECT * FROM Books WHERE BookTitle = '{bookTitle}' AND PatronId= '{patronId}'";
            using var connection = _context.CreateConnection();
            var bookOut = await connection.QueryFirstOrDefaultAsync<BookModel>(query);
            return bookOut;

        }
        public async Task<IEnumerable<BookModel>> GetBookByGenre(string Genre)
        {
            var query = $"SELECT * FROM Books WHERE Genre = '{Genre}'";
            using var connection = _context.CreateConnection();
            var books = await connection.QueryAsync<BookModel>(query);
            return books.ToList();
        }
        public async Task<IEnumerable<string>> GetListOfGenres()
        {
            var query = $"SELECT DISTINCT Genre FROM Books";
            using var connection = _context.CreateConnection();
            var genres = await connection.QueryAsync<string>(query);
            return genres.ToList();
        }
        public async Task<IEnumerable<BookRequestModel>> GetWaitListBooks()
        {
            var query = $"SELECT * FROM BookRequests WHERE WaitList = 'Yes'";
            using var connection = _context.CreateConnection();
            var waitListBooks = await connection.QueryAsync<BookRequestModel>(query);
            return waitListBooks.ToList();
        }
        public async Task<IEnumerable<BookRequestModel>> CheckForBookOnWaitList(string bookTitle)
        {
            bookTitle = bookTitle.Replace("'", "''");
            var query = $"SELECT * FROM BookRequests WHERE BookTitle = '{bookTitle}' AND WaitList = 'Yes'";
            using var connection = _context.CreateConnection();
            var waitListBook = await connection.QueryAsync<BookRequestModel>(query);
            return waitListBook.ToList();
        }

        public async Task<IEnumerable<BookRequestModel>> GetRequestListBooks()
        {
            var query = $"SELECT * FROM BookRequests WHERE WaitList = 'No'";
            using var connection = _context.CreateConnection();
            var requestBooks = await connection.QueryAsync<BookRequestModel>(query);
            return requestBooks.ToList();
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
    }
}
