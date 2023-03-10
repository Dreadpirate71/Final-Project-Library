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
        public async Task AddStaff(string FirstName, string LastName, string PhoneNumber, string Position)
        {
            var query = "INSERT INTO Staff (FirstName, LastName, PhoneNumber, Position)" +
                $"VALUES (@FirstName, @LastName, @PhoneNumber, @Position);";

            var parameters = new DynamicParameters();
            parameters.Add("@FirstName", FirstName, DbType.String);
            parameters.Add("@LastName", LastName, DbType.String);
            parameters.Add("@PhoneNumber", PhoneNumber, DbType.String);
            parameters.Add("@Position", Position, DbType.String);


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
        public async Task<StaffModel> GetStaffById(int Id)
        {
            var query = $"SELECT * FROM Staff WHERE Id = '{Id}'";
            using (var connection = _context.CreateConnection())
            {
                var staff = await connection.QueryFirstOrDefaultAsync<StaffModel>(query);
                return staff;
            }
        }

        public async Task<IEnumerable<StaffModel>> DeleteStaffById(int Id)
        {
            var query = $"DELETE FROM Staff WHERE Id = {Id}";
            using (var connection = _context.CreateConnection())
            {
                var staff = await connection.QueryAsync<StaffModel>(query);
                return staff.ToList();
            }
        }
        public async Task UpdateStaffById(int Id, string FirstName, string LastName, string PhoneNumber, string Position)
        {

            var query = "UPDATE Staff SET FirstName = @FirstName, LastName = @LastName, PhoneNumber = @PhoneNumber, " +
                        $"Position =@Position WHERE Id = @Id;";



            var parameters = new DynamicParameters();
            parameters.Add("@Id", Id, DbType.Int32);
            parameters.Add("@FirstName", FirstName, DbType.String);
            parameters.Add("@LastName", LastName, DbType.String);
            parameters.Add("@PhoneNumber", PhoneNumber, DbType.String);
            parameters.Add("@Position", Position, DbType.String);


            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync (query, parameters);
            }
           
        }
        public async Task<StaffModel> CheckStaffForAdmin(int Id)
        {
            var query = $"SELECT * FROM Staff WHERE Id = {Id} AND position = 'Admin'";

            using (var connection = _context.CreateConnection())
            {
                var staff = await connection.QueryFirstOrDefaultAsync<StaffModel>(query);

                return staff;
            }
        }
        public void GetListOfAllStaffTest()
        {
            _sqlWrapper.QueryStaff<StaffModel>("SELECT * FROM Patrons");
        }
        public void DeleteStaff()
        {
            _sqlWrapper.QueryStaff<StaffModel>("DELETE FROM Staff WHERE Id = '{Id}'");
        }

    }

}


