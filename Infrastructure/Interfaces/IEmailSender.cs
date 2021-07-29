using System.Threading.Tasks;
using Infrastructure.Data;

namespace Infrastructure.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
