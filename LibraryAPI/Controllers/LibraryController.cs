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

        private IBookDao bookDao;

        public BooksController()
        {
        }

        public BooksController(BookDao bookDao)
        {
            _bookDao = bookDao;
        }

        [HttpPost]
        [Route("Books")]
        public async Task<IActionResult> AddBook(string titleName, string authorFname, string authorLName, string type, decimal price, string Status, int patronId)
        {
            try
            {
                await _bookDao.AddBook(titleName, authorFname, authorLName, type, price, Status, patronId);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        public void CallDao()
        {
            throw new NotImplementedException();
        }
    }
}
