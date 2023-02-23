using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Azure.Messaging;
using LibraryAPI.Daos;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;


namespace LibraryAPI.Controllers
{
    public class BooksController : ControllerBase
    {
        private readonly IBookDao _bookDao;
        private readonly IPatronDao _patronDao;

        public BooksController(IBookDao bookDao, IPatronDao patronDao)
        {
            this._bookDao = bookDao;
            this._patronDao = patronDao;
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
                    return StatusCode(404, "No book found with that Title!");
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
                    return StatusCode(404, "No book found with that Id!");
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
        public async Task<IActionResult> AddBook(string bookTitle, string authorFname, string authorLName, string genre, decimal price, string status, string checkOutDate, int patronId)
        {
            try
            {
                await _bookDao.AddBook(bookTitle, authorFname, authorLName, genre, price, status, checkOutDate, patronId);
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpPatch]
        [Route("Books/{Title}")]
        public async Task<IActionResult> UpdateBookByTitle([FromBody] BookModel updateRequest, int AdminId)
        {
            try
            {
                var adminCheck = await _bookDao.CheckStaffForAdmin(AdminId);
                if (adminCheck == null)
                {
                    return StatusCode(404, "Not an admin ID");
                }
                var book = await _bookDao.GetBookByTitle(updateRequest.BookTitle);
                if (book == null)
                {
                    return StatusCode(404, "No book found with that title!");
                }
                else
                {
                    await _bookDao.UpdateBookByTitle(updateRequest);
                    return StatusCode(204, "Book has been updated!");
                }

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
                    return StatusCode(404, "No book found with that Id!");
                }
                await _bookDao.DeleteBookById(book.Id);
                return StatusCode(200);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpPatch]
        [Route("CheckOutBook/{bookTitle}, {patronEmail}")]
        public async Task <IActionResult> CheckOutBook([FromRoute] string bookTitle, [FromRoute] string patronEmail)
        {
             
            try
            {
                var book = await _bookDao.GetBookByTitle(bookTitle);
                var patron = await _patronDao.GetPatronByEmail(patronEmail);
                if (book == null) 
                {
                    return StatusCode(404, "Book with this title does not exist");
                } 
                if (patron == null)
                {
                    return StatusCode(404, "Patron with this email does not exist!");
                }
                var patronBooksOut = await _bookDao.GetTotalOfCheckedOutBooks(patron.Id);
                if (patronBooksOut >= 5)
                {
                    return StatusCode(500, "Exceeded maximum of 5 books checked out! Please return a book to proceed.");
                }
                else if (book.Status == "Out")
                {
                    return StatusCode(500, "Book Status = 'Out'. Please choose a book that is not already checked out. ");
                }
                book.Status = "Out";
                book.PatronId = patron.Id;
                book.CheckOutDate = DateTime.Now.ToString("MM/dd/yyyy");
                await _bookDao.UpdateBookByTitle(book);
                return StatusCode(200);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            } 
        }
        [HttpGet]
        [Route("BooksCheckedOut/{patronEmail}")]
        public async Task<IActionResult> GetBooksCheckedOutByPatron([FromRoute] string patronEmail)
        {
            try
            {
                var patron = await _patronDao.GetPatronByEmail(patronEmail);
                if (patron == null)
                {
                    return StatusCode(404, "Patron with that email does not exist!");
                }
                var patronBooksOut = await _bookDao.GetListOfBooksCheckedOut(patron.Id);
                if (patronBooksOut == null)
                {
                    return StatusCode(404, "Patron does not have any books currently checked out!");
                }
                return Ok(patronBooksOut);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpGet]
        [Route("BookByGenre/{Genre}")]
        public async Task<IActionResult> GetBookByGenre([FromRoute] string Genre)
        {
            try
            {
                var book = await _bookDao.GetBookByGenre(Genre);
                if (book == null)
                {
                    return StatusCode(404, "No books found with that Genre.");
                }
                return Ok(book);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
    public class PatronsController : ControllerBase
    {
        private readonly IPatronDao _patronDao;
        
        public PatronsController(IPatronDao patronDao)
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
        [HttpGet]
        [Route("PatronById/{Id}")]
        public async Task<IActionResult> GetPatronById([FromRoute] int Id)
        {
            try
            {
                var patron = await _patronDao.GetPatronById(Id);
                if (patron == null)
                {
                    return StatusCode(404, "Patron with that Id number does not exist!");
                }
                return Ok(patron);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpGet]
        [Route("PatronByEmail/{Email}")]
        public async Task<IActionResult> GetPatronByEmail([FromRoute] string Email)
        {
            try
            {
                var patron = await _patronDao.GetPatronByEmail(Email);
                if (patron == null)
                {
                    return StatusCode(404, "Patron with that email does not exist!");
                }
                return Ok(patron);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpPost]
        [Route("Patrons")]
        public async Task<IActionResult> AddPatron(string firstName, string lastName, string email, string streetAddress, string city, string state, string postalCode, string phoneNumber)
        {
            try
            {
                await _patronDao.AddPatron(firstName, lastName, email, streetAddress, city, state, postalCode, phoneNumber);
                return Ok();
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
        }
        
        [HttpPatch]
        [Route("Patrons/{Email}")]
        public async Task<IActionResult> UpdatePatronByEmail([FromBody] PatronModel updateRequest)
        {
            try
            {
                var patron = await _patronDao.GetPatronByEmail(updateRequest.Email);
                if (patron == null)
                {
                    return StatusCode(404, "Patron with that email does not exist!");
                }
                await _patronDao.UpdatePatronByEmail(updateRequest);
                return Ok(patron);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpDelete]
        [Route("Patrons/{Id}")]
        public async Task<IActionResult>DeletePatronById([FromRoute] int Id)
        {
            try
            {
                var patron = await _patronDao.GetPatronById(Id);
                if (patron == null)
                {
                    return StatusCode(404, "Patron with that Id does not exist!");
                }
                await _patronDao.DeletePatronById(patron.Id);
                return StatusCode(200);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
        }

    }
    public class StaffController : ControllerBase
    {
        public readonly StaffDao _staffDao;

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
        public async Task<IActionResult> DeleteStaffById([FromRoute] int Id, int AdminId)
        {
            try
            {
                var staff = await _staffDao.GetStaffById(Id);
                var adminCheck = await _staffDao.CheckStaffForAdmin(AdminId);
                if (staff == null ) 
                {
                    return StatusCode(404);
                }
                if (adminCheck == null)
                {
                    return StatusCode(404, "Not an admin ID");
                }
                else
                {
                    await _staffDao.DeleteStaffById(Id);
                    return StatusCode(200);
                }


            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }


        [HttpPatch]
        [Route("Staff/{Id}")]
        public async Task<IActionResult> UpdateStaffById([FromRoute]int Id, string FirstName, string LastName, string PhoneNumber, string Position, int AdminId)
        {
            try
            {

                var adminCheck = await _staffDao.CheckStaffForAdmin(AdminId);
                if (adminCheck == null)
                {
                    return StatusCode(404, "Not an admin ID");
                }
                else
                {
                    await _staffDao.UpdateStaffById(Id, FirstName, LastName, PhoneNumber, Position);
                    return Ok();
                }

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
 

    }
}
