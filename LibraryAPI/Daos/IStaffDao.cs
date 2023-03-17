using LibraryAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryAPI.Daos
{

    public interface IStaffDao
    {
        Task AddStaff(string FirstName, string LastName, string PhoneNumber, string Position);
        Task<IEnumerable<StaffModel>> GetStaff();
        Task<StaffModel> GetStaffById(int Id);
        Task<IEnumerable<StaffModel>> DeleteStaffById(int Id);
        Task UpdateStaffById(int Id, string FirstName, string LastName, string PhoneNumber, string Position);
        Task <bool>CheckStaffForAdmin(int Id);

    }
    
}