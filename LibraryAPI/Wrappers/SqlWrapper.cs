using LibraryAPI.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryAPI.Wrappers
{
    public class SqlWrapper : ISqlWrapperBook, ISqlWrapperPatron 
    {
        public static string ConnectionString;
        public SqlWrapper()
        {
            ConnectionString = "server=VUHL-H0HJKN3\\SQLExpress; database=Library; Integrated Security=True; TrustServerCertificate=True";
        }

        public Task<IEnumerable> QueryBook<BookModel>(string sql)
        {
            throw new System.NotImplementedException();
        }
        public Task<IEnumerable>QueryPatron<PatronModel>(string sql)
        {
            throw new System.NotImplementedException();
        }
        public Task<IEnumerable<PatronModel>> GetListOfAllPatrons()
        {
            throw new System.NotImplementedException();
        }
    }
}
