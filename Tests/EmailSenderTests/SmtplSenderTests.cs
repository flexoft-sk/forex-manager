using Flexoft.ForexManager.NotificationManager;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace EmailSenderTests
{
    public class SmtplSenderTests
    {
        ILogger<SecureSmtpSender> _logger;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<ILogger<SecureSmtpSender>>();
        }

        //[Test]
        public void IntegrationTest()
        {
            var options = new EmailSenderOptions
            {
                Server = "mail005.nameserver.sk",
                Port = 587,
                User = "yourUser",
                Password = "yourPassword",
                Sender = "test@forexmanager.sk"
            };

            var sender = new SecureSmtpSender(_logger, options);
            sender.Send("test", "<h3>From unit test</h3>", "test.target@gtest.com");
        }
    }
}