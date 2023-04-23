using LibraryAPI.Controllers;
using LibraryAPI.Daos;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuGet.Frameworks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using TestContext = Microsoft.VisualStudio.TestTools.UnitTesting.TestContext;

namespace LibraryApi.UnitTest
{
    [TestClass]
    public class Initialize
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            Console.WriteLine("Inside AssemblyInitialize");
        }
    }

    [TestClass]
    public class DeInitialize
    {
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Console.WriteLine("Inside AssemblyCleanup");
        }
    }

    [TestClass]
    public class BooksControllerTests
    {
        private readonly Mock<IBookDao> _bookDaoMock;
        private readonly Mock<IPatronDao> _patronDaoMock;
        private readonly Mock<IStaffDao> _staffDaoMock;
        private readonly BooksController _booksControllerMock;
        private readonly PatronsController _patronsControllerMock;
        private readonly IEnumerable<BookModel>? _booksNull;
        private BookModel _bookModelMock;
        private BookRequestModel _bookRequestModelMock;
        private StaffModel _staffModelMock;
        private PatronModel _patronModelMock;
        private IEnumerable<BookModel>? _books;
        private BookModel? _bookModelMockNull;
        private PatronModel? _patronModelMockNull;
        private BookRequestModel? _bookRequestModelMockNull;
        private readonly IEnumerable<string> _genres;
        private IEnumerable<BookRequestModel>? _waitListBooks;
        private readonly IEnumerable<string> _booksHistory;
        private readonly IEnumerable<string>? _booksHistoryNull;


        public BooksControllerTests()
        {
            _bookDaoMock = new Mock<IBookDao>();
            _patronDaoMock = new Mock<IPatronDao>();
            _staffDaoMock = new Mock<IStaffDao>();
            _booksControllerMock = new BooksController(_bookDaoMock.Object, _patronDaoMock.Object, _staffDaoMock.Object);
            _patronsControllerMock = new PatronsController(_patronDaoMock.Object, _staffDaoMock.Object);
            _bookModelMock = new BookModel() { Id = 1200, BookTitle = "Wonder and Chaos of Being", AuthorFName = "Kris", AuthorLName = "Remus", Genre = "Fiction", Price = (decimal)12.00, Status = "In", DueDate = null, PatronId = 1003 };
            _patronModelMock = new PatronModel();
            _staffModelMock = new StaffModel();
            _bookRequestModelMock = new BookRequestModel();
            _books = new List<BookModel>() { _bookModelMock };
            _waitListBooks = new List<BookRequestModel>() { _bookRequestModelMock, _bookRequestModelMock };
            _genres = new List<string>() { "History", "Education", "Young Adult Fiction" };
            _booksHistory = new List<string>() { "Wonder and Chaos of Being|HarryPotter and the Philosopher's Stone|New Moon" };

        }
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Console.WriteLine("Inside ClassInitialize");
        }
        [ClassCleanup]
        public static void ClassCleanup()
        {
            Console.WriteLine("Inside ClassCleanup");
        }


        [TestMethod]
        public async Task AddBookTest_ActionExecutes_ReturnsOkMessageWhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod AddBookTest");
            _bookDaoMock.Setup(book => book.AddBook(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>())).Returns(Task.FromResult(_bookModelMock));

            //Act
            var result = await _booksControllerMock.AddBook("C# Player's Guide", "RB", "Whitaker", "Educational", (decimal)12.00);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual("C# Player's Guide has been added to library.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_BookByIdExecutes_ReturnsOkMessageWithObjectWhenSuccessful()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns success message.");
            _bookDaoMock.Setup(book => book.GetBookById(It.IsAny<int>())).Returns(Task.FromResult(_bookModelMock));

            //Act
            var result = await _booksControllerMock.GetBook(1, null, null, null, 0, 0, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual(_bookModelMock, (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_BookByTitleExecutes_ReturnsOkMessageWithObjectWhenSuccessful()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns success message.");
            _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));

            //Act
            var result = await _booksControllerMock.GetBook(0, "title", null, null, 0, 0, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual(_bookModelMock, (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_BookByAuthorExecutes_ReturnsOkMessageWithObjectWhenSuccessful()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns success message.");
            _bookDaoMock.Setup(books => books.GetBooksByAuthorLName(It.IsAny<string>())).Returns(Task.FromResult(_books));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, "author", null, 0, 0, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual(_books, (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_BookByGenreExecutes_ReturnsOkMessageWithObjectWhenSuccessful()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns success message.");
            _bookDaoMock.Setup(books => books.GetBookByGenre(It.IsAny<string>())).Returns(Task.FromResult(_books));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, null, "genre", 0, 0, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual(_books, (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_BookCurrentOutByPatronExecutes_ReturnsOkMessageWithObjectWhenSuccessful()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns success message.");
            _patronDaoMock.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModelMock));
            _bookDaoMock.Setup(books => books.GetListOfBooksCheckedOut(It.IsAny<int>())).Returns(Task.FromResult(_books));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, null, null, 1, 0, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual(_books, (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_BooksHistoryByPatronExecutes_ReturnsOkMessageWithObjectWhenSuccessful()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns success message.");
            _patronDaoMock.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModelMock));
            _bookDaoMock.Setup(books => books.GetBooksHistory(It.IsAny<int>())).Returns(Task.FromResult(_booksHistory));
            var booksHistory = _booksHistory.ToList();
            var booksList = booksHistory[0].Split('|');

            //Act
            var result = await _booksControllerMock.GetBook(0, null, null, null, 0, 1, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task GetBooksTest_BookWaitListExecutes_ReturnsOkMessageWithObjectWhenSuccessful()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns success message.");
            _bookDaoMock.Setup(books => books.GetWaitListBooks()).Returns(Task.FromResult(_waitListBooks));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, null, null, 0, 0, "Yes", null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual(_waitListBooks, (result as ObjectResult).Value);
        }


        [TestMethod]
        public async Task GetBooksTest_OverdueBooksExecutes_ReturnsOkMessageWithObjectWhenSuccessful()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns success message.");
            _bookDaoMock.Setup(books => books.GetOverdueBooks()).Returns(Task.FromResult(_books));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, null, null, 0, 0, null, "Yes", null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual(_books, (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_BooksByStatusExecutes_ReturnsOkMessageWithObjectWhenSuccessful()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns success message.");
            _bookDaoMock.Setup(books => books.GetListOfBooksByStatus(It.IsAny<string>())).Returns(Task.FromResult(_books));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, null, null, 0, 0, null, null, "In");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual(_books, (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_ActionExecutes_ReturnsIdNullMessage()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns error message.");

            //Act
            var result = await _booksControllerMock.GetBook(1, null, null, null, 0, 0, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
            Assert.AreEqual("No book found with that Id.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_ActionExecutes_ReturnsTitleNullMessage()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns error message.");
            _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMockNull));

            //Act
            var result = await _booksControllerMock.GetBook(0, "title", null, null, 0, 0, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
            Assert.AreEqual("No book found with that title.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_ActionExecutes_ReturnsAuthorNullMessage()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns error message.");
            _books = null;
            _bookDaoMock.Setup(books => books.GetBooksByAuthorLName(It.IsAny<string>())).Returns(Task.FromResult(_books));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, "author", null, 0, 0, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
            Assert.AreEqual("No books found by that author.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_ActionExecutes_ReturnsGenreNullMessage()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns error message.");
            _books = null;
            _bookDaoMock.Setup(books => books.GetBookByGenre(It.IsAny<string>())).Returns(Task.FromResult(_books));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, null, "genre", 0, 0, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
            Assert.AreEqual("No books found in that genre.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_ActionExecutes_ReturnsPatronNullMessage()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns error message.");
            _books = null;
            _patronDaoMock.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModelMockNull));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, null, null, 1, 0, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
            Assert.AreEqual("Patron with that Id does not exist.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_ActionExecutes_ReturnsCheckedNullMessage()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns error message.");
            _books = null;
            _patronDaoMock.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModelMock));
            _bookDaoMock.Setup(books => books.GetListOfBooksCheckedOut(It.IsAny<int>())).Returns(Task.FromResult(_books));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, null, null, 1, 0, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
            Assert.AreEqual("No books checked out by patron with that Id.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_ActionExecutes_ReturnsPatronHistoryNullMessage()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns error message.");
            _books = null;
            _patronDaoMock.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModelMockNull));
            _bookDaoMock.Setup(books => books.GetListOfBooksCheckedOut(It.IsAny<int>())).Returns(Task.FromResult(_books));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, null, null, 0, 1, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
            Assert.AreEqual("Patron with that Id does not exist.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_ActionExecutes_ReturnsBooksHistoryNullMessage()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns error message.");
            _patronDaoMock.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModelMock));
            _bookDaoMock.Setup(books => books.GetBooksHistory(It.IsAny<int>())).Returns(Task.FromResult(_booksHistoryNull));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, null, null, 0, 1, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
            Assert.AreEqual("No books checked out history for patron with that Id.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_ActionExecutes_ReturnsWaitListNullMessage()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns error message.");
            _waitListBooks = null;
            _bookDaoMock.Setup(books => books.GetWaitListBooks()).Returns(Task.FromResult(_waitListBooks));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, null, null, 0, 0, "waitList", null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
            Assert.AreEqual("There are no books on the waitlist.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_ActionExecutes_ReturnsOverdueBookNullMessage()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns error message.");
            _books = null;
            _bookDaoMock.Setup(books => books.GetOverdueBooks()).Returns(Task.FromResult(_books));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, null, null, 0, 0, null, "overdue", null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
            Assert.AreEqual("There are currently no overdue books.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBooksTest_ActionExecutes_ReturnsStatusNullMessage()
        {
            //Arrange 
            Console.WriteLine("Inside GetBooksTest returns error message.");
            _books = null;
            _bookDaoMock.Setup(books => books.GetListOfBooksByStatus(It.IsAny<string>())).Returns(Task.FromResult(_books));

            //Act
            var result = await _booksControllerMock.GetBook(0, null, null, null, 0, 0, null, null, "status");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
            Assert.AreEqual("There were no books found with that status.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBookTest_ActionExecutes_ThrowsException()
        {
            //Arrange
            Console.WriteLine("Inside GetBookTest throws exception.");
            _bookDaoMock.Setup(book => book.GetBookById(It.IsAny<int>())).Throws<Exception>();

            //Act 
            var result = await _booksControllerMock.GetBook(1, null, null, null, 0, 0, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdateBookByIdTest_UpdateBookRecord_ReturnsStatusCode200WhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdateBookByIdTest Returns 200");
            _bookDaoMock.Setup(arg => arg.GetBookById(It.IsAny<int>())).Returns(Task.FromResult(_bookModelMock));

            //Act
            var result = await _booksControllerMock.UpdateBookById(1201, "C# Player's Guide", "RB", "Whitaker", "Education", (decimal)12.00);
            Console.WriteLine(result.GetType());
            //Assert

            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("C# Player's Guide has been updated.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdateBookByIdTest_UpdateBookRecord_ReturnsMessageWhenBookIsNull()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdateBookByIdTest Returns message for null Book");
            _bookDaoMock.Setup(book => book.GetBookById(It.IsAny<int>())).Returns(Task.FromResult(_bookModelMockNull));

            //Act
            var result = await _booksControllerMock.UpdateBookById(1, "C# Player's Guide", "RB", "Whitaker", "Education", (decimal)12.00);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.IsNull(_bookModelMockNull);
            Assert.AreEqual("No book found with that Id!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdateBookByIdTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdateBookById throws exception");
            _bookDaoMock.Setup(arg => arg.GetBookById(It.IsAny<int>())).Returns(Task.FromResult(_bookModelMock));
            _bookDaoMock.Setup(updateBook => updateBook.UpdateBookById(It.IsAny<BookModel>())).Throws<Exception>();

            //Act
            var result = await _booksControllerMock.UpdateBookById(1, "C# Player's Guide", "RB", "Whitaker", "Education", (decimal)12.00);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task DeleteBookByIdTest_DeletesBookRecord_ReturnsCode200WhenSuccessful()
        {
            Console.WriteLine("Inside TestMethod DeleteBookByIdOk");
            //Arrange
            _bookDaoMock.Setup(book => book.GetBookById(It.IsAny<int>())).Returns(Task.FromResult(_bookModelMock));
            _bookDaoMock.Setup(deleteBook => deleteBook.DeleteBookById(It.IsAny<int>())).Returns(Task.FromResult(_bookModelMock));

            //Act
            var result = await _booksControllerMock.DeleteBookById(_bookModelMock.Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual("Wonder and Chaos of Being with Id 1200 has been deleted.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task DeleteBookByIdTest_DeleteBookRecord_ReturnsMessageWhenNull()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod DeleteBookByIdNull");
            _bookDaoMock.Setup(arg => arg.GetBookById(It.IsAny<int>())).Returns(Task.FromResult(_bookModelMockNull));

            //Act
            var result = await _booksControllerMock.DeleteBookById(_bookModelMock.Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.IsNull(_bookModelMockNull);
            Assert.AreEqual("No book found with that Id!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task DeleteBookByIdTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod DeleteBookById throws exception");
            _bookDaoMock.Setup(arg => arg.GetBookById(It.IsAny<int>())).Returns(Task.FromResult(_bookModelMock));
            _bookDaoMock.Setup(book => book.DeleteBookById(1200)).Throws<Exception>();

            //Act
            var result = await _booksControllerMock.DeleteBookById(_bookModelMock.Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdateBookTest_CheckOutBookExecutes_ReturnsOkWithObjectWhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside UpdateBookTest check out book returns success.");
            _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            _patronDaoMock.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMock));
            _bookDaoMock.Setup(booksOut => booksOut.GetTotalOfCheckedOutBooks(It.IsAny<int>())).Returns(Task.FromResult(0));

            //Act
            var result = await _booksControllerMock.UpdateBook(_bookModelMock.BookTitle, "email@patron.com", "Y", null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode );
            Assert.AreEqual("Wonder and Chaos of Being has been checked out.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task UpdateBookTest_ReturnBookExecutes_ReturnsOkWithObjectWhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside UpdateBookTest check out book returns success.");
            _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            _patronDaoMock.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMock));
            _bookDaoMock.Setup(booksOut => booksOut.GetTotalOfCheckedOutBooks(It.IsAny<int>())).Returns(Task.FromResult(0));
            _bookDaoMock.Setup(book => book.GetBookByTitleAndId(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(_bookModelMock));

            //Act
            var result = await _booksControllerMock.UpdateBook(_bookModelMock.BookTitle, _patronModelMock.Email, null, "Y", null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual("Wonder and Chaos of Being has been returned.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task UpdateBookTest_ReturnBookExecutes_ChecksOutBookToFirstPatronOnWaitList()
        {
            //Arrange
            Console.WriteLine("Inside UpdateBookTest check out book returns success.");
            _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            _patronDaoMock.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMock));
            _bookDaoMock.Setup(booksOut => booksOut.GetTotalOfCheckedOutBooks(It.IsAny<int>())).Returns(Task.FromResult(0));
            _bookDaoMock.Setup(book => book.GetBookByTitleAndId(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(_bookModelMock));
            _bookDaoMock.Setup(bookWait => bookWait.CheckForBookOnWaitList(It.IsAny<string>())).Returns(Task.FromResult(_waitListBooks));

            //Act
            var result = await _booksControllerMock.UpdateBook(_bookModelMock.BookTitle, _patronModelMock.Email, null, "Y", null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual("Wonder and Chaos of Being has been checked out to first eligible patron on waitlist.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task UpdateBookTest_ReturnBookExecutes_ReturnsBookWhenNoEligiblePatronOnWaitList()
        {
            //Arrange
            Console.WriteLine("Inside UpdateBookTest return book returns success.");
            _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            _patronDaoMock.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMock));
            _bookDaoMock.Setup(booksOut => booksOut.GetTotalOfCheckedOutBooks(It.IsAny<int>())).Returns(Task.FromResult(5));
            _bookDaoMock.Setup(book => book.GetBookByTitleAndId(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(_bookModelMock));
            _bookDaoMock.Setup(bookWait => bookWait.CheckForBookOnWaitList(It.IsAny<string>())).Returns(Task.FromResult(_waitListBooks));

            //Act
            var result = await _booksControllerMock.UpdateBook(_bookModelMock.BookTitle, _patronModelMock.Email, null, "Y", null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual("Wonder and Chaos of Being has been returned.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task UpdateBookTest_ReturnBookExecutes_ChecksOutBookToFirstEligiblePatronOnWaitList()
        {
            //Arrange
            Console.WriteLine("Inside UpdateBookTest return book checks out book to elibible patron on waitlist.");
            _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            _patronDaoMock.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMock));
            _bookDaoMock.Setup(booksOut => booksOut.GetTotalOfCheckedOutBooks(It.IsAny<int>())).Returns(Task.FromResult(5));
            _bookDaoMock.Setup(book => book.GetBookByTitleAndId(It.IsAny<string>(), It.IsAny<int>())).Returns(Task.FromResult(_bookModelMock));
            _bookDaoMock.Setup(bookWait => bookWait.CheckForBookOnWaitList(It.IsAny<string>())).Returns(Task.FromResult(_waitListBooks));
            var waitBook1 = _waitListBooks.ElementAt(0);
            _bookDaoMock.Setup(booksOut1 => booksOut1.GetTotalOfCheckedOutBooks(waitBook1.PatronId)).Returns(Task.FromResult(5));
            var waitBook2 = _waitListBooks.ElementAt(1);
            _bookDaoMock.Setup(booksOut2 => booksOut2.GetTotalOfCheckedOutBooks(waitBook2.PatronId)).Returns(Task.FromResult(2));

            //Act
            var result = await _booksControllerMock.UpdateBook(_bookModelMock.BookTitle, _patronModelMock.Email, null, "Y", null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual("Wonder and Chaos of Being has been checked out to first eligible patron on waitlist.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task UpdateBookTest_AddBookToWaitListExecutes_ReturnsOKSuccessMessage()
        {
            //Arrange
            Console.WriteLine("Inside UpdateBookTest check out book returns success.");
            _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            _patronDaoMock.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMock));
            _bookModelMock.Status = "Out";
            _bookDaoMock.Setup(waitBook => waitBook.BookWaitList(_patronModelMock.Id, _bookModelMock.BookTitle, _bookModelMock.AuthorFName, _bookModelMock.AuthorLName)).Returns(Task.FromResult(_bookModelMock));
            
            //Act
            var result = await _booksControllerMock.UpdateBook(_bookModelMock.BookTitle, _patronModelMock.Email, null, null, "Y");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual("Wonder and Chaos of Being has been added to waitlist.", (result as ObjectResult).Value);
        }
    }
}
