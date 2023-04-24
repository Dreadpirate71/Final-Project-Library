using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
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
using Swashbuckle.AspNetCore.Annotations;

namespace LibraryAPI.Controllers
{
    public class BooksController : ControllerBase
    {
        private readonly IBookDao _bookDao;
        private readonly IPatronDao _patronDao;
        private readonly IStaffDao _staffDao;

        public BooksController(IBookDao bookDao, IPatronDao patronDao, IStaffDao staffDao)
        {
            _bookDao = bookDao;
            _patronDao = patronDao;
            _staffDao = staffDao;
        }

        [HttpGet]
        [Route("Books")]
        public async Task <IActionResult>GetBook(int bookById, string bookByTitle, string authorLName, string booksByGenre, 
                                                [SwaggerParameter("Enter Id to get current books checked out.")] int patronId, 
                                                [SwaggerParameter("Enter Id to get books checked out history.")] int patronIdHistory, 
                                                [SwaggerParameter("Enter Y to select.")] string waitList, [SwaggerParameter("Enter Y to select.")]string overdueBooks,
                                                [SwaggerParameter("Enter 'In' or 'Out'.")] string status)
        {
            try
            {
                if (bookById != 0)
                {
                    var book = await _bookDao.GetBookById(bookById);
                    if (book == null)
                    {
                        return StatusCode(404, "No book found with that Id.");
                    }
                    return Ok(book);
                }
                else if (bookByTitle != null)
                {
                    var book = await _bookDao.GetBookByTitle(bookByTitle);
                    if (book == null)
                    {
                        return StatusCode(404, "No book found with that title.");
                    }
                    return Ok(book);
                }
                else if (authorLName != null)
                {
                    var books = await _bookDao.GetBooksByAuthorLName(authorLName);
                    if (books == null)
                    {
                        return StatusCode(404, "No books found by that author.");
                    }
                    return Ok(books);
                }
                else if (booksByGenre != null)
                {
                    var books = await _bookDao.GetBookByGenre(booksByGenre);
                    if (books == null)
                    {
                        return StatusCode(404, "No books found in that genre.");
                    }
                    return Ok(books);
                }
                else if (patronId != 0)
                {
                    var patron = await _patronDao.GetPatronById(patronId);
                    if (patron == null)
                    {
                        return StatusCode(404, "Patron with that Id does not exist.");
                    }
                    var booksOut = await _bookDao.GetListOfBooksCheckedOut(patronId);
                    
                    if (booksOut == null)
                    {
                        return StatusCode(404, "No books checked out by patron with that Id.");
                    }
                    return Ok(booksOut);
                }
                else if (patronIdHistory != 0)
                {
                    var patron = await _patronDao.GetPatronById(patronIdHistory);
                    if (patron == null)
                    {
                        return StatusCode(404, "Patron with that Id does not exist.");
                    }
                    var booksOut = await _bookDao.GetBooksHistory(patronIdHistory);

                    if (booksOut == null)
                    {
                        return StatusCode(404, "No books checked out history for patron with that Id.");
                    }
                    var booksHistory = booksOut.ToList();
                    var booksList = booksHistory[0].Split('|');
                    return Ok(booksList);
                }

                else if (waitList != null)
                {
                    var waitBooks = await _bookDao.GetWaitListBooks();
                    if (waitBooks == null)
                    {
                        return StatusCode(404, "There are no books on the waitlist.");
                    }
                    return Ok(waitBooks);
                }
                else if (overdueBooks != null)
                {
                    var booksDue = await _bookDao.GetOverdueBooks();
                    if (booksDue == null)
                    {
                        return StatusCode(404, "There are currently no overdue books.");
                    }
                    else { return Ok(booksDue); }
                }
                else if (!string.IsNullOrEmpty(status))
                {
                    var books = await _bookDao.GetListOfBooksByStatus(status);
                    if (books == null)
                    {
                        return StatusCode(404, "There were no books found with that status.");
                    }
                    return Ok(books);
                }
                else { return BadRequest(); }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

        }      
        
        [HttpPost]
        [Route("Book")]
        public async Task<IActionResult> AddBook(string bookTitle, string authorFname, string authorLName, string genre, decimal price)
        {
            try
            {
                var message = "";
                await _bookDao.AddBook(bookTitle, authorFname, authorLName, genre, price);
                message = bookTitle + " has been added to library.";
                return Ok(message);
                
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPatch]
        [Route("Book")]
        public async Task<IActionResult> UpdateBookById([SwaggerParameter(Required = true)] int bookId, string updateBookTitle, string updateAuthorFName, string updateAuthorLName, string updateGenre, decimal updatePrice)
        {
            var updateBook = new BookModel();
            
            try
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
                    updateBook.DueDate = book.DueDate;
                    updateBook.PatronId = book.PatronId;  
                        
                }
                await _bookDao.UpdateBookById(updateBook);
                var message = updateBook.BookTitle + " has been updated.";
                return StatusCode(200, message);
                
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPatch]
        [Route("Books")]
        [SwaggerOperation("Used to check out a book, return a book, or add a book to the waitlist.")]
        public async Task <IActionResult> UpdateBook([SwaggerParameter(Required = true)]string bookTitle, [SwaggerParameter(Required = true)] string patronEmail, [SwaggerParameter("Enter Y to select")] string checkOutBook, 
                                                    [SwaggerParameter("Enter Y to select")] string returnBook, [SwaggerParameter("Enter Y to select")] string waitListBook)
        {
            try
            {
                var dueDate = DateTime.Now.AddDays(14);

                var book = await _bookDao.GetBookByTitle(bookTitle);
                if (book == null)
                {
                    return StatusCode(404, "Book with this title does not exist!");
                }
                var patron = await _patronDao.GetPatronByEmail(patronEmail);
                if (patron == null)
                {
                    return StatusCode(404, "Patron with that email does not exist.");
                }
                var patronBooksOut = await _bookDao.GetTotalOfCheckedOutBooks(patron.Id);
                if (checkOutBook != null)
                {
                    if (book.Status == "Out")
                    {
                        return StatusCode(400, "Book Status = 'Out'. Please choose a book that is not already checked out.");
                    }
                    if (patronBooksOut >= 5)
                    {
                        return StatusCode(400, "Exceeded maximum of 5 books checked out! Please return a book to proceed.");
                    }
                    else
                    {
                        book.Status = "Out";
                        book.PatronId = patron.Id;
                        book.DueDate = dueDate;
                        if (patron.BooksHistory == null)
                        {
                            patron.BooksHistory = book.BookTitle;
                            await _patronDao.UpdatePatronById(patron);
                        }
                        else
                        {
                            patron.BooksHistory = patron.BooksHistory + "|" + book.BookTitle;
                            await _patronDao.UpdatePatronById(patron);
                        }
                        await _bookDao.UpdateBookById(book);
                        var statusMessage = bookTitle + " has been checked out.";
                        return StatusCode(200, statusMessage);
                    }
                }
                else if (returnBook != null)
                {
                    var bookPatron = await _bookDao.GetBookByTitleAndId(bookTitle, patron.Id);
                    if (bookPatron == null)
                    {
                        return StatusCode(404, "No book checked out by that patron with that title!");
                    }
                    var waitListBooks = await _bookDao.CheckForBookOnWaitList(bookTitle);
                    if (waitListBooks.Count() == 0)
                    {
                        book.PatronId = null;
                        book.Status = "In";
                        book.DueDate = null;
                        await _bookDao.UpdateBookById(book);
                        var statusMessage = book.BookTitle + " has been returned.";
                        return StatusCode(200, statusMessage);
                    }
                    else
                    {
                        int elem = 0;

                        var waitBook = waitListBooks.ElementAt(elem);
                        var waitStatusMessage = bookTitle + " has been checked out to first eligible patron on waitlist.";
                        patronBooksOut = await _bookDao.GetTotalOfCheckedOutBooks(waitBook.PatronId);
                        if (patronBooksOut < 5)
                        {
                            book.PatronId = waitBook.PatronId;
                            book.Status = "Out";
                            book.DueDate = dueDate;
                            if (patron.BooksHistory == null)
                            {
                                patron.BooksHistory = book.BookTitle;
                                await _patronDao.UpdatePatronById(patron);
                            }
                            else
                            {
                                patron.BooksHistory = patron.BooksHistory + "|" + book.BookTitle;
                                await _patronDao.UpdatePatronById(patron);
                            }
                            await _bookDao.UpdateBookById(book);
                            await _bookDao.DeleteWaitListBook(waitBook.PatronId, waitBook.BookTitle);
                            return StatusCode(200, waitStatusMessage);
                        }
                        do
                        {
                            elem++;
                            if (elem < waitListBooks.Count())
                            {
                                //elem++;
                                waitBook = waitListBooks.ElementAt(elem);
                                patronBooksOut = await _bookDao.GetTotalOfCheckedOutBooks(waitBook.PatronId);
                                if (patronBooksOut < 5)
                                {
                                    book.PatronId = waitBook.PatronId;
                                    book.Status = "Out";
                                    book.DueDate = dueDate;
                                    if (patron.BooksHistory == null)
                                    {
                                        patron.BooksHistory = book.BookTitle;
                                        await _patronDao.UpdatePatronById(patron);
                                    }
                                    else
                                    {
                                        patron.BooksHistory = patron.BooksHistory + "|" + book.BookTitle;
                                        await _patronDao.UpdatePatronById(patron);
                                    }
                                    await _bookDao.UpdateBookById(book);
                                    await _bookDao.DeleteWaitListBook(waitBook.PatronId, waitBook.BookTitle);
                                    return StatusCode(200, waitStatusMessage);
                                }
                            }
                        } while (patronBooksOut >= 5 && elem + 1 < waitListBooks.Count());

                        book.PatronId = 1003;
                        book.Status = "In";
                        book.DueDate = null;
                        await _bookDao.UpdateBookById(book);
                        var statusMessage = book.BookTitle + " has been returned.";
                        return StatusCode(200, statusMessage);
                    }
                }
                else if (waitListBook !=  null)
                {
                    if (book.Status == "In")
                    {
                        var statusMessage = book.BookTitle + " is available to check out.";
                        return StatusCode(400, statusMessage);
                    }
                    else
                    {
                        var waitBook = _bookDao.BookWaitList(patron.Id, book.BookTitle, book.AuthorFName, book.AuthorLName);
                        var statusMessage = book.BookTitle + " has been added to waitlist.";
                        return StatusCode(200, statusMessage);
                    }
                }
                else { return StatusCode(403); }
            }
            catch (Exception e) 
            {
                return StatusCode(500, e.Message);
            }
        }
        
        [HttpDelete]
        [Route("Book")]
        public async Task<IActionResult> DeleteBookById([SwaggerParameter(Required = true)] int bookId)
        {
            try
            {
                var message = "";
                               
                var book = await _bookDao.GetBookById(bookId);
                if (book == null)
                {
                    return StatusCode(404, "No book found with that Id!");
                }
                await _bookDao.DeleteBookById(book.Id);
                message = book.BookTitle + " with Id " + book.Id + " has been deleted.";
                return StatusCode(200, message);
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
        private readonly PatronModel _patronModel;
        
        public PatronsController(IPatronDao patronDao, IStaffDao staffDao)
        {
            _patronDao= patronDao;
            _staffDao= staffDao;
        }
        
        
        [HttpGet]
        [Route("Patron")]
        public async Task<IActionResult> GetPatron(int id, string email, string phoneNumber, string lastName)
        {
            try
            {
                if (id != 0)
                {
                    var patron = await _patronDao.GetPatronById(id);
                    if (patron == null)
                    {
                        return StatusCode(404, "No patron with that Id exists.");
                    }
                    return Ok(patron);
                }
                else if (!string.IsNullOrEmpty(email))
                {
                    var patron = await _patronDao.GetPatronByEmail(email);
                    if (patron == null)
                    {
                        return StatusCode(404, "No patron with that email exists.");
                    }
                    return Ok(patron);
                }
                else if (!string.IsNullOrEmpty(phoneNumber))
                {
                    var patron = await _patronDao.GetPatronByPhoneNumber(phoneNumber);
                    if (patron == null)
                    {
                        return StatusCode(404, "No patron with that phone number exists.");
                    }
                    return Ok(patron);
                }
                else if (!string.IsNullOrEmpty(lastName))
                {
                    var patron = await _patronDao.GetPatronByLastName(lastName);
                    if (patron.Count() == 0)
                    {
                        return StatusCode(404, "No patron with that last name exists.");
                    }
                    return Ok(patron);
                }
                else
                    return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        [Route("Patron")]
        public async Task<IActionResult> AddPatron(string firstName, string lastName, string email, string streetAddress, string city, string state, string postalCode, string phoneNumber)
        {
            try
            {
                var newPatron = new PatronModel();
                if (!newPatron.IsValidEmailRegEx(email))
                { return StatusCode(400, "Please enter a valid email address!"); }
                else
                {
                    var emailUnique = await _patronDao.CheckEmailUnique(email);
                    if (emailUnique == true)
                    {
                        newPatron.FirstName = firstName;
                        newPatron.LastName = lastName;
                        newPatron.Email = email;
                        newPatron.StreetAddress = streetAddress;
                        newPatron.City = city;
                        newPatron.State = state;
                        newPatron.PostalCode = postalCode;
                        if (newPatron.CheckPhoneNumber(phoneNumber) == true)
                        { newPatron.PhoneNumber = phoneNumber; }
                        else
                        { return StatusCode(400, "The phone number entered is not valid!"); }
                        newPatron.PhoneNumber = phoneNumber;
                        newPatron.BooksHistory = null;
                        await _patronDao.AddPatron(newPatron);
                        return Ok("New Patron created.");                        
                    }
                    else
                    { return StatusCode(400, "That email is already in use. Please use a different email."); }
                }
            }
            catch(Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
        
        [HttpPatch]
        [Route("Patron")]
        public async Task<IActionResult> UpdatePatronByEmail([SwaggerParameter(Required = true)]string currentEmail, string firstName, string lastName, 
                                                                string updateEmail, string streetAddress, string city, string state, string postalCode, 
                                                                string phoneNumber)
        {
            var updatePatron = new PatronModel();
            try
            {             
                var patron = await _patronDao.GetPatronByEmail(currentEmail);
                if (patron == null)
                {
                    return StatusCode(404, "No patron with that email exists.");
                }
                updatePatron.Id = patron.Id;

                if (firstName == null)
                { updatePatron.FirstName = patron.FirstName; }
                else
                { updatePatron.FirstName = firstName; }

                if (lastName == null)
                { updatePatron.LastName = patron.LastName; }
                else
                { updatePatron.LastName = lastName; }

                if (updateEmail == null)
                { updatePatron.Email = patron.Email; }
                else
                {
                    if (!updatePatron.IsValidEmailRegEx(updateEmail))
                    { return StatusCode(400, "Please enter a valid email address!"); }
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
                }
                if (streetAddress == null)
                { updatePatron.StreetAddress = patron.StreetAddress; }
                else
                { updatePatron.StreetAddress = streetAddress; }

                if (city == null)
                { updatePatron.City = patron.City; }
                else
                { updatePatron.City = city; }

                if (state == null)
                { updatePatron.State = patron.State; }
                else
                { updatePatron.State = state; }

                if (postalCode == null)
                { updatePatron.PostalCode = patron.PostalCode; }
                else
                { updatePatron.PostalCode = postalCode; }

                if (phoneNumber == null)
                { updatePatron.PhoneNumber = patron.PhoneNumber; }
                else
                {
                    if (updatePatron.CheckPhoneNumber(phoneNumber) == true)
                    { updatePatron.PhoneNumber = phoneNumber; }
                    else
                    { return StatusCode(400, "The phone number entered is not valid!"); }
                }
                
                await _patronDao.UpdatePatronById(updatePatron);
                return Ok("Patron has been updated!");
                
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpDelete]
        [Route("Patron")]
        public async Task<IActionResult>DeletePatronById([SwaggerParameter(Required = true)] int patronId)
        {
            try
            {
                var patron = await _patronDao.GetPatronById(patronId);
                
                if (patron == null)
                {return StatusCode(404, "Patron with that Id does not exist!");}
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
        [Route("Staff")]
        public async Task<IActionResult> GetStaff(int id, string email, string lastName, string phoneNumber, string position)
        {
            try
            {
                if (id != 0)
                {
                    var staffMember = await _staffDao.GetStaffById(id);
                    if (staffMember == null)
                    {
                        return StatusCode(404, "No staff member with that Id exists.");
                    }
                    return Ok(staffMember);
                }
                else if (email != null)
                {
                    var staffMember = await _staffDao.GetStaffByEmail(email);
                    if (staffMember == null)
                    {
                        return StatusCode(404, "No staff member with that email exists.");
                    }
                    return Ok(staffMember);
                }
                else if (lastName != null)
                {
                    var staffMember = await _staffDao.GetStaffByLastName(lastName);
                    if (staffMember == null)
                    {
                        return StatusCode(404, "No staff member with that last name exists.");
                    }
                    return Ok(staffMember);
                }
                else if (phoneNumber != null)
                {
                    var staffMember = await _staffDao.GetStaffByPhoneNumber(phoneNumber);
                    if (staffMember == null)
                    {
                        return StatusCode(404, "No staff member with that phone number exists.");
                    }
                    return Ok(staffMember);
                }
                else if (position != null)
                {
                    var staffMembers = await _staffDao.GetStaffByPosition(position);
                    if (staffMembers == null)
                    {
                        return StatusCode(404, "No staff member with that position exists.");
                    }
                    return Ok(staffMembers);
                }
                else return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost]
        [Route("Staff")]
        public async Task<IActionResult> AddStaff(string firstName, string lastName, string phoneNumber, string email, string position)
        {
            var patron = new PatronModel();
            try
            {
                if (patron.CheckPhoneNumber(phoneNumber)== false)
                {
                    return StatusCode(400, "The phone number entered is not valid!");
                }
                else
                {
                    await _staffDao.AddStaff(firstName, lastName, phoneNumber, email, position);
                    return Ok("Staff member has been added.");
                }
                
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPatch]
        [Route("Staff")]
        public async Task<IActionResult> UpdateStaffById([SwaggerParameter(Required = true)] int staffId, string updateFirstName,
                                                            string updateLastName, string updatePhoneNumber, string updateEmail, string updatePosition)
        {
            var updateStaff = new StaffModel();
            var patron = new PatronModel();
            try
            {
                var staff = await _staffDao.GetStaffById(staffId);

                if (staff == null)
                { return StatusCode(404, "Staff member with that Id does not exist!"); }

                else
                {
                    updateStaff.Id = staffId;
                    if (updateFirstName == null)
                    { updateStaff.FirstName = staff.FirstName; }
                    else { updateStaff.FirstName = updateFirstName; }

                    if (updateLastName == null)
                    { updateStaff.LastName = staff.LastName; }
                    else { updateStaff.LastName = updateLastName; }

                    if (updateEmail == null)
                    { updateStaff.Email = staff.Email; }
                    else
                    {
                        if (!patron.IsValidEmailRegEx(updateEmail))
                        { return StatusCode(400, "Please enter a valid email address!"); }
                        else
                        {
                            var emailUnique = await _staffDao.CheckEmailUnique(updateEmail);
                            if (emailUnique == true)
                            {
                                updateStaff.Email = updateEmail;
                            }
                            else
                            {
                                return StatusCode(400, "That email is already in use. Please use a different email.");
                            }
                        }
                    }

                    if (updatePhoneNumber == null) 
                    { updateStaff.PhoneNumber = staff.PhoneNumber; }
                    else
                    {
                        if (patron.CheckPhoneNumber(updatePhoneNumber) == false)
                        {
                            return StatusCode(400, "The phone number entered is not valid!");
                        }
                        updateStaff.PhoneNumber = updatePhoneNumber;
                    }

                    if (updatePosition == null)
                    { updateStaff.Position = staff.Position; }
                    else 
                    { updateStaff.Position = updatePosition; }

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
        [Route("Staff")]
        public async Task<IActionResult> DeleteStaffById([SwaggerParameter(Required = true)] int staffId)
        {
            try
            {
                var staff = await _staffDao.GetStaffById(staffId);
                
                if (staff == null)
                {
                    return StatusCode(404, "Staff member with that Id does not exist!");
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
