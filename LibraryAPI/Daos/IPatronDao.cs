using LibraryAPI.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryAPI.Daos
{
    public interface IPatronDao
    {
        Task<IEnumerable<PatronModel>> GetListOfAllPatrons();
        Task <PatronModel> GetPatronById(int id);

        Task <PatronModel> GetPatronByEmail(string email);
        Task UpdatePatronByEmail(PatronModel patron);
        Task AddPatron(string firstName, string lastName, string email, string streetAddress, string city, string state, string postalCode, string phoneNumber);
        Task DeletePatronById(int id);
    }
}
