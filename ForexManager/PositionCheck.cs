using System;
using System.Threading.Tasks;
using Flexoft.ForexManager.BusinessLogic;
using Flexoft.ForexManager.RatesFetcher;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ForexManager
{
	public class PositionCheck
    {
		readonly IRateEvaluator _evaluator;

		public PositionCheck(IRateEvaluator evaluator)
		{
			_evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
		}

		[FunctionName("PositionCheck")]
		public async Task Run([TimerTrigger("0 0 7-21 * * *")] TimerInfo myTimer, ILogger log)
		{
            try
            {
                await _evaluator.EvaluateRateAsync(Currency.EUR, Currency.USD);
            }
            catch (Exception exception)
            {
                log.LogError($"Position check failed due {exception}");
            }
        }
	}
}
