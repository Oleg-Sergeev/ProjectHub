using System.Threading.Tasks;
using Infrastructure.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;

namespace Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private const string SenderName = "Projects hub";
        private const string FromEmailName = "csharpclr@gmail.com";
        private const string FromEmailPassword = "ZixxanGames777";

        public async Task SendEmailAsync(string email, string subject, string text)
        {
            var message = new MimeMessage()
            {
                Subject = subject
            };
            message.From.Add(new MailboxAddress(SenderName, FromEmailName));
            message.To.Add(MailboxAddress.Parse(email));

            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = text
            };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, false);
            await client.AuthenticateAsync(FromEmailName, FromEmailPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
