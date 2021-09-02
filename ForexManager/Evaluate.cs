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
using Flexoft.ForexManager.RatesFetcher;
using System.Web.Http;

namespace Flexoft.ForexManager.ForexManager
{
    public class Evaluate
    {
        readonly IRateEvaluator _evaluator;

		public Evaluate(IRateEvaluator evaluator)
		{
			_evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
        }

		[FunctionName("Evaluate")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string fromParam = req.Query["from"];
            if (!Enum.TryParse<Currency>(fromParam, out var from))
            {
                return new BadRequestObjectResult("from");
            }

            string toParam = req.Query["to"];
            if (!Enum.TryParse<Currency>(toParam, out var to) || !Enum.IsDefined(typeof(Currency), to))
            {
                return new BadRequestObjectResult("to");
            }

            string rateParam = req.Query["rate"];
            if (!float.TryParse(rateParam, out var rate))
            {
                return new BadRequestObjectResult("rate");
            }

			try
			{
				await _evaluator.EvaluateRateAsync(from, to, rate);
				return new OkResult();
			}
			catch (Exception exception)
			{
                return new ExceptionResult(exception, true);
			}
        }
    }
}
