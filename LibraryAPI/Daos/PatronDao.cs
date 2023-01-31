using Dapper;
using LibraryAPI.Models;
using LibraryAPI.Wrappers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryAPI.Daos
{
    public class PatronDao : IPatronDao
    {
        private readonly DapperContext _dapperContext;
        private readonly ISqlWrapper _sqlWrapper;

        public PatronDao(ISqlWrapper SqlWrapper)
        { 
            this._sqlWrapper = SqlWrapper;
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
    }
}
