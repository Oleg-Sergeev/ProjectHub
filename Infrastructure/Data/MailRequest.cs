using MimeKit;

namespace Infrastructure.Data
{
    public class MailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public MessagePriority MessagePriority { get; set; } = MessagePriority.Normal;
    }
}
