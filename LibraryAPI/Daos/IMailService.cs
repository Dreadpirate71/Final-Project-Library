using LibraryAPI.Models;
using System.Threading.Tasks;

namespace LibraryAPI.Daos
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequestModel mailRequest);
    }
}
