using System;
using Microsoft.OData.Edm;

namespace LibraryAPI.Models
{
    public class BookModel
    {
        public int Id { get; set; } 
        public string BookTitle{ get; set; }   
        public string AuthorFName { get; set; }
        public string AuthorLName { get; set; }
        public string Genre{ get; set; }
        public decimal Price{ get; set; }
        public string Status { get; set; } 
        public string CheckOutDate { get; set; }
        public int PatronId { get; set; }

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
