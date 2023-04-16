using Dapper;
using LibraryAPI.Models;
using LibraryAPI.Wrappers;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
using System;
using Microsoft.AspNetCore.Identity;

namespace LibraryAPI.Daos

{
    public class StaffDao : IStaffDao
    {
        private readonly DapperContext _context;
        private readonly ISqlWrapperStaff _sqlWrapper;

        public StaffDao(ISqlWrapperStaff sqlWrapper)
        {
            this._sqlWrapper = sqlWrapper;
        }
        public StaffDao(DapperContext context)
        {
            _context = context;
        }
        public async Task AddStaff(string firstName, string lastName, string phoneNumber, string position, string password)
        {
            var query = "INSERT INTO Staff (firstName, lastName, phoneNumber, position, password)" +
                $"VALUES (@FirstName, @LastName, @PhoneNumber, @Position, @Password);";

            var parameters = new DynamicParameters();
            parameters.Add("@FirstName", firstName, DbType.String);
            parameters.Add("@LastName", lastName, DbType.String);
            parameters.Add("@PhoneNumber", phoneNumber, DbType.String);
            parameters.Add("@Position", position, DbType.String);
            parameters.Add("@Password", password, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
        public async Task<IEnumerable<StaffModel>> GetStaff()
        {
            var query = "SELECT * FROM Staff";
            using (var connection = _context.CreateConnection())
            {
                var staff = await connection.QueryAsync<StaffModel>(query);
                return staff.ToList();
            }
        }
        public async Task<StaffModel> GetStaffById(int id)
        {
            var query = $"SELECT * FROM Staff WHERE Id = '{id}'";
            using (var connection = _context.CreateConnection())
            {
                var staff = await connection.QueryFirstOrDefaultAsync<StaffModel>(query);
                return staff;
            }
        }

        public async Task DeleteStaffById(int id)
        {
            var query = $"DELETE FROM Staff WHERE Id = {id}";
            using (var connection = _context.CreateConnection())
            {
                var staff = await connection.ExecuteAsync(query);
            }
        }
        public async Task UpdateStaffById(StaffModel updateRequest)
        {

            var query = "UPDATE Staff SET firstName = @FirstName, lastName = @LastName, phoneNumber = @PhoneNumber, " +
                        $"position =@Position, password = @Password WHERE id = @Id;";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", updateRequest.Id, DbType.Int32);
            parameters.Add("@FirstName", updateRequest.FirstName, DbType.String);
            parameters.Add("@LastName", updateRequest.LastName, DbType.String);
            parameters.Add("@PhoneNumber", updateRequest.PhoneNumber, DbType.String);
            parameters.Add("@Position", updateRequest.Position, DbType.String);
            parameters.Add("@Password", updateRequest.Password, DbType.String);


            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync (query, parameters);
            }
           
        }
        public async Task<bool> CheckStaffForAdmin(int id, string password)
        {
            var query = $"SELECT * FROM Staff WHERE Id = {id} AND position = 'Admin' AND Password = '{password}'";

            using (var connection = _context.CreateConnection())
            {
                var staff = await connection.QueryFirstOrDefaultAsync<StaffModel>(query);
                if (staff == null)
                {
                    return false;
                }
                return true;
            }
        }
        public void GetListOfAllStaffTest()
        {
            _sqlWrapper.QueryStaff<StaffModel>("SELECT * FROM Staff");
        }
        public void DeleteStaff()
        {
            _sqlWrapper.QueryStaff<StaffModel>("DELETE FROM Staff WHERE Id = '{Id}'");
        }

    }

}


