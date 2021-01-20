using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Flexoft.ForexManager.NotificationManager;
using Flexoft.ForexManager.RatesFetcher;

namespace Flexoft.ForexManager.ForexManager
{
    public class Test
    {
        readonly INotificationManager _notificationManager;
        readonly IRates _ratesFetcher;
        readonly Options _options;

        public Test(INotificationManager notificationManager, IRates ratesFetcher, Options options)
        {
            _notificationManager = notificationManager ?? throw new ArgumentNullException(nameof(notificationManager));
            _ratesFetcher = ratesFetcher ?? throw new ArgumentNullException(nameof(ratesFetcher));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        [FunctionName("Test")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var responseMessage = _options.NotificationTarget + "<br>" + _notificationManager.Dump();
            try
            {
                var rate = await _ratesFetcher.GetRateAsync(Currency.EUR, Currency.USD);
                _notificationManager.Notify("Fx Manager Notification", $"EUR - USD rate: {rate}", _options.NotificationTarget);

                responseMessage += "<br>success";
            }
            catch (Exception exception)
            {
                responseMessage += exception.ToString();
            }

            //string name = req.Query["name"];

            //string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            //dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            //string responseMessage = string.IsNullOrEmpty(name)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
