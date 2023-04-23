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
        Task UpdatePatronById(PatronModel patron);
        Task AddPatron(PatronModel newPatron);
        Task DeletePatronById(int id);
        Task <bool>CheckEmailUnique(string email);
        Task <PatronModel> GetPatronByPhoneNumber(string phoneNumber);
        Task <IEnumerable<PatronModel>> GetPatronByLastName(string lastName);
       
    } 
}
