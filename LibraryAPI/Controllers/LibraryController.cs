using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
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
        /*public enum GenreEnum
        {
            ChildrensFiction,
            Architecture,
            Autobiography,
            Biography,
            Drama,
            Education,
            FairyTales,
            History
        }
        private readonly GenreEnum _genreEnum;*/
        private readonly IBookDao _bookDao;
        private readonly IPatronDao _patronDao;
        private readonly IStaffDao _staffDao;

        public BooksController(IBookDao bookDao, IPatronDao patronDao, IStaffDao staffDao)
        {
            this._bookDao = bookDao;
            this._patronDao = patronDao;
            this._staffDao = staffDao;
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
        [Route("Books/Available")]
        public async Task<IActionResult> GetListOfAllAvailableBooks()
        {
            try
            {
                var booksAvailable = await _bookDao.GetListOfAllAvailableBooks();
                return Ok(booksAvailable);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpGet]
        [Route("BookByTitle/{title}")]
        public async Task<IActionResult> GetBookByTitle([FromRoute] string title)
        {
            try
            {
                var book = await _bookDao.GetBookByTitle(title);
                if (book == null)
                {
                    return StatusCode(404, "No book found with that title!");
                }
                return Ok(book);
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpGet]
        [Route("BookById/{id}")]
        public async Task<IActionResult> GetBookById([FromRoute] int id)
        {
            try
            {
                var book = await _bookDao.GetBookById(id);
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

        [HttpGet]
        [Route("OverdueBooks")]
        public async Task<IActionResult> GetOverdueBooks()
        {
            try
            {
                var book = await _bookDao.GetOverdueBooks();
                if (book == null)
                {
                    return StatusCode(404, "No overdue books found");
                }
                return Ok(book);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet]
        [Route("BooksCheckedOut/{patronEmail}, {patronPassword}")]
        public async Task<IActionResult> GetBooksCheckedOutByPatron([FromRoute] string patronEmail, [FromRoute]string patronPassword)
        {
            try
            {
                var checkCredentials = await _patronDao.CheckPatronCredentials(patronEmail, patronPassword);
                if (checkCredentials == false)
                {
                    return StatusCode(404, "Patron with that email and password does not exist!");
                }
                else
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
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet]
        [Route("ListOfGenres")]
        public async Task<IActionResult> GetListOfGenres()
        {
            try
            {
                var genres = await _bookDao.GetListOfGenres();
                return Ok(genres);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet]
        [Route("BookByGenre/{genre}")]

        public async Task<IActionResult> GetBookByGenre([FromRoute] string genre)
        {
            try
            {
                var book = await _bookDao.GetBookByGenre(genre);
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

        public IBookDao Get_bookDao()
        {
            return _bookDao;
        }

        [HttpGet]
        [Route("NewYorkTimesBestSellers")]
        public async Task<IActionResult> GetNYTBestSellers()
        {
            var bookModel = new BookModel();
            await Task.Run(() => { bookModel.OpenUrl("https://www.nytimes.com/books/best-sellers/"); } );
            return Ok();
           
        }
        [HttpPost]
        [Route("NewBook/{adminId}, {adminPassword}")]
        public async Task<IActionResult> AddBook([FromRoute] int adminId, [FromRoute] string adminPassword, string bookTitle, string authorFname, string authorLName, string genre, decimal price)
        {
            try
            {
                var adminCheck = await _staffDao.CheckStaffForAdmin(adminId, adminPassword);
                if (adminCheck == false)
                {
                    return StatusCode(404, "You need to have an adminId to complete this task");
                }
                else
                {
                    await _bookDao.AddBook(bookTitle, authorFname, authorLName, genre, price);
                    return Ok();
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpPatch]
        [Route("UpdateBook/{adminId}, {adminPassword}, {bookId}")]
        public async Task<IActionResult> UpdateBookById([FromRoute] int adminId, [FromRoute] string adminPassword, [FromRoute] int bookId, string updateBookTitle, string updateAuthorFName, string updateAuthorLName, string updateGenre, decimal updatePrice)
        {
            var updateBook = new BookModel();
            
            try
            {
                var adminCheck = await _staffDao.CheckStaffForAdmin(adminId, adminPassword);
                if (adminCheck == false)
                {
                    return StatusCode(404, "You need to have an adminId to complete this task");
                }
                else
                {
                    var book = await _bookDao.GetBookById(bookId);
                    if (book == null)
                    {
                        return StatusCode(404, "No book found with that Id!");
                    }
                    else
                    {
                        
                        updateBook.Id = book.Id;
                        if (updateBookTitle == null)
                        { updateBook.BookTitle = book.BookTitle; }
                        else
                        { updateBook.BookTitle = updateBookTitle; }

                        if (updateAuthorFName == null)
                        {updateBook.AuthorFName = book.AuthorFName;}
                        else
                        {updateBook.AuthorFName = updateAuthorFName;}

                        if (updateAuthorLName == null)
                        {updateBook.AuthorLName = book.AuthorLName;}
                        else
                        {updateBook.AuthorLName = updateAuthorLName;}

                        if (updateGenre == null)
                        {updateBook.Genre = book.Genre;}
                        else
                        { updateBook.Genre = updateGenre; }    
                    
                        if (updatePrice.Equals(null))
                        { updateBook.Price = book.Price; }
                        else
                        { updateBook.Price = updatePrice; }

                        updateBook.Status = book.Status;
                        updateBook.CheckOutDate = book.CheckOutDate;
                        updateBook.PatronId = book.PatronId;  
                        
                    }
                    await _bookDao.UpdateBookById(updateBook);
                    return StatusCode(200, "Book has been updated!");
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        
        [HttpPatch]
        [Route("CheckOutBook/{bookTitle}, {patronEmail}, {patronPassword}")]
        public async Task <IActionResult> CheckOutBook([FromRoute] string bookTitle, [FromRoute] string patronEmail, [FromRoute]string patronPassword)
        {
             
            try
            {
                var checkCredentials = await _patronDao.CheckPatronCredentials(patronEmail, patronPassword);
                if (checkCredentials == false)
                {
                    return StatusCode(404, "Patron with that email and password does not exist!");
                }
                else
                {
                    var book = await _bookDao.GetBookByTitle(bookTitle);
                    var patron = await _patronDao.GetPatronByEmail(patronEmail);
                    if (book == null)
                    {
                        return StatusCode(404, "Book with this title does not exist!");
                    }
                    
                    var patronBooksOut = await _bookDao.GetTotalOfCheckedOutBooks(patron.Id);
                    if (patronBooksOut >= 5)
                    {
                        return StatusCode(500, "Exceeded maximum of 5 books checked out! Please return a book to proceed.");
                    }
                    else if (book.Status == "Out")
                    {
                        return StatusCode(500, "Book Status = 'Out'. Please choose a book that is not already checked out.");
                    }
                    book.Status = "Out";
                    book.PatronId = patron.Id;
                    var checkOutDate = DateTime.Now;
                    book.CheckOutDate = checkOutDate;
                    await _bookDao.UpdateBookById(book);
                    return StatusCode(200);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            } 
        }
        [HttpPatch]
        [Route("ReturnBook/{patronEmail}, {patronPassword}, {bookTitle}")]

        public async Task<IActionResult> ReturnBook([FromRoute] string patronEmail, [FromRoute]string patronPassword, [FromRoute] string bookTitle)
        {
            try
            {
                var checkCredentials = await _patronDao.CheckPatronCredentials(patronEmail, patronPassword);
                if (checkCredentials == false)
                {
                    return StatusCode(404, "Patron with that email and password does not exist!");
                }
                else
                {
                    var patron = await _patronDao.GetPatronByEmail(patronEmail);
                    var book = await _bookDao.GetBookByTitleAndId(bookTitle, patron.Id);
                    if (book == null)
                    {
                        return StatusCode(404, "No book checked out with that title!");
                    }

                    book.PatronId = 1003;
                    book.Status = "In";
                    await _bookDao.UpdateBookById(book);
                    return StatusCode(200, "Book has been returned.");
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete]
        [Route("Book/{adminId}, {adminPassword}, {bookId}")]
        public async Task<IActionResult> DeleteBookById([FromRoute] int adminId, [FromRoute] string adminPassword, [FromRoute] int bookId)
        {
            try
            {
                var adminCheck = await _staffDao.CheckStaffForAdmin(adminId, adminPassword);
                if (adminCheck == false)
                {
                    return StatusCode(404, "You need to have an adminId to complete this task");
                }
                else
                {
                    var book = await _bookDao.GetBookById(bookId);
                    if (book == null)
                    {
                        return StatusCode(404, "No book found with that Id!");
                    }
                    await _bookDao.DeleteBookById(book.Id);
                    return StatusCode(200);
                }
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
        private readonly IStaffDao _staffDao;
        
        public PatronsController(IPatronDao patronDao, IStaffDao staffDao)
        {
            this._patronDao= patronDao;
            this._staffDao= staffDao;
        }
        
        [HttpGet]
        [Route("Patrons/{adminId}, {adminPassword}")]
        public async Task<IActionResult> GetListOfAllPatrons([FromRoute]int adminId, [FromRoute]string adminPassword)
        {
            try
            {
                var adminCheck = await _staffDao.CheckStaffForAdmin(adminId, adminPassword);
                if (adminCheck == false)
                {
                    return StatusCode(404, "You do not have proper admin credentials!");
                }
                else
                {
                    var patrons = await _patronDao.GetListOfAllPatrons();
                    return (Ok(patrons));
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet]
        [Route("PatronById/{adminId}, {adminPassword}, {patronId}")]
        public async Task<IActionResult> GetPatronById([FromRoute]int adminId, [FromRoute]string adminPassword, [FromRoute] int patronId)
        {
            try
            {
                var adminCheck = await _staffDao.CheckStaffForAdmin(adminId, adminPassword);
                if (adminCheck == false)
                {
                    return StatusCode(404, "You do not have proper admin credentials!");
                }
                else
                {
                    var patron = await _patronDao.GetPatronById(patronId);
                    if (patron == null)
                    {
                        return StatusCode(404, "Patron with that Id number does not exist!");
                    }
                    return Ok(patron);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpGet]
        [Route("PatronByEmail/{patronEmail}, {patronPassword}")]
        public async Task<IActionResult> GetPatronByEmail([FromRoute] string patronEmail, [FromRoute]string patronPassword)
        {
            try
            {
                var checkCredentials = await _patronDao.CheckPatronCredentials(patronEmail, patronPassword);
                if (checkCredentials == false)
                {
                    return StatusCode(404, "Patron with that email and password does not exist!");
                }
                else
                {
                    var patron = await _patronDao.GetPatronByEmail(patronEmail);
                    if (patron == null)
                    {
                        return StatusCode(404, "Patron with that email does not exist!");
                    }
                    return Ok(patron);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        [Route("NewPatron")]
        public async Task<IActionResult> AddPatron(string firstName, string lastName, string email, string streetAddress, string city, string state, string postalCode, string phoneNumber, string password, string confirmPassword)
        {
            try
            {
                var newPatron = new PatronModel();
                var emailUnique = await _patronDao.CheckEmailUnique(email);
                if (emailUnique == true)
                {
                    if (password == confirmPassword)
                    {
                        newPatron.FirstName = firstName;
                        newPatron.LastName = lastName;
                        newPatron.Email = email;
                        newPatron.StreetAddress = streetAddress;
                        newPatron.City = city;
                        newPatron.State = state;
                        newPatron.PostalCode = postalCode;
                        newPatron.PhoneNumber = phoneNumber;
                        newPatron.Password = password;
                        newPatron.AccountLock = "No";
                        await _patronDao.AddPatron(newPatron);
                        return Ok("New Patron created.");
                    }
                    else { return StatusCode(400, "Passwords entered do not match!"); }
                }
                else
                {return StatusCode(400, "That email is already in use. Please use a different email.");}
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
            
        }
        
        [HttpPatch]
        [Route("UpdatePatron/{email}, {password}")]
        public async Task<IActionResult> UpdatePatronByEmail([FromRoute]string email, [FromRoute] string password,string updateFirstName, string updateLastName, 
                                                                string updateEmail, string updateStreetAddress, string updateCity, string updateState, string updatePostalCode, 
                                                                string updatePhoneNumber, string updatePassword, string updateConfirmPassword)
        {
            var updatePatron = new PatronModel();
            try
            {
                var checkCredentials = await _patronDao.CheckPatronCredentials(email, password);
                if (checkCredentials == false)
                {
                    return StatusCode(404, "Patron with that email and password does not exist!");
                }
                else
                {
                    var patron = await _patronDao.GetPatronByEmail(email);

                    updatePatron.Id = patron.Id;

                    if (updateFirstName == null)
                    { updatePatron.FirstName = patron.FirstName; }
                    else
                    { updatePatron.FirstName = updateFirstName; }

                    if (updateLastName == null)
                    { updatePatron.LastName = patron.LastName; }
                    else
                    { updatePatron.LastName = updateLastName; }

                    if (updateEmail == null)
                    { updatePatron.Email = patron.Email; }
                    else
                    {
                        var emailUnique = await _patronDao.CheckEmailUnique(updateEmail);
                        if (emailUnique == true)
                        {
                            updatePatron.Email = updateEmail;
                        }
                        else
                        {
                            return StatusCode(400, "That email is already in use. Please use a different email.");
                        }
                    }
                    if (updateStreetAddress == null)
                    { updatePatron.StreetAddress = patron.StreetAddress; }
                    else
                    { updatePatron.StreetAddress = updateStreetAddress; }

                    if (updateCity == null)
                    { updatePatron.City = patron.City; }
                    else
                    { updatePatron.City = updateCity; }

                    if (updateState == null)
                    { updatePatron.State = patron.State; }
                    else
                    { updatePatron.State = updateState; }

                    if (updatePostalCode == null)
                    { updatePatron.PostalCode = patron.PostalCode; }
                    else
                    { updatePatron.PostalCode = updatePostalCode; }

                    if (updatePhoneNumber == null)
                    { updatePatron.PhoneNumber = patron.PhoneNumber; }
                    else
                    { updatePatron.PhoneNumber = updatePhoneNumber; }
                    if (updatePassword == null)
                    { updatePatron.Password = patron.Password; }
                    else
                    {
                        if (updatePassword == updateConfirmPassword)
                        { updatePatron.Password = updatePassword; }
                        else
                        { return StatusCode(400, "Passwords entered do not match!"); }
                    }
                    updatePatron.AccountLock = patron.AccountLock;

                    await _patronDao.UpdatePatronById(updatePatron);
                    return Ok("Patron has been updated!");
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete]
        [Route("Patron/{adminId}, {adminPassword}, {patronId}")]
        public async Task<IActionResult>DeletePatronById([FromRoute] int adminId, [FromRoute] string adminPassword, [FromRoute] int patronId)
        {
            try
            {
                var patron = await _patronDao.GetPatronById(patronId);
                var adminCheck = await _staffDao.CheckStaffForAdmin(adminId, adminPassword);

                if (patron == null)
                {return StatusCode(404, "Patron with that Id does not exist!");}
                else if (adminCheck == false) 
                { return StatusCode(404, "You do not have proper admin credentials!"); }
                else
                {
                    await _patronDao.DeletePatronById(patron.Id);
                    return StatusCode(200, "Patron has been deleted.");
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }

    public class StaffController : ControllerBase
    {
        private readonly IStaffDao _staffDao;

        public StaffController(IStaffDao _staffDao)
        {
            this._staffDao = _staffDao;
        }

        [HttpGet]
        [Route("Staff/{adminId}, {adminPassword}")]
        public async Task<IActionResult> GetStaff([FromRoute] int adminId, [FromRoute] string adminPassword)
        {
            try
            {
                var adminCheck = await _staffDao.CheckStaffForAdmin(adminId, adminPassword);
                if (adminCheck == false)
                {
                    return StatusCode(404, "You do not have proper admin credentials!");
                }
                var staff = await _staffDao.GetStaff();
                return Ok(staff);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        [HttpPost]
        [Route("Staff")]
        public async Task<IActionResult> AddStaff(string firstName, string lastName, string phoneNumber, string position, string password, string confirmPassword)
        {
            
            try
            {
                if (password == confirmPassword)
                {
                    await _staffDao.AddStaff(firstName, lastName, phoneNumber, position, password);
                    return Ok();
                }
                else
                {
                    return StatusCode(400, "Passwords entered do not match!");
                }
            }
            
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPatch]
        [Route("Staff/{adminId}, {adminPassword}, {staffId}")]
        public async Task<IActionResult> UpdateStaffById([FromRoute] int adminId, [FromRoute] string adminPassword, [FromRoute] int staffId,  string firstName, string lastName, string phoneNumber, string position, string password)
        {
            var updateStaff = new StaffModel();
            try
            {
                var staff = await _staffDao.GetStaffById(staffId);
                var adminCheck = await _staffDao.CheckStaffForAdmin(adminId, adminPassword);

                if (adminCheck == false)
                {return StatusCode(404, "You do not have proper admin credentials!");}

                else if (staff == null)
                { return StatusCode(404, "No staff member with that Id."); }

                else
                {
                    updateStaff.Id = staffId;
                    if (firstName == null)
                    { updateStaff.FirstName = staff.FirstName; }
                    else { updateStaff.FirstName = firstName; }

                    if (lastName == null)
                    { updateStaff.LastName = staff.LastName; }
                    else { updateStaff.LastName = lastName; }

                    if (phoneNumber == null) 
                    { updateStaff.PhoneNumber = staff.PhoneNumber; }
                    else
                    {updateStaff.PhoneNumber = phoneNumber;}

                    if (position == null)
                    { updateStaff.Position = position; }
                    else 
                    { updateStaff.Position = position; }

                    if (password == null)
                    { updateStaff.Password = password; }
                    else
                    { updateStaff.Password = password; }

                    await _staffDao.UpdateStaffById(updateStaff);
                    return StatusCode(200, "Staff member has been updated.");
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete]
        [Route("Staff/{adminId}, {adminPassword}, {staffId}")]
        public async Task<IActionResult> DeleteStaffById([FromRoute] int adminId, [FromRoute] string adminPassword, [FromRoute] int staffId)
        {
            try
            {
                var staff = await _staffDao.GetStaffById(staffId);
                var adminCheck = await _staffDao.CheckStaffForAdmin(adminId, adminPassword);
                if (staff == null)
                {
                    return StatusCode(404, "No staff with that Id.");
                }
                else if (adminCheck == false)
                {
                    return StatusCode(404, "You do not have proper admin credentials!");
                }
                else
                {
                    await _staffDao.DeleteStaffById(staffId);
                    return StatusCode(200, "Staff member has been deleted.");
                }

            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}
