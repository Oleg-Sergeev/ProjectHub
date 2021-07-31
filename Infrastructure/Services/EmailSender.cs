using System.Threading.Tasks;
using Infrastructure.Data.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly MailSettings _mailSettings;


        public EmailSender(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }


        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var message = new MimeMessage()
            {
                Subject = mailRequest.Subject,
                Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = mailRequest.Body
                }
            };

            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));


            using var client = new SmtpClient();

            await client.ConnectAsync(_mailSettings.Host, _mailSettings.Port, _mailSettings.UseSsl);
            await client.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
