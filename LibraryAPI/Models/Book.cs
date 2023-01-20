using System;

namespace LibraryAPI.Models
{
    public class Book
    {
        public int titleId { get; set; } 
        public string titleName { get; set; }   
        public string authorFName { get; set; }
        public string authorLName { get; set; }
        public string type { get; set; }
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
