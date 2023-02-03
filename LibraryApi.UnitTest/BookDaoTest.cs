using LibraryAPI.Daos;
using LibraryAPI.Models;
using LibraryAPI.Wrappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApi.UnitTest
{
    [TestClass]
    public class BookDaoTests
    {
        private readonly Mock<ISqlWrapperBook> _mockSqlWrapper;
        private readonly BookDao _bookDaoMock;
        public BookDaoTests()
        {
            _mockSqlWrapper = new Mock<ISqlWrapperBook>();
            _bookDaoMock = new BookDao(_mockSqlWrapper.Object);
        }
        [TestMethod]
        public void CallSqlWithSelectString_VerifyQueries_MatchingExpressionsConfirmed()
        {
            _bookDaoMock.GetBook();
            _mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.QueryBook<BookAvailableModel>(It.Is<string>(sql => sql == "SELECT * FROM books_Available")), Times.Once);
        }
        [TestMethod]
        public void CallSqlWithUpdateString_VerifyQueries_MatchingExpressionsConfirmed()
        {
            _bookDaoMock.UpdateBook();
            _mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.QueryBook<BookAvailableModel>(It.Is<string>(sql => sql == "UPDATE books_Available SET BookTitle = @BookTitle, AuthorFName = @AuthorFName, AuthorLName = @AuthorLName, Genre = @Genre," +
                                   $"Price = @Price, Status = @Status WHERE BookTitle = @BookTitle")), Times.Once);
        }
        [TestMethod]
        public void CallSqlWithInsertString_VerifyQueries_MatchingExpressionsConfirmed()
        {
            _bookDaoMock.AddBook();
            _mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.QueryBook<BookAvailableModel>(It.Is<string>(sql => sql == "INSERT INTO books_Available (BookTitle, AuthorFName, AuthorLName, Genre, Price, Status)" +
                $"VALUES (@BookTitle, @AuthorFname, @AuthorLName, @Genre, @Price, @Status)")), Times.Once);
        }
        [TestMethod]
        public void CallSqlWithUpdateWhereString_VerifyQueries_MatchingExpressionsConfirmed()
        {
            _bookDaoMock.GetBookTitle();
            _mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.QueryBook<BookAvailableModel>(It.Is<string>(sql => sql == "SELECT * FROM books_Available WHERE BookTitle = '{bookTitle}'")), Times.Once);
        }
        [TestMethod]
        public void CallSqlWithDeleteString_VerifyQueries_MatchingExpressionsConfirmed()
        {
            _bookDaoMock.DeleteBook();
            _mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.QueryBook<BookAvailableModel>(It.Is<string>(sql => sql == "DELETE * FROM books_Available WHERE Id = '{Id}'")), Times.Once);
        }
    }
}
