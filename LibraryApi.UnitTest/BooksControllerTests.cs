using LibraryAPI.Controllers;
using LibraryAPI.Daos;
using LibraryAPI.Models;
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
        private readonly BooksController _booksControllerMock;
        private BookModel _bookModelMock;

        public BooksControllerTests()
        {
            _bookDaoMock= new Mock<IBookDao>();
            _booksControllerMock = new BooksController(_bookDaoMock.Object);
            _bookModelMock = new BookModel();
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
        public void GetListOfAllBooksTest_ActionExecutes_ReturnsOkWithData()
        {
            Console.WriteLine("Inside TestMethod GetListOfAllBooksTest");
            var result = _booksControllerMock.GetListOfAllBooks();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AddBookTest_ActionExecutes_ReturnsOk()
        {
            Console.WriteLine("Inside TestMethod AddBookTest");
            var result = _booksControllerMock.AddBook("C# Player's Guide", "RB", "Whitaker", "Educational", (decimal)12.00, "In", "", 1001 );
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetBookByTitleTest_ActionExecutes_ReturnsOkWithData()
        {
            Console.WriteLine("Inside TestMethod GetBookByTitleTest");
            var result = _booksControllerMock.GetBookByTitle("New Moon");
            Assert.IsNotNull(result);            
        }

        [TestMethod]
        public void UpdateBookByTitleTest_ActionExecutes_ReturnsCode204WhenSuccessful()
        {
            Console.WriteLine("Inside TestMethod UpdateBookByTitleTest");
            var result = _booksControllerMock.UpdateBookByTitle(_bookModelMock);
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public void DeleteBookById_ActionExecutes_ReturnsCode200WhenSuccessful()
        {
            Console.WriteLine("Inside TestMethod DeleteBookById");
            var result = _booksControllerMock.DeleteBookById(_bookModelMock.Id);
            Assert.IsNotNull(result);
        }
    }
}
