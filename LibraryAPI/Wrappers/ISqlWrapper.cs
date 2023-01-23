using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryAPI.Wrappers
{
    public interface ISqlWrapper
    {
        Task<IEnumerable> Query<BookModel>(string sql);  
    }
}
