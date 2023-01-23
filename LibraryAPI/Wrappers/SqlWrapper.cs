using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryAPI.Wrappers
{
    public class SqlWrapper : ISqlWrapper
    {
        public static string ConnectionString;
        public SqlWrapper() 
        { 
            ConnectionString = "\"server=VUHL-H0HJKN3\\\\SQLExpress; database=Library; Integrated Security=True; TrustServerCertificate=True";
        }
      
        public Task<IEnumerable>Query<BookModel>(string sql)
        {
            throw new System.NotImplementedException();
        }
    }
}
