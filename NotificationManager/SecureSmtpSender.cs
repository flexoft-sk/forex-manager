using System;
using System.Net;
using System.Net.Mail;

namespace Flexoft.ForexManager.NotificationManager
{
    public class SecureSmtpSender : INotificationManager
    {
        private readonly EmailSenderOptions _options;

        public SecureSmtpSender(EmailSenderOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void Notify(string title, string content, string receiver)
        {
            Send(title, content, receiver);
        }

        public void Send(string subject, string content, string to)
        {
            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentException(nameof(to));
            }

            var smtpClient = new SmtpClient();
            var basicCredential = new NetworkCredential(_options.User, _options.Password);
            var message = new MailMessage();
            var fromAddress = new MailAddress(_options.Sender);

            smtpClient.EnableSsl = true;
            smtpClient.Host = _options.Server;
            smtpClient.Port = _options.Port;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = basicCredential;

            message.From = fromAddress;
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = content;
            message.To.Add(to);

            smtpClient.Send(message);
        }
    }
}
