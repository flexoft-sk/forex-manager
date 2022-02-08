using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Flexoft.ForexManager.ForexManager
{
	public class CloseUI
	{
        static readonly string _htmlTemplate;
        private readonly IConfiguration _configuration;

        public CloseUI(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        static CloseUI() 
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Flexoft.ForexManager.ForexManager.close.html");
            using var reader = new StreamReader(stream, Encoding.UTF8);
            _htmlTemplate = reader.ReadToEnd();
        }

        [FunctionName("CloseUI")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var id = int.Parse(req.Query["id"]);
                var url = _configuration["CloseFunctionRelativeURL"];

                url += url.Contains('?')
                    ? "&"
                    : "?";

                return new ContentResult
                {
                    
                    Content = _htmlTemplate.Replace("__id__", id.ToString())
                        .Replace("__close_url__", url),
                    StatusCode = 200,
                    ContentType = "text/html; charset=utf-8"
                };
            }
            catch (Exception exception)
            {
                log.LogError($"Position close UI {req.QueryString} failed due {exception}");
                return new OkObjectResult(exception);
            }
        }
    }
}
