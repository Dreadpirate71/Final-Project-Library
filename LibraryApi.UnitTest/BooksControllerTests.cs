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
        public void CallDao()
        {
            Console.WriteLine("Inside TestMethod CallDao");
            _booksControllerMock.CallDao();
            _bookDaoMock.Verify(bookDao => bookDao.GetBook(), Times.Once);
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
        public async Task GetBookByTitleTest_ActionExecutes_ReturnsOkWithData()
        {
            Console.WriteLine("Inside TestMethod GetBookByTitleTest");
            var result = await _booksControllerMock.GetBookByTitle("New Moon");
            Assert.IsNotNull(result);  
            Assert.IsTrue(result is StatusCodeResult);
            Assert.IsInstanceOfType(result, result.GetType());
        }

        [TestMethod]
        public async Task UpdateBookByTitleTest_ActionExecutes_ReturnsCode204WhenSuccessful()
        {
            Console.WriteLine("Inside TestMethod UpdateBookByTitleTest");
            var result = await _booksControllerMock.UpdateBookByTitle(_bookModelMock);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is StatusCodeResult);
            Assert.IsInstanceOfType(result, result.GetType());
        }
        [TestMethod]
        public async Task DeleteBookById_ActionExecutes_ReturnsCode404WhenNull()
        {
            Console.WriteLine("Inside TestMethod DeleteBookById");
            var result = await _booksControllerMock.DeleteBookById(_bookModelMock.Id) as StatusCodeResult;
            Assert.IsNotNull(result);
            Assert.IsTrue(result is StatusCodeResult);
            Assert.AreEqual(result.StatusCode, StatusCodes.Status404NotFound);
        }
        [TestMethod]
        public async Task CheckOutBook_ActionExecutes_ReturnsCode200WhenSuccessful()
        {
            Console.WriteLine("Inside TestMethod CheckOutBook");
            var result = await _booksControllerMock.CheckOutBook(_bookModelMock.BookTitle, _patronModelMock.Email);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
        }
    }
}
