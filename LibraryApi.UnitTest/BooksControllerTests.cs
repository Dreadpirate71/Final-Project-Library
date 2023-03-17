using LibraryAPI.Controllers;
using LibraryAPI.Daos;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NuGet.Frameworks;
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
        private readonly Mock<IStaffDao> _staffDaoMock;
        private readonly BooksController _booksControllerMock;
        private readonly PatronsController _patronsControllerMock;
        private readonly IEnumerable <BookModel>? _booksNull;
        private BookModel _bookModelMock;
        private StaffModel _staffModelMock;
        private PatronModel _patronModelMock;
        private IEnumerable<BookModel> _books;
        private BookModel? _bookModelMockNull;
        private PatronModel? _patronModelMockNull;
        private readonly IEnumerable<string> _genres;

        public BooksControllerTests()
        {
            _bookDaoMock= new Mock<IBookDao>();
            _patronDaoMock= new Mock<IPatronDao>();
            _staffDaoMock= new Mock<IStaffDao>();
            _booksControllerMock = new BooksController(_bookDaoMock.Object, _patronDaoMock.Object, _staffDaoMock.Object);
            _patronsControllerMock = new PatronsController(_patronDaoMock.Object);
            _bookModelMock = new BookModel() { Id = 1200, BookTitle = "Wonder and Chaos of Being", AuthorFName = "Kris", AuthorLName = "Remus", Genre = "Fiction", Price = (decimal)12.00, Status = "In", CheckOutDate = null, PatronId = 1111};
            _patronModelMock = new PatronModel();
            _staffModelMock = new StaffModel();
            _books = new List<BookModel>() { _bookModelMock};
            _genres = new List<string>() { "History", "Education", "Young Adult Fiction" };
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
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task GetListOfAllBooksTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetListOfAllBooks throws exception");
            _bookDaoMock.Setup(book => book.GetListOfAllBooks()).Throws<Exception>();

            //Act
            var result = await _booksControllerMock.GetListOfAllBooks();
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task AddBookTest_ActionExecutes_ReturnsOk()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod AddBookTest");
            _ = _staffDaoMock.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>())).Returns(Task.FromResult(true));

            //Act
            var result = await _booksControllerMock.AddBook(1, "C# Player's Guide", "RB", "Whitaker", "Educational", (decimal)12.00, "In", "", 1001 );
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is OkResult);
        }
        [TestMethod]
        public async Task AddBookTest_UpdateBookRecord_ReturnsMessageWhenNotAdmin()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod AddBookTest Returns message for not Admin");
            _ = _staffDaoMock.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>())).Returns(Task.FromResult(false));

            //Act
            var result = await _booksControllerMock.AddBook(1, "C# Player's Guide", "RB", "Whitaker", "Educational", (decimal)12.00, "In", "", 1001);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("You need to have an adminId to complete this task", (result as ObjectResult).Value);
        }
        [TestMethod]
        public async Task AddBookTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod AddBook throws exception");
            _ = _staffDaoMock.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>())).Returns(Task.FromResult(true));
            _bookDaoMock.Setup(book => book.AddBook("C# Player's Guide", "RB", "Whitaker", "Educational", (decimal)12.00, "In", "", 1001)).Throws<Exception>();

            //Act
            var result = await _booksControllerMock.AddBook(1, "C# Player's Guide", "RB", "Whitaker", "Educational", (decimal)12.00, "In", "", 1001);
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
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
           
            //Assert
            //Assert.IsNotNull(result);  
            Assert.IsTrue(result is ObjectResult);
            Assert.IsInstanceOfType(result, result.GetType());
            Assert.AreEqual("No book found with that title!", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBookByTitleTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetBookByTitle throws exception");
            _bookDaoMock.Setup(book => book.GetBookByTitle("Foo")).Throws<Exception>();

            //Act
            var result = await _booksControllerMock.GetBookByTitle("Foo");
            
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task UpdateBookByTitleTest_UpdateBookRecord_ReturnsStatusCode200WhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdateBookByTitleTest Returns 200");
            _ = _staffDaoMock.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>())).Returns(Task.FromResult(true));
            _ = _bookDaoMock.Setup(arg => arg.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));

            //Act
            var result = await _booksControllerMock.UpdateBookByTitle(1,_bookModelMock);
            Console.WriteLine(result.GetType());
            //Assert

            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Book has been updated!", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task UpdateBookByTitleTest_UpdateBookRecord_ReturnsMessageWhenNotAdmin()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdateBookByTitleTest Returns message for not Admin");
            _ = _staffDaoMock.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>())).Returns(Task.FromResult(false));
            _ = _bookDaoMock.Setup(arg => arg.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            //Act
            var result = await _booksControllerMock.UpdateBookByTitle(5, _bookModelMock);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("You need to have an adminId to complete this task", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task UpdateBookByTitleTest_UpdateBookRecord_ReturnsMessageWhenBookIsNull()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdateBookByTitleTest Returns message for null Book");
            _ = _staffDaoMock.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>())).Returns(Task.FromResult(true));

            //Act
            var result = await _booksControllerMock.UpdateBookByTitle(1,_bookModelMock);
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("No book found with that title!", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task UpdateBookByTitleTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdateBookByTitle throws exception");
            _ = _staffDaoMock.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>())).Returns(Task.FromResult(true));
            _ = _bookDaoMock.Setup(arg => arg.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            _bookDaoMock.Setup(book => book.UpdateBookByTitle(_bookModelMock)).Throws<Exception>();

            //Act
            var result = await _booksControllerMock.UpdateBookByTitle(5,_bookModelMock);
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task DeleteBookByIdTest_DeleteBookRecord_ReturnsMessageWhenNull()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod DeleteBookByIdNull");
            _ = _staffDaoMock.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>())).Returns(Task.FromResult(true));
            //Act
            var result = await _booksControllerMock.DeleteBookById(5, _bookModelMock.Id);
           
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("No book found with that Id!", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task DeleteBookByIdTest_DeleteBookRecord_ReturnsMessageWhenNotAdmin()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdateBookByTitleTest Returns message for not Admin");
            //_ = _staffDaoMock.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>())).Returns(Task.FromResult(_staffModelMock));

            //Act
            var result = await _booksControllerMock.DeleteBookById(5, _bookModelMock.Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("You need to have an adminId to complete this task", (result as ObjectResult).Value);
        }
        [TestMethod]
        public async Task DeleteBookByIdTest_DeletesBookRecord_ReturnsCode200WhenSuccessful()
        {
            Console.WriteLine("Inside TestMethod DeleteBookByIdOk");
            //Arrange
            _ = _staffDaoMock.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>())).Returns(Task.FromResult(true));
            _ = _bookDaoMock.Setup(arg => arg.DeleteBookById(It.IsAny<int>())).Returns(Task.FromResult<int>(1200));
            _ = _bookDaoMock.Setup(arg => arg.GetBookById(It.IsAny<int>())).Returns(Task.FromResult(_bookModelMock));
            Console.WriteLine(_bookModelMock.Id);
            //Act
            var result = await _booksControllerMock.DeleteBookById(5, _bookModelMock.Id);
           
            //Assert
            //Console.WriteLine(actual.ToString());
            Assert.IsNotNull(result);
            Assert.IsTrue(result is StatusCodeResult);
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.AreEqual(StatusCodes.Status200OK, (result as StatusCodeResult).StatusCode);
        }
        [TestMethod]
        public async Task DeleteBookByIdTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod DeleteBookById throws exception");
            _ = _staffDaoMock.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>())).Returns(Task.FromResult(true));
            _ = _bookDaoMock.Setup(arg => arg.GetBookById(It.IsAny<int>())).Returns(Task.FromResult(_bookModelMock));
            _bookDaoMock.Setup(book => book.DeleteBookById(1200)).Throws<Exception>();

            //Act
            var result = await _booksControllerMock.DeleteBookById(5, _bookModelMock.Id);
            
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
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
            
            //Assert
            Assert.IsNotNull(result);
            //Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as StatusCodeResult).StatusCode);
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
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Book with this title does not exist!", (result as ObjectResult).Value);
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
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Patron with this email does not exist!", (result as ObjectResult).Value);
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
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exceeded maximum of 5 books checked out! Please return a book to proceed.", (result as ObjectResult).Value);
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
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Book Status = 'Out'. Please choose a book that is not already checked out.", (result as ObjectResult).Value);
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
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
        }
        [TestMethod]
        public async Task GetListOfAllAvailableBooksTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetListOfAllAvailableBooks throws exception");
            _bookDaoMock.Setup(book => book.GetListOfAllAvailableBooks()).Throws<Exception>();

            //Act
            var result = await _booksControllerMock.GetListOfAllAvailableBooks();
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBookByGenreTest_RunsQuery_ReturnsOkWithData()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetBookByGenre returns OK.");
            _ = _bookDaoMock.Setup(book => book.GetBookByGenre(It.IsAny<string>())).Returns(Task.FromResult(_books));
            //Act
            var result = await _booksControllerMock.GetBookByGenre(_bookModelMock.Genre);
            //Assert
            Assert.IsNotNull (result);
            Assert.IsTrue(result is OkObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as OkObjectResult).StatusCode);
        }
        [TestMethod]
        public async Task GetBookByGenreTest_RunsQuery_ReturnMessageWhenNull()
        {
            //Arrange
            Console.WriteLine("Insinde TestMethod GetBookByGenre returns null message");
            
            _ = _bookDaoMock.Setup(book => book.GetBookByGenre(It.IsAny<string>())).Returns(Task.FromResult(_booksNull));
            //Act
            var result = await _booksControllerMock.GetBookByGenre(null);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("No books found with that Genre.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetBookByGenreTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetBookByGenre throws exception");
            _bookDaoMock.Setup(book => book.GetBookByGenre(It.IsAny<string>())).Throws<Exception>();

            //Act
            var result = await _booksControllerMock.GetBookByGenre("genre");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetListOfGenresTest_QueryExecuted_ReturnsOKResultWhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside GetListOfGenresTest when successful.");
            _bookDaoMock.Setup(genres => genres.GetListOfGenres()).Returns(Task.FromResult(_genres));

            //Act
            var result = await _booksControllerMock.GetListOfGenres();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is OkObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
        }
        [TestMethod]
        public async Task GetListOfGenresTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetListOfGenres throws exception");
            _bookDaoMock.Setup(genres => genres.GetListOfGenres()).Throws<Exception>();

            //Act
            var result = await _booksControllerMock.GetListOfGenres();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task ReturnBookTest_ActionExecuted_Returns200WhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside ReturnBookTest returns 200");
            _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            _patronDaoMock.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMock));
            //Act
            var result = await _booksControllerMock.ReturnBook("patronEmail", "bookTitle");
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is StatusCodeResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as StatusCodeResult).StatusCode);

        }

        [TestMethod]
        public async Task ReturnBookTest_ActionExecuted_ReturnsMessageWhenBookIsNull()
        {
            //Arrange
            Console.WriteLine("Inside ReturnBookTest returns null book message");
            _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMockNull));
            //Act
            var result = await _booksControllerMock.ReturnBook("patronEmail", "bookTitle");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("No book found with that title!", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task ReturnBookTest_ActionExecuted_ReturnsMessageWhenPatronIsNull()
        {
            //Arrange
            Console.WriteLine("Inside ReturnBookTest returns null patron message");
            _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            _patronDaoMock.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMockNull));
            //Act
            var result = await _booksControllerMock.ReturnBook("patronEmail", "bookTitle");
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Patron with that email does not exist!", (result as ObjectResult).Value);
        }

        public async Task ReturnBookTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod ReturnBook throws exception");
            _bookDaoMock.Setup(book => book.GetBookByTitle(It.IsAny<string>())).Returns(Task.FromResult(_bookModelMock));
            _patronDaoMock.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMock));
            _bookDaoMock.Setup(book => book.UpdateBookByTitle(It.IsAny<BookModel>())).Throws<Exception>();

            //Act
            var result = await _booksControllerMock.ReturnBook("patronEmail", "bookTitle");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
        }

    }
}
