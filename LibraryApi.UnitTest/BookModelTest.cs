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
            //Arrange
            BookAvailableModel sut = new();
            string expectedBook = "Awesome!";
            
            //Act
            Console.WriteLine(expectedBook);
            sut.AddTitleName(expectedBook);

            //Assert
            Assert.AreEqual(expectedBook, sut.BookTitle);
        }

        [TestMethod]
        public void AddAuthorTest()
        {
            BookAvailableModel sut = new();
            string expectedAuthor = "AuthorFName";
            sut.AddAuthorNames(expectedAuthor);
            Assert.AreEqual(expectedAuthor, sut.AuthorFName);
        }
    }
}
   