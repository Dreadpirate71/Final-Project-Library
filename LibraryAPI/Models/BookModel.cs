using System;

namespace LibraryAPI.Models
{
    public class BookModel
    {
        public int Id { get; set; } 
        public string bookTitle { get; set; }   
        public string authorFName { get; set; }
        public string authorLName { get; set; }
        public string genre { get; set; }
        public decimal price{ get; set; }
        public string Status { get; set; } 
        public int patronId { get; set; }

        public void AddTitleName(string titleName)
        {
            throw new NotImplementedException();
        }
        public void AddAuthorNames(string authorName)
        {
            throw new NotImplementedException();
        }
    }
}
