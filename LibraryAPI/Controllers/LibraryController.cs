using System;
using System.Threading.Tasks;
using LibraryAPI.Daos;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;

namespace LibraryAPI.Controllers
{
    public class BooksController : ControllerBase
    {
        private readonly BookDao _bookDao;

        private readonly IBookDao bookDao;

        public BooksController(IBookDao bookDao)
        {
            this.bookDao = bookDao;
        }

        public BooksController(BookDao bookDao)
        {
            _bookDao = bookDao;
        }

        [HttpGet]
        [Route("Books")]
        public async Task<IActionResult> GetBooks()
        {
            try
            {
                var books = await _bookDao.GetBooks();
                return Ok(books);
            }
            catch (Exception e) 
            { 
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        [Route("Books")]
        public async Task<IActionResult> AddBook(string bookTitle, string authorFname, string authorLName, string type, decimal price, string Status, int patronId)
        {
            try
            {
                await _bookDao.AddBook(bookTitle, authorFname, authorLName, type, price, Status, patronId);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        public void CallDao()
        {
            bookDao.GetBooks();
        }
    }
}
