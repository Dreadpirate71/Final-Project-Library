using System;
using System.Globalization;
using System.Threading.Tasks;
using LibraryAPI.Daos;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.OData.Edm;

namespace LibraryAPI.Controllers
{
    public class BooksController : ControllerBase
    {
        private readonly BookDao _bookDao;
        private readonly IBookDao _interfaceBookDao;

        public BooksController(IBookDao interfaceBookDao)
        {
            this._interfaceBookDao = interfaceBookDao;
        }
        public BooksController(BookDao bookDao)
        {
            _bookDao = bookDao;
        }

        [HttpGet]
        [Route("Books")]
        public async Task<IActionResult> GetListOfAllBooks()
        {
            try
            {
                var books = await _bookDao.GetListOfAllBooks();
                return Ok(books);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpGet]
        [Route("BookByTitle/{Title}")]
        public async Task<IActionResult> GetBookByTitle([FromRoute] string Title)
        {
            try
            {
                var book = await _bookDao.GetBookByTitle(Title);
                if (book == null)
                {
                    return StatusCode(404);
                }
                return Ok(book);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpGet]
        [Route("BookById/{Id}")]
        public async Task<IActionResult> GetBookById([FromRoute] int Id)
        {
            try
            {
                var book = await _bookDao.GetBookById(Id);
                if (book == null)
                {
                    return StatusCode(404);
                }
                return Ok(book);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpPost]
        [Route("Books")]
        public async Task<IActionResult> AddBook(string bookTitle, string authorFname, string authorLName, string genre, decimal price, string status)
        {
            try
            {
                await _bookDao.AddBook(bookTitle, authorFname, authorLName, genre, price, status);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpPatch]
        [Route("Books/{Title}")]
        public async Task<IActionResult> UpdateBookByTitle([FromBody] BookAvailableModel updateRequest)
        {
            try
            {
                var book = await _bookDao.GetBookByTitle(updateRequest.BookTitle);
                if (book == null)
                {
                    return StatusCode(404);
                }
                await _bookDao.UpdateBookByTitle(updateRequest);
                return StatusCode(204);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpDelete]
        [Route("DeleteBook/{Id}")]
        public async Task<IActionResult>DeleteBookById([FromRoute] int Id)
        {
            try
            {
                var book = await _bookDao.GetBookById(Id);
                if (book == null)
                {
                    return StatusCode(404);
                }
                await _bookDao.DeleteBookById(Id);
                return StatusCode(200);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        public void CallDao()
        {
            _interfaceBookDao.GetBook();
        }
    }
    public class PatronsController : ControllerBase
    {
        private readonly IPatronDao _iFacePatronDao;
        private readonly PatronDao _patronDao;

        /*public PatronsController(IPatronDao iFacePatronDao)
        {
            this._iFacePatronDao= iFacePatronDao;
        }*/
        public PatronsController(PatronDao patronDao)
        {
            this._patronDao= patronDao;
        }
        [HttpGet]
        [Route("Patrons")]
        public async Task<IActionResult> GetListOfAllPatrons()
        {
            try
            {
                var patrons = await _patronDao.GetListOfAllPatrons();
                return(Ok(patrons));
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
