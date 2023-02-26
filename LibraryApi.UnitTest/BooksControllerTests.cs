using LibraryAPI.Controllers;
using LibraryAPI.Daos;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        private readonly BooksController _booksControllerMock;
        private readonly PatronsController _patronsControllerMock;
        private BookModel _bookModelMock;
        private PatronModel _patronModelMock;
        private IEnumerable<BookModel> _books;
        private BookModel? _bookModelMockNull;
        private PatronModel? _patronModelMockNull;

        public BooksControllerTests()
        {
            _bookDaoMock= new Mock<IBookDao>();
            _patronDaoMock= new Mock<IPatronDao>();
            _booksControllerMock = new BooksController(_bookDaoMock.Object, _patronDaoMock.Object);
            _patronsControllerMock = new PatronsController(_patronDaoMock.Object);
            _bookModelMock = new BookModel() { Id = 1200, BookTitle = "Wonder and Chaos of Being", AuthorFName = "Kris", AuthorLName = "Remus", Genre = "Fiction", Price = (decimal)12.00, Status = "In", CheckOutDate = null, PatronId = 1111};
            _patronModelMock = new PatronModel();
            _books = new List<BookModel>() { _bookModelMock};
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
        public async Task GetListOfAllBooksTest_ActionExecutes_ReturnsOkWithData()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetListOfAllBooksTest"); 
            _ = _bookDaoMock.Setup(arg => arg.GetListOfAllBooks()).Returns(Task.FromResult(_books));

            //Act
            var result = await _booksControllerMock.GetListOfAllBooks();
            Console.WriteLine(result);
            var resultStatusCode = result as OkObjectResult;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is OkObjectResult);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, resultStatusCode.StatusCode);
        }
        [TestMethod]
        public async Task GetListOfAllBooksTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetListOfAllBooks throws exception");
            _bookDaoMock.Setup(book => book.GetListOfAllBooks()).Throws<Exception>();

            //Act
            var result = await _booksControllerMock.GetListOfAllBooks();
            var resultMessage = (result as ObjectResult).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", resultMessage);
        }

        [TestMethod]
        public async Task AddBookTest_ActionExecutes_ReturnsOk()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod AddBookTest");

            //Act
            var result = await _booksControllerMock.AddBook("C# Player's Guide", "RB", "Whitaker", "Educational", (decimal)12.00, "In", "", 1001 );
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is OkResult);
        }

        [TestMethod]
        public async Task GetBookByTitleTest_SelectBookByTitle_ReturnsBookObjectWhenOK()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetBookByTitleTest");
            _ = _bookDaoMock.Setup(x => x.GetBookByTitle("New Moon")).ReturnsAsync(new BookModel { BookTitle = "New Moon" });
            //Act
            var result = await _booksControllerMock.GetBookByTitle("New Moon") as OkObjectResult;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is OkObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }

        [TestMethod]
        public async Task GetBookByTitleTest_SelectBookByTitle_ReturnsMessageWhenNull()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetBookByTitleTest Null");
            //Act
            var result = await _booksControllerMock.GetBookByTitle("Foo");
            var resultMessage = (result as ObjectResult).Value;
            //Assert
            //Assert.IsNotNull(result);  
            Assert.IsTrue(result is ObjectResult);
            Assert.IsInstanceOfType(result, result.GetType());
            Assert.AreEqual("No book found with that title!", resultMessage);
        }
        
        [TestMethod]
        public async Task UpdateBookByTitleTest_UpdateBookRecord_ReturnsStatusCode200WhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdateBookByTitleTest Returns 200");
            _ = _bookDaoMock.Setup(arg => arg.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));

            //Act
            var result = await _booksControllerMock.UpdateBookByTitle(_bookModelMock);
            var resultMessage = (result as ObjectResult).Value;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Book has been updated!", resultMessage);
        }

        [TestMethod]
        public async Task UpdateBookByTitleTest_UpdateBookRecord_ReturnsMessageWhenBookIsNull()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdateBookByTitleTest Returns message for null Book");

            //Act
            var result = await _booksControllerMock.UpdateBookByTitle(_bookModelMock);
            var resultMessage = (result as ObjectResult).Value;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("No book found with that title!", resultMessage);
        }

        [TestMethod]
        public async Task DeleteBookById_DeleteBookRecord_ReturnsMessageWhenNull()
        {
            Console.WriteLine("Inside TestMethod DeleteBookByIdNull");
            //Act
            var result = await _booksControllerMock.DeleteBookById(_bookModelMock.Id);
            var actual = (result as ObjectResult).Value;    
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("No book found with that Id!", actual);
        }
        [TestMethod]
        public async Task DeleteBookById_DeletesBookRecord_ReturnsCode200WhenSuccessful()
        {
            Console.WriteLine("Inside TestMethod DeleteBookByIdOk");
            //Arrange
            _ = _bookDaoMock.Setup(arg => arg.DeleteBookById(It.IsAny<int>())).Returns(Task.FromResult<int>(1200));
            _ = _bookDaoMock.Setup(arg => arg.GetBookById(It.IsAny<int>())).Returns(Task.FromResult(_bookModelMock));
            Console.WriteLine(_bookModelMock.Id);
            //Act
            var result = await _booksControllerMock.DeleteBookById(_bookModelMock.Id);
            var actual = result as StatusCodeResult;
            //Assert
            //Console.WriteLine(actual.ToString());
            Assert.IsNotNull(result);
            Assert.IsTrue(result is StatusCodeResult);
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.AreEqual(StatusCodes.Status200OK, actual.StatusCode);
        }

        [TestMethod]
        public async Task CheckOutBook_BookUpdatedWithPatronIdAndStatusOut_ReturnsCode200WhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod CheckOutBook Returns 200");
            _ = _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            _ = _patronDaoMock.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMock)); 
            //Act
            var result = await _booksControllerMock.CheckOutBook(_bookModelMock.BookTitle, _patronModelMock.Email);
            var resultStatusCode = result as StatusCodeResult;
            Console.WriteLine(resultStatusCode);
            //Assert
            Assert.IsNotNull(result);
            //Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, resultStatusCode.StatusCode);
        }

        [TestMethod]
        public async Task CheckOutBook_BookIsNull_ReturnsCode404WithMessage()
        {
            //Arrange
            _bookModelMockNull = null;
            Console.WriteLine("Inside TestMethod CheckOutBook Returns 404 with Message");
            _ = _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMockNull));
            _ = _patronDaoMock.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMock));

            //Act
            var result = await _booksControllerMock.CheckOutBook(_bookModelMock.BookTitle, _patronModelMock.Email);
            var resultMessage = (result as ObjectResult).Value;
            //Console.WriteLine(resultMessage.ToString());

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Book with this title does not exist!", resultMessage);
        }

        [TestMethod]
        public async Task CheckOutBook_PatronIsNull_ReturnsCode404WithMessage()
        {
            //Arrange
            _patronModelMockNull = null;
            Console.WriteLine("Inside TestMethod CheckOutBook Returns 404 with Message");
            _ = _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            _ = _patronDaoMock.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMockNull));

            //Act
            var result = await _booksControllerMock.CheckOutBook(_bookModelMock.BookTitle, _patronModelMock.Email);
            var resultMessage = (result as ObjectResult).Value;
            //Console.WriteLine(resultMessage.ToString());

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Patron with this email does not exist!", resultMessage);
        }

        [TestMethod]
        public async Task CheckOutBook_BooksOutExceeds5_ReturnsErrorMessage()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod CheckOutBook Returns Exceeds message");
            _ = _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            _ = _patronDaoMock.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMock));
            _ = _bookDaoMock.Setup(books => books.GetTotalOfCheckedOutBooks(It.IsAny<int>())).Returns(Task.FromResult(6));
            //Act
            var result = await _booksControllerMock.CheckOutBook(_bookModelMock.BookTitle, _patronModelMock.Email);
            var resultMessage = (result as ObjectResult).Value;
            Console.WriteLine(resultMessage.ToString());
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exceeded maximum of 5 books checked out! Please return a book to proceed.", resultMessage);
        }

        [TestMethod]
        public async Task CheckOutBook_BookStatusIsOut_ReturnsErrorMessage()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod CheckOutBook Returns Out message");
            _ = _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            _ = _patronDaoMock.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMock));
            _ = _bookDaoMock.Setup(books => books.GetTotalOfCheckedOutBooks(It.IsAny<int>())).Returns(Task.FromResult(1));
            _ = _bookModelMock.Status = "Out";
            //Act
            var result = await _booksControllerMock.CheckOutBook(_bookModelMock.BookTitle, _patronModelMock.Email);
            var resultMessage = (result as ObjectResult).Value;
            Console.WriteLine(resultMessage.ToString());
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Book Status = 'Out'. Please choose a book that is not already checked out.", resultMessage);
        }
        
        [TestMethod]
        public async Task GetListOfAllAvailableBooks_CreatesIEnumerable_ReturnsOkWithData()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetListOfAllBooks Returns Ok Object");
            _ = _bookDaoMock.Setup(books => books.GetListOfAllAvailableBooks()).Returns(Task.FromResult(_books));

            //Act
            var result = await _booksControllerMock.GetListOfAllAvailableBooks();
            Console.WriteLine(result);
            var resultStatusCode = result as OkObjectResult;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is OkObjectResult);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, resultStatusCode.StatusCode);
        }
    }
}
