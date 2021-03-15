using Flexoft.ForexManager.BusinessLogic;
using Flexoft.ForexManager.NotificationManager;
using Flexoft.ForexManager.RatesFetcher;
using Flexoft.ForexManager.Store;
using Flexoft.ForexManager.Store.Contracts;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Tests
{
	public class DefaultRateEvaluatorTests
	{
		RateEvaluatorOptions _options;
		DefaultRateEvaluator _underTest;
		INotificationManager _notificationManager;
		IRates _rateFetcher;
		IPosition _position;

		[SetUp]
		public void Setup()
		{
			_options = new RateEvaluatorOptions
			{
				CloseOffset = 0.003f,
				NotificationTarget = "target@email.com",
				OpenAmount = 10,
				OpenHour = DateTime.UtcNow.Hour
			};
			var logger = Substitute.For<ILogger<DefaultRateEvaluator>>();
			_notificationManager = Substitute.For<INotificationManager>();
			_rateFetcher = Substitute.For<IRates>();
			_rateFetcher.GetRateAsync(Currency.EUR, Currency.USD).Returns(Task.FromResult(2f));

			_position = Substitute.For<IPosition>();

			var store = Substitute.For<IDataStore>();
			store.Position.Returns(_position);

			_underTest = new DefaultRateEvaluator(_options, logger, _notificationManager,  _rateFetcher, store);
		}

		[Test]
		public async Task PositionIsOpenedAtDefinedHourOnly()
		{
			_position.FindOpenPositionsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<float>()).Returns(Task.FromResult(new List<Position>()));

			await _underTest.EvaluateRateAsync(Currency.EUR, Currency.USD);

			await _position.Received().OpenAsync("EUR", "USD", 10f, 2f);

			_options.OpenHour = DateTime.UtcNow.AddHours(2).Hour;

			_position.ClearReceivedCalls();

			await _underTest.EvaluateRateAsync(Currency.EUR, Currency.USD);

			await _position.DidNotReceive().OpenAsync("EUR", "USD", 10f, 2f);
		}
	}
}