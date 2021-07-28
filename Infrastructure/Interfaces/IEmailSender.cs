using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string text);
    }
}
