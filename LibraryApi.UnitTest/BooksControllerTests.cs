using LibraryAPI.Controllers;
using LibraryAPI.Daos;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public BooksControllerTests()
        {
            _bookDaoMock= new Mock<IBookDao>();
            _patronDaoMock= new Mock<IPatronDao>();
            _booksControllerMock = new BooksController(_bookDaoMock.Object, _patronDaoMock.Object);
            _patronsControllerMock = new PatronsController(_patronDaoMock.Object);
            _bookModelMock = new BookModel();
            _patronModelMock = new PatronModel();
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
            Console.WriteLine("Inside TestMethod GetListOfAllBooksTest");
            var result = await _booksControllerMock.GetListOfAllBooks();
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
        }

        [TestMethod]
        public async Task AddBookTest_ActionExecutes_ReturnsOk()
        {
            Console.WriteLine("Inside TestMethod AddBookTest");
            var result = await _booksControllerMock.AddBook("C# Player's Guide", "RB", "Whitaker", "Educational", (decimal)12.00, "In", "", 1001 );
            Assert.IsNotNull(result);
            Assert.IsTrue(result is OkResult);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
        [TestMethod]
        public async Task GetBookByTitleTest_ActionExecutes_ReturnsBookObjectWhenOK()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetBookByTitleTest");
            _ = _bookDaoMock.Setup(x => x.GetBookByTitle("New Moon")).ReturnsAsync(new BookModel { BookTitle = "New Moon" });
            //Act
            var result = await _booksControllerMock.GetBookByTitle("New Moon") as OkObjectResult;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is OkObjectResult);
            //Assert.AreEqual(Ok, result);
        }
        [TestMethod]
        public async Task GetBookByTitleTest_ActionExecutes_ReturnsStatusCode404WhenNull()
        {
            Console.WriteLine("Inside TestMethod GetBookByTitleTest");
            //Act
            var result = await _booksControllerMock.GetBookByTitle("New Moon") as StatusCodeResult;
            //Assert
            Assert.IsNotNull(result);  
            Assert.IsTrue(result is StatusCodeResult);
            Assert.IsInstanceOfType(result, result.GetType());
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task UpdateBookByTitleTest_ActionExecutes_ReturnsObjectWhenSuccessful()
        {
            Console.WriteLine("Inside TestMethod UpdateBookByTitleTest");
            //Act
            var result = await _booksControllerMock.UpdateBookByTitle(_bookModelMock);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
        }

        [TestMethod]
        public async Task DeleteBookById_ActionExecutes_ReturnsCode404WhenNull()
        {
            Console.WriteLine("Inside TestMethod DeleteBookByIdNull");
            //Act
            var result = await _booksControllerMock.DeleteBookById(_bookModelMock.Id) as StatusCodeResult;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is StatusCodeResult);
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
        }
        [TestMethod]
        public async Task DeleteBookById_ActionExecutes_ReturnsCode200WhenSuccessful()
        {
            Console.WriteLine("Inside TestMethod DeleteBookByIdOk");
            //Arrange
            _ = _bookDaoMock.Setup(x => x.DeleteBookById(11));
            var result = await _booksControllerMock.DeleteBookById(11);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is StatusCodeResult);
            //Assert.IsInstanceOfType(result, typeof(OkResult));
            //Assert.AreEqual(StatusCodes.Status200OK, result);
        }
        [TestMethod]
        public async Task CheckOutBook_ActionExecutes_ReturnsObjectWhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod CheckOutBook");
            //Act
            var result = await _booksControllerMock.CheckOutBook(_bookModelMock.BookTitle, _patronModelMock.Email);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
        }
    }
}
