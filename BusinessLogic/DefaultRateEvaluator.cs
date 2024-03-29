﻿using Flexoft.ForexManager.NotificationManager;
using Flexoft.ForexManager.RatesFetcher;
using Flexoft.ForexManager.Store;
using Flexoft.ForexManager.Store.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flexoft.ForexManager.BusinessLogic
{
	public class DefaultRateEvaluator : IRateEvaluator
	{
		private readonly ILogger<DefaultRateEvaluator> _logger;
		private readonly IRates _rateFatcher;
		private readonly IDataStore _dataStore;
		private readonly RateEvaluatorOptions _options;

		public DefaultRateEvaluator(RateEvaluatorOptions options, ILogger<DefaultRateEvaluator> logger, 
			INotificationManager notificationManager, IRates rateFatcher, IDataStore dataStore)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			NotificationManager = notificationManager ?? throw new ArgumentNullException(nameof(notificationManager));
			_rateFatcher = rateFatcher ?? throw new ArgumentNullException(nameof(rateFatcher));
			_dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
			_options = options ?? throw new ArgumentNullException(nameof(options));

			_options.CloseUIUrl += (_options.CloseUIUrl.Contains("?") ? "&" : "?");
		}

		public INotificationManager NotificationManager { get; }

		public string NotificationTarget => _options.NotificationTarget;

		public async Task EvaluateRateAsync(Currency from, Currency to, float rate) 
		{
			var reversedRate = 1f / rate;

			_logger.LogInformation($"Evaluating {from} -> {to} with: {rate} [{reversedRate}]");

			var closeOportunities = await FindCloseOportunitiesAsync(from, to, rate);

			_logger.LogInformation($"Found opportunities for {from} -> {to}: {string.Join(",", closeOportunities.Select(p => p.Id))}");

			var reverseOpportunities = await FindCloseOportunitiesAsync(to, from, reversedRate);
			_logger.LogInformation($"Found opportunities for {to} -> {from}: {string.Join(",", reverseOpportunities.Select(p => p.Id))}");
			closeOportunities.AddRange(reverseOpportunities);

			var notification = string.Join("<br>", closeOportunities.Select(p => $"<a href=\"{_options.CloseUIUrl}id={p.Id}\">[{p.Id}]</a> {p.FromCurrency} -> {p.ToCurrency} : {p.OpenAmount} for {p.OpenRate}. Proposal: {p.OpenAmount * p.OpenRate} [{rate} - {reversedRate}]"));

			if (!string.IsNullOrEmpty(notification))
			{
				NotificationManager.Notify("Opportunities", notification, NotificationTarget);
			}
		}

		public async Task EvaluateRateAsync(Currency from, Currency to)
		{
			var now = DateTime.UtcNow;
			if (now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday)
			{
				return;
			}

			var rate = await _rateFatcher.GetRateAsync(from, to);
			var reversedRate = 1f / rate;

			await EvaluateRateAsync(from, to, rate);

			if (now.Hour == _options.OpenHour)
			{
				var reversedAmount = _options.OpenAmount / reversedRate;
				var openedId = await _dataStore.Position.OpenAsync(from.ToString(), to.ToString(), _options.OpenAmount, rate);
				var reverseOpenedId = await _dataStore.Position.OpenAsync(to.ToString(), from.ToString(), reversedAmount, reversedRate);

				_logger.LogInformation($"Opened: {from} -> {to} for {rate}");
				_logger.LogInformation($"Opened: {to} -> {from} for {reversedRate}");

				NotificationManager.Notify("Opened", $"[{openedId}/{reverseOpenedId}] {from} -> {to} : {rate} [{reversedRate}]", NotificationTarget);
			}
		}

		public async Task<(double amount, string currency)> CloseAsync(int id, double rate, double? fee) 
		{
			var position = await _dataStore.Position.GetPositionAsync(id);

			if (position.CloseStamp.HasValue)
			{
				throw new InvalidOperationException($"Position {id} is already closed.");
			}

			var toSell = position.OpenAmount * position.OpenRate;
			var bought = toSell * rate;

			var diff = await _dataStore.Position.CloseAsync(id, bought, rate, fee);

			_logger.LogInformation($"Position {id} closed with diff {diff}");

			return (bought, position.FromCurrency);
		}

		async Task<List<Position>> FindCloseOportunitiesAsync(Currency from, Currency to, float rate)
		{
			var offset = rate * (1 + (_options.CloseOffsetPercentage * 0.01f ));
			return await _dataStore.Position.FindOpenPositionsAsync(from.ToString(), to.ToString(), offset);
		}
	}
}
