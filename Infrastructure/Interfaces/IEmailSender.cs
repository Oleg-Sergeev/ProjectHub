using System.Threading.Tasks;
using Infrastructure.Data.Entities;

namespace Infrastructure.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
