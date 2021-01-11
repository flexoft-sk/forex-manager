using Flexoft.ForexManager.NotificationManager;
using NUnit.Framework;

namespace EmailSenderTests
{
    public class SmtplSenderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
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

            var sender = new SecureSmtpSender(options);
            sender.Send("test", "<h3>From unit test</h3>", "vladimir.iszer@gmail.com");
        }
    }
}