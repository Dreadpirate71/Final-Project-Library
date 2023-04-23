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
        Task AddBook(string bookTitle, string authorFName, string authorLName, string genre, decimal price);
        Task UpdateBookById(BookModel bookModel);
        Task <BookModel> GetBookById(int id);
        Task <BookModel> GetBookByTitle(string title);
        Task DeleteBookById(int id);
        Task<int> GetTotalOfCheckedOutBooks(int patronId);
        Task<IEnumerable<BookModel>> GetListOfBooksCheckedOut(int patronId);
        Task<IEnumerable<BookModel>> GetBookByGenre(string bookGenre);
        Task<IEnumerable<BookModel>> GetListOfBooksByStatus(string status);
        Task<IEnumerable<string>> GetListOfGenres();
        Task <IEnumerable<BookModel>>GetOverdueBooks();
        Task <BookModel>GetBookByTitleAndId(string bookTitle, int patronId);
        Task BookWaitList(int patronId, string bookTitle, string authorFName, string authorLName);
        Task <IEnumerable<BookRequestModel>> GetWaitListBooks();
        Task <IEnumerable<BookRequestModel>> CheckForBookOnWaitList(string bookTitle);
        Task DeleteWaitListBook(int patronId, string bookTitle);
        Task BookRequestList(int patronId, string bookTitle, string authorFName, string authorLName);
        Task <IEnumerable<BookRequestModel>> GetRequestListBooks();
        Task <IEnumerable<BookModel>> GetBooksByAuthorLName(string authorLName);
        Task <IEnumerable<string>> GetBooksHistory(int patronId);
    }
}
