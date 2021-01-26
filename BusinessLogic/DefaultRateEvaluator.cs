using Flexoft.ForexManager.NotificationManager;
using Flexoft.ForexManager.RatesFetcher;
using Flexoft.ForexManager.Store;
using Flexoft.ForexManager.Store.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flexoft.ForexManager.BusinessLogic
{
	public class DefaultRateEvaluator : IRateEvaluator
	{
		private readonly ILogger<DefaultRateEvaluator> _logger;
		private readonly INotificationManager _notificationManager;
		private readonly IRates _rateFatcher;
		private readonly IDataStore _dataStore;
		private readonly RateEvaluatorOptions _options;

		public DefaultRateEvaluator(RateEvaluatorOptions options, ILogger<DefaultRateEvaluator> logger, 
			INotificationManager notificationManager, IRates rateFatcher, IDataStore dataStore)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_notificationManager = notificationManager ?? throw new ArgumentNullException(nameof(notificationManager));
			_rateFatcher = rateFatcher ?? throw new ArgumentNullException(nameof(rateFatcher));
			_dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
			_options = options ?? throw new ArgumentNullException(nameof(options));
		}

		public async Task EvaluateRateAsync(Currency from, Currency to)
		{
			var rate = await _rateFatcher.GetRateAsync(from, to);
			var reversedRate = 1f / rate;

			var closeOportunities = await FindCloseOportunitiesAsync(from, to, rate);

			_logger.LogInformation($"Found opportunities for {from} -> {to}: {string.Join(",", closeOportunities.Select(p => p.Id))}");

			var reverseOpportunities = await FindCloseOportunitiesAsync(to, from, reversedRate);
			_logger.LogInformation($"Found opportunities for {to} -> {from}: {string.Join(",", reverseOpportunities.Select(p => p.Id))}");
			closeOportunities.AddRange(reverseOpportunities);

			var notification = string.Join("<br>", closeOportunities.Select(p => $"[{p.Id}] {p.FromCurrency} -> {p.ToCurrency} : {p.OpenAmount} for {p.OpenRate}. Proposal: {p.OpenAmount*p.OpenRate}"));

			if (!string.IsNullOrEmpty(notification))
			{
				_notificationManager.Notify("Opportunities", notification, _options.NotificationTarget);
			}

			if (DateTime.UtcNow.Hour == _options.OpenHour)
			{
				var reversedAmount = _options.OpenAmount / reversedRate;
				await _dataStore.Position.OpenAsync(from.ToString(), to.ToString(), _options.OpenAmount, rate);
				await _dataStore.Position.OpenAsync(to.ToString(), from.ToString(), reversedAmount, reversedRate);

				_logger.LogInformation($"Opened: {from} -> {to} for {rate}");
				_logger.LogInformation($"Opened: {to} -> {from} for {reversedRate}");

				_notificationManager.Notify("Opened", $"{from} -> {to} : {rate} [{reversedRate}]", _options.NotificationTarget);
			}
		}

		async Task<List<Position>> FindCloseOportunitiesAsync(Currency from, Currency to, float rate)
		{
			var offset = rate > 0
				? rate + _options.CloseOffset
				: 1 / ((1f / rate) - _options.CloseOffset);
			return await _dataStore.Position.FindOpenPositionsAsync(from.ToString(), to.ToString(), offset);
		}
	}
}
