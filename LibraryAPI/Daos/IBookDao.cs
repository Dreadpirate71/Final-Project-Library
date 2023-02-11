using LibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryAPI.Daos
{
    public interface IBookDao
    {

        public void GetBook()
        {
        }
        Task<IEnumerable<BookModel>> GetListOfAllBooks();
        Task AddBook(string bookTitle, string authorFName, string authorLName, string genre, decimal price, string status, string checkOutDate, int patronId);
        Task UpdateBookByTitle(BookModel bookModel);
        Task <BookModel> GetBookById(int id);
        Task <BookModel> GetBookByTitle(string title);
        Task DeleteBookById(int id);
        Task<int> GetTotalOfCheckedOutBooks(int patronId);

    }
}
