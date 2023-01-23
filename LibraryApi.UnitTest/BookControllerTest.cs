using LibraryAPI.Controllers;
using LibraryAPI.Daos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
    public class DeInitialize
    {
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Console.WriteLine("Inside AssemblyCleanup");
        }
    }
    [TestClass]
    public class BookControllerTest
    {
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
            Console.WriteLine("Inside TestMethod BookControllerTest");
            Mock<IBookDao> mockBookDao = new Mock<IBookDao>();

            BookController sut = new BookController(mockBookDao.Object);
            sut.CallDao();
            mockBookDao.Verify(bookDao => bookDao.GetBooks(), Times.Once);
        }
    }
}
