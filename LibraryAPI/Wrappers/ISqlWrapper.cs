using LibraryAPI.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryAPI.Wrappers
{
    public interface ISqlWrapperBook
    {
        Task<IEnumerable> QueryBook<BookModel>(string sql);
    }
    public interface ISqlWrapperPatron
    {
        Task<IEnumerable> QueryPatron<PatronModel>(string sql);

    }
}
