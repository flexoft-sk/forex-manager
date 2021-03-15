using Flexoft.ForexManager.NotificationManager;
using Flexoft.ForexManager.RatesFetcher;
using System.Threading.Tasks;

namespace Flexoft.ForexManager.BusinessLogic
{
	public interface IRateEvaluator
	{
		Task EvaluateRateAsync(Currency from, Currency to);
		Task<(double amount, string currency)> CloseAsync(int pos, double rate, double? fee);

		INotificationManager NotificationManager { get; }

		public string NotificationTarget { get; }
	}
}
