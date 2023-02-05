using LibraryAPI.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LibraryAPI.Wrappers
{
    public interface ISqlWrapperBook
    {
        Task<IEnumerable> QueryBook<BookModel>(string sql);
<<<<<<< HEAD
    }
    public interface ISqlWrapperPatron
    {
        Task<IEnumerable> QueryPatron<PatronModel>(string sql);
<<<<<<< HEAD
       
=======
>>>>>>> James
=======

    }
    public interface ISqlWrapperStaff
    {
        Task<IEnumerable> QueryStaff(string sql);
>>>>>>> b56c204da5bd9621ab93821d4e563494ed4de838
    }
    public interface ISqlWrapperPatron
    {
        Task<IEnumerable> QueryPatron<PatronModel>(string sql);
       
    }
    public interface ISqlWrapperStaff
    {
        Task<IEnumerable> QueryStaff(string sql);
    }
}
