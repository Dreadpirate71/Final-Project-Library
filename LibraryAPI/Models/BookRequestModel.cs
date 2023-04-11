using Microsoft.Identity.Client;

namespace LibraryAPI.Models
{
    public class BookRequestModel
    {
        public int Id { get; set; }
        public int PatronId { get; set; }
        public string BookTitle { get; set; }
        public string AuthorFName { get; set; }
        public string AuthorLName { get; set; }
        public string WaitList { get; set; }

    }
}
