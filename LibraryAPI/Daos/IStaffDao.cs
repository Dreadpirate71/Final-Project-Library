using LibraryAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryAPI.Daos
{

    public interface IStaffDao
    {
        Task AddStaff(string firstName, string lastName, string phoneNumber, string email, string position);
        Task<IEnumerable<StaffModel>> GetStaff();
        Task<StaffModel> GetStaffById(int id);
        Task<StaffModel> GetStaffByEmail(string email);
        Task<StaffModel> GetStaffByPhoneNumber(string phoneNumber);
        Task<IEnumerable<StaffModel>> GetStaffByLastName(string lastName);
        Task<IEnumerable<StaffModel>> GetStaffByPosition(string position);

        Task DeleteStaffById(int id);
        Task UpdateStaffById (StaffModel updateStaff);
        Task<bool> CheckEmailUnique(string email);

    }
    
}