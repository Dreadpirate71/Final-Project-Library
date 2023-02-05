using Dapper;
using LibraryAPI.Models;
using LibraryAPI.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<PatronModel>GetPatronById(int Id)
        {
            var query = $"SELECT * FROM Patrons WHERE Id = '{Id}'";
            using var connection = _dapperContext.CreateConnection();
            {
                var patronById = await connection.QueryFirstOrDefaultAsync<PatronModel>(query);
                return patronById;
            }
        }
        public async Task<PatronModel>GetPatronByEmail(string Email)
        {
            var query = $"SELECT * FROM Patrons WHERE Email = '{Email}'";
            using var connection = _dapperContext.CreateConnection();
    
            var patronByEmail = await connection.QueryFirstOrDefaultAsync<PatronModel>(query);
            return patronByEmail;
        }
        public async Task UpdatePatronByEmail(PatronModel updateRequest)
        {
            var query = "UPDATE Patrons SET FirstName = @FirstName, LastName = @LastName," +
                        $"Email = @Email, StreetAddress = @StreetAddress, City = @City, State = @State, PostalCode = @ PostalCode, PhoneNumber = @PhoneNumber" +
                        $" WHERE Email = @Email";

            var parameters = new DynamicParameters();
            parameters.Add("@FirstName", updateRequest.FirstName, DbType.String);
            parameters.Add("@LastName", updateRequest.LastName, DbType.String);
            parameters.Add("@Email", updateRequest.Email, DbType.String);
            parameters.Add("@StreetAddress", updateRequest.StreetAddress, DbType.String);
            parameters.Add("@City", updateRequest.City, DbType.String);
            parameters.Add("@State", updateRequest.State, DbType.String);
            parameters.Add("@PostalCode", updateRequest.PostalCode, DbType.String);
            parameters.Add("@PhoneNumber", updateRequest.PhoneNumber, DbType.String);

            using var connection = _dapperContext.CreateConnection();
            await connection.ExecuteAsync(query, parameters);
        }

        public async Task AddPatron(string firstName, string lastName, string email, string streetAddress, string city, string state, string postalCode, string phoneNumber)
        {
            var query = "INSERT INTO Patrons (FirstName, LastName, Email, StreetAddress, City, State, PostalCode, PhoneNumber)" +
                $"VALUES (@FirstName, @LastName, @Email, @StreetAddress, @PostalCode, @State, @PostalCode, @PhoneNumber)";
            var parameters = new DynamicParameters();
            parameters.Add("@FirstName", firstName, DbType.String);
            parameters.Add("@LastName", lastName, DbType.String);
            parameters.Add("@Email", email, DbType.String);
            parameters.Add("@StreetAddress", streetAddress, DbType.String);
            parameters.Add("@City", city, DbType.String);
            parameters.Add("@State", state, DbType.String);
            parameters.Add("@PostalCode", postalCode, DbType.String);
            parameters.Add("@PhoneNumber", phoneNumber, DbType.String);

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
        public void GetListOfAllPatronsTest()
        {
            _sqlWrapper.QueryPatron<PatronModel>("SELECT * FROM Patrons");
        }
        public void UpdatePatron()
        {
            _sqlWrapper.QueryPatron<PatronModel>("UPDATE Patrons SET FirstName = @FirstName, LastName = @LastName," +
                                   $"Email = @Email, StreetAddress = @StreetAddress, City = @City, State = @State, PostalCode = @ PostalCode, PhoneNumber = @PhoneNumber" +
                                   $"WHERE Email = @Email");
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
