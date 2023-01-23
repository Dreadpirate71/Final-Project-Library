using System;

namespace LibraryAPI.Models
{
    public class BookModel
    {
        public int Id { get; set; } 
        public string BookTitle { get; set; }   
        public string AuthorFName { get; set; }
        public string AuthorLName { get; set; }
        public string Genre { get; set; }
        public decimal Price{ get; set; }
        public string Status { get; set; } 
        public int PatronId { get; set; }

        public string AddTitleName(string bookTitle)
        {
            BookTitle = bookTitle;
            return bookTitle;
        }
        public void AddAuthorNames(string authorName)
        {
            AuthorFName= authorName;
        }
    }
}
