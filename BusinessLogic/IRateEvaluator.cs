using Flexoft.ForexManager.NotificationManager;
using Flexoft.ForexManager.RatesFetcher;
using System.Threading.Tasks;

namespace Flexoft.ForexManager.BusinessLogic
{
	/// <summary>Performs evaluation and opening of positions based on current rate.</summary>
	public interface IRateEvaluator
	{
		/// <summary>Gets the notification manager.</summary>
		/// <value>The notification manager.</value>
		INotificationManager NotificationManager { get; }

		/// <summary>Gets the notification target.</summary>
		/// <value>The notification target.</value>
		string NotificationTarget { get; }

		/// <summary>Evaluates and opens positions based on current rate.</summary>
		/// <param name="from">Source currency.</param>
		/// <param name="to">Target currency.</param>
		/// <returns></returns>
		Task EvaluateRateAsync(Currency from, Currency to);

		/// <summary>Closes a position with provided rate and fees.</summary>
		/// <param name="pos">The position.</param>
		/// <param name="rate">The rate.</param>
		/// <param name="fee">The fee.</param>
		/// <returns></returns>
		Task<(double amount, string currency)> CloseAsync(int pos, double rate, double? fee);
	}
}
