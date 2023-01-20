using LibraryAPI.Controllers;
using LibraryAPI.Daos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApi.UnitTest
{
    public class BookControllerTest
    {
        [TestMethod]
        public void CallDao()
        {
            Mock<IBookDao> mockBookDao = new Mock<IBookDao>();

            BooksController sut = new();
            sut.CallDao();
            mockBookDao.Verify(bookDao => bookDao.GetBooks(), Times.Once);
        }
    }
}
