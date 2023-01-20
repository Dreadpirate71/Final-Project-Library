using LibraryAPI.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace LibraryApi.UnitTest
{
    [TestClass]
    public class BookModelTest
    {
        [TestMethod]
        public void AddTitleTest()
        {
            Book sut = new Book();
            string expectedBook = "Awesome!";
            sut.AddTitleName(expectedBook);

            Assert.AreEqual(expectedBook, sut.titleName);
        }
        
        public void AddAuthorTest()
        {
            Book sut = new Book();
            string expectedAuthor = "AuthorFName";
            sut.AddAuthorNames(expectedAuthor);
            Assert.AreEqual(expectedAuthor, sut.authorFName);
        }
    }
}
   