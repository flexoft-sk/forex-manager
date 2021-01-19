﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flexoft.ForexManager.NotificationManager
{
    public static class NotificationManagerModule
    {
        public static void RegisterNotificationManager(this IServiceCollection services)
        {
            services.AddSingleton<INotificationManager>(provider => {
                var config = provider.GetService<IConfiguration>();
                var options = new EmailSenderOptions
                {
                    Password = config["EmailSender:Password"],
                    Port = int.Parse(config["EmailSender:Port"]),
                    Sender = config["EmailSender:Sender"],
                    Server = config["EmailSender:Server"],
                    User = config["EmailSender:User"],
                };

                return new SecureSmtpSender(options);
            });
        }
    }
}
