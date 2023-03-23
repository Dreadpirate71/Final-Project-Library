using LibraryAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryAPI.Daos
{

    public interface IStaffDao
    {
        Task AddStaff(string firstName, string lastName, string phoneNumber, string position, string password);
        Task<IEnumerable<StaffModel>> GetStaff();
        Task<StaffModel> GetStaffById(int id);
        Task<IEnumerable<StaffModel>> DeleteStaffById(int id);
        Task UpdateStaffById (StaffModel updateStaff);
        Task <bool>CheckStaffForAdmin(int id, string password);
       
    }
    
}