using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
        public Date? CheckOutDate { get; set; }
        public int PatronId { get; set; }

        public string AddTitleName(string titleName)
        {
            BookTitle = titleName;
            return BookTitle;
        }
        public void AddAuthorNames(string authorName)
        {
            AuthorFName= authorName;
        }
        public Task OpenUrl(string url)
        {
            Process pro = new Process();
            pro.StartInfo.FileName = "C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe";
            pro.StartInfo.Arguments = url;
            pro.Start();
            return Task.CompletedTask;
        }
    }
}
