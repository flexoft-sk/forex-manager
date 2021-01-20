using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;

namespace Flexoft.ForexManager.NotificationManager
{
    public class SecureSmtpSender : INotificationManager
    {
        private readonly EmailSenderOptions _options;
        private readonly ILogger<SecureSmtpSender> _logger;

        public SecureSmtpSender(ILogger<SecureSmtpSender> logger, EmailSenderOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

		public string Dump()
		{
            return $"{_options.Server}:{_options.Port} {_options.User} {_options.Sender}";
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
            _logger.LogInformation($"Mail sent succesfully to {to}");
        }
    }
}
