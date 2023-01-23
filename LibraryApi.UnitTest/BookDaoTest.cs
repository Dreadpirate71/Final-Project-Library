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
    public class BookDaoTest
    {
        [TestMethod]
        public void CallSqlWithString()
        {
            Mock<ISqlWrapper> mockSqlWrapper = new Mock<ISqlWrapper>();
            BookDao sut = new(mockSqlWrapper.Object);

            sut.GetBook();

            mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.Query<BookModel> (It.Is<string>(sql => sql == "SELECT * FROM Books")), Times.Once);

        }
    }
}
