using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Flexoft.ForexManager.BusinessLogic;

namespace Flexoft.ForexManager.ForexManager
{
    public class Close
    {
        readonly IRateEvaluator _evaluator;

		public Close(IRateEvaluator evaluator)
		{
            _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
        }

        [FunctionName("Close")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                var pos = int.Parse(req.Query["pos"]);
                var rate = double.Parse(req.Query["rate"]);

                var fee = double.TryParse(req.Query["fee"], out var f) ? f : (double?)null;

                var result = await _evaluator.CloseAsync(pos, rate, fee);
                return new OkObjectResult($"{result.amount} {result.currency}");
            }
            catch (Exception exception)
            {
                log.LogError($"Position close {req.QueryString} failed due {exception}");
                return new OkObjectResult(exception);
            }
        }
    }
}
