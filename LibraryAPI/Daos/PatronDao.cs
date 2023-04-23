using Dapper;
using LibraryAPI.Models;
using LibraryAPI.Wrappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Daos
{
    public class PatronDao : IPatronDao
    {
        private readonly DapperContext _dapperContext;
        private readonly ISqlWrapperPatron _sqlWrapper;

        public PatronDao(ISqlWrapperPatron sqlWrapper)
        {
            this._sqlWrapper = sqlWrapper;
        }
        public PatronDao(DapperContext dapperContext)
        {
            this._dapperContext = dapperContext;
        }

        public async Task<IEnumerable<PatronModel>> GetListOfAllPatrons()
        {
            var query = "SELECT * FROM Patrons";
            using var connection = _dapperContext.CreateConnection();
            var patrons = await connection.QueryAsync<PatronModel>(query);
            return patrons.ToList();
        }

        public async Task<PatronModel> GetPatronById(int id)
        {
            var query = $"SELECT * FROM Patrons WHERE Id = '{id}'";
            using var connection = _dapperContext.CreateConnection();
            {
                var patronById = await connection.QueryFirstOrDefaultAsync<PatronModel>(query);
                return patronById;
            }
        }
        public async Task<PatronModel> GetPatronByEmail(string email)
        {
            var query = $"SELECT * FROM Patrons WHERE Email = '{email}'";
            using var connection = _dapperContext.CreateConnection();

            var patronByEmail = await connection.QueryFirstOrDefaultAsync<PatronModel>(query);
            return patronByEmail;
        }
        public async Task<PatronModel> GetPatronByPhoneNumber(string phoneNumber)
        {
            var query = $"SELECT * FROM Patrons WHERE PhoneNumber = '{phoneNumber}'";
            using var connection = _dapperContext.CreateConnection();

            var patronByEmail = await connection.QueryFirstOrDefaultAsync<PatronModel>(query);
            return patronByEmail;
        }
        public async Task<IEnumerable<PatronModel>> GetPatronByLastName(string lastName)
        {
            var query = $"SELECT * FROM Patrons WHERE LastName = '{lastName}'";
            using var connection = _dapperContext.CreateConnection();

            var patronByEmail = await connection.QueryAsync<PatronModel>(query);
            return patronByEmail.ToList();
        }
        public async Task UpdatePatronById(PatronModel updatePatron)
        {
            var query = "UPDATE Patrons SET FirstName = @FirstName, LastName = @LastName," +
                        $"Email = @Email, StreetAddress = @StreetAddress, City = @City, State = @State, PostalCode = @PostalCode, PhoneNumber = @PhoneNumber," +
                        $"BooksHistory = @BooksHistory WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("@Id", updatePatron.Id, DbType.Int32);
            parameters.Add("@FirstName", updatePatron.FirstName, DbType.String);
            parameters.Add("@LastName", updatePatron.LastName, DbType.String);
            parameters.Add("@Email", updatePatron.Email, DbType.String);
            parameters.Add("@StreetAddress", updatePatron.StreetAddress, DbType.String);
            parameters.Add("@City", updatePatron.City, DbType.String);
            parameters.Add("@State", updatePatron.State, DbType.String);
            parameters.Add("@PostalCode", updatePatron.PostalCode, DbType.String);
            parameters.Add("@PhoneNumber", updatePatron.PhoneNumber, DbType.String);
            parameters.Add("@BooksHistory", updatePatron.BooksHistory, DbType.String);

            using var connection = _dapperContext.CreateConnection();
            await connection.ExecuteAsync(query, parameters);
        }
        
        public async Task AddPatron(PatronModel newPatron)
        {
            var query = "INSERT INTO Patrons (FirstName, LastName, Email, StreetAddress, City, State, PostalCode, PhoneNumber, BooksHistory)" +
                $"VALUES (@FirstName, @LastName, @Email, @StreetAddress, @City, @State, @PostalCode, @PhoneNumber, @BooksHistory)";
            var parameters = new DynamicParameters();
            parameters.Add("@FirstName", newPatron.FirstName, DbType.String);
            parameters.Add("@LastName", newPatron.LastName, DbType.String);
            parameters.Add("@Email", newPatron.Email, DbType.String);
            parameters.Add("@StreetAddress", newPatron.StreetAddress, DbType.String);
            parameters.Add("@City", newPatron.City, DbType.String);
            parameters.Add("@State", newPatron.State, DbType.String);
            parameters.Add("@PostalCode", newPatron.PostalCode, DbType.String);
            parameters.Add("@PhoneNumber", newPatron.PhoneNumber, DbType.String);
            parameters.Add("@BooksHistory", newPatron.BooksHistory, DbType.String);

            using var connection = _dapperContext.CreateConnection();
            await connection.ExecuteAsync(query, parameters);
        }

        public async Task DeletePatronById(int Id)
        {
            var query = $"DELETE FROM Patrons WHERE Id = '{Id}'";
            using var connection = _dapperContext.CreateConnection();
            {
                await connection.ExecuteAsync(query);
            }
        }

        public async Task<bool> CheckEmailUnique(string email)
        {
            var query = $"SELECT Email FROM Patrons WHERE Email = '{email}'";
            using var connection = _dapperContext.CreateConnection();
            {
                var patronEmail = await connection.QueryFirstOrDefaultAsync(query);
                if (patronEmail == null)
                { return true; }
                else 
                { return false; }
            }
        }

        public void GetListOfAllPatronsTest()
        {
            _sqlWrapper.QueryPatron<PatronModel>("SELECT * FROM Patrons");
        }
        public void UpdatePatron()
        {
            _sqlWrapper.QueryPatron<PatronModel>("UPDATE Patrons SET FirstName = @FirstName, LastName = @LastName," +
                                   $"Email = @Email, StreetAddress = @StreetAddress, City = @City, State = @State, PostalCode = @ PostalCode, PhoneNumber = @PhoneNumber" +
                                   $"WHERE Id = @Id");
        }
        public void AddPatron()
        {
            _sqlWrapper.QueryPatron<PatronModel>($"INSERT INTO Patrons (FirstName, LastName, Email, StreetAddress, City, State, PostalCode, PhoneNumber)" +
                $"VALUES (@FirstName, @LastName, @Email, @StreetAddress, @PostalCode, @State, @PostalCode, @PhoneNumber)");
        }
        public void GetPatronEmail()
        {
            _sqlWrapper.QueryPatron<PatronModel>("SELECT * FROM Patrons WHERE Email = '{Email}'");
        }

        public void DeletePatron()
        {
            _sqlWrapper.QueryPatron<PatronModel>("DELETE FROM Patrons WHERE Id = '{Id}'");
        }
        public void GetPatronId()
        {
            _sqlWrapper.QueryPatron<PatronModel>("SELECT * FROM Patrons WHERE Id = '{Id}'");
        }
    }
}
