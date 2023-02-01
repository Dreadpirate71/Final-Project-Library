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

        public BooksController(BookDao _bookDao)
        {
            this._bookDao = _bookDao;
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
    }
    public class BookController : ControllerBase
    {
        private readonly IBookDao bookDao;

        public BookController(IBookDao bookDao)
        {
            this.bookDao = bookDao;
        }
        public void CallDao()
        {
            bookDao.GetBooks();
        }
    }
    public class StaffController : ControllerBase
    {
        private readonly StaffDao _staffDao;

        public StaffController(StaffDao _staffDao)
        {
            this._staffDao = _staffDao;
        }


        [HttpPost]
        [Route("Staff")]
        public async Task<IActionResult> AddStaff(string FirstName, string LastName, string PhoneNumber, string Position)
        {
            try
            {
                await _staffDao.AddStaff(FirstName, LastName, PhoneNumber, Position);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpGet]
        [Route("Staff")]
        public async Task<IActionResult> GetStaff()
        {
            try
            {
                var staff = await _staffDao.GetStaff();
                return Ok(staff);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpDelete]
        [Route("Staff/{Id}")]
        public async Task<IActionResult> DeleteStaffById([FromRoute] int Id)
        {
            try
            {
                var staff = await _staffDao.GetStaffById(Id);
                if ( staff == null)
                {
                    return StatusCode(404);
                }
                await _staffDao.DeleteStaffById(Id);
                return StatusCode(200);

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpPatch]
        [Route("Staff/{Id}")]
        public async Task<IActionResult> UpdateStaffById([FromRoute]int Id, string FirstName, string LastName, string PhoneNumber, string Position)
        {
            try
            {
                await _staffDao.UpdateStaffById(Id, FirstName, LastName, PhoneNumber, Position);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }


    }
}
