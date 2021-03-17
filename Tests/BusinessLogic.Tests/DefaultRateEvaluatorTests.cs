using Flexoft.ForexManager.BusinessLogic;
using Flexoft.ForexManager.NotificationManager;
using Flexoft.ForexManager.RatesFetcher;
using Flexoft.ForexManager.Store;
using Flexoft.ForexManager.Store.Contracts;
using FluentAssertions;
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
		const string TargetEmail = "target@email.com";

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
				NotificationTarget = TargetEmail,
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

		[Test]
		public async Task OpenNotificationIsSent()
		{
			_position.FindOpenPositionsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<float>()).Returns(Task.FromResult(new List<Position>()));
			await _underTest.EvaluateRateAsync(Currency.EUR, Currency.USD);

			_notificationManager.Received().Notify("Opened", Arg.Any<string>(), TargetEmail);
		}

		[Test]
		public async Task NotificationAboutOpportunitiesIsSent()
		{
			_position.FindOpenPositionsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<float>()).Returns(Task.FromResult(new List<Position> {
				new Position
				{
					Id = 567,
					OpenAmount = 200,
					OpenRate = 1.191,
					FromCurrency = "EUR",
					ToCurrency = "USD",
					OpenStamp = DateTime.UtcNow
				} }));

			string opportunityDetail = null;
			_notificationManager.Notify("Opportunities", Arg.Do<string>(a => opportunityDetail = a), TargetEmail);

			await _underTest.EvaluateRateAsync(Currency.EUR, Currency.USD);

			opportunityDetail.Should().NotBeNull("there should be a notification");
			opportunityDetail.Should().Contain("567", "the position is an opportunity");
		}

		[Test]
		public async Task OffsetIsDynamiccalyComputed()
		{
			_rateFetcher.GetRateAsync(Currency.EUR, Currency.USD).Returns(Task.FromResult(1.195f));
			_position.FindOpenPositionsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<float>()).Returns(Task.FromResult(new List<Position>()));

			float offsetOfReverted = float.NaN;
			float offset = float.NaN;

			await _position.FindOpenPositionsAsync("EUR", "USD", Arg.Do<float>(a => offset = a));
			await _position.FindOpenPositionsAsync("USD", "EUR", Arg.Do<float>(a => offsetOfReverted = a));

			await _underTest.EvaluateRateAsync(Currency.EUR, Currency.USD);

			offset.Should().NotBe(float.NaN, "offset needs to be evaluated");
			// this is needed because of precision (1.195f + 0.003f = 1.1980000007)
			offset.Should().BeInRange(1.198f, 1.1981f, "offset is equal to configured one");

			offsetOfReverted.Should().NotBe(float.NaN, "reverted offset needs to be evaluated");
			offsetOfReverted.Should().BeInRange(0.8369f, 0.8398f, "reverted offset is less than non-reverted");
		}

		[Test]
		public void PositionCanBeClosedJustOnce()
		{
			_position.GetPositionAsync(22).Returns(new Position 
			{ 
				CloseStamp = DateTime.UtcNow
			});

			_underTest.Invoking(ut => ut.CloseAsync(22, 2, null)).Should().Throw<InvalidOperationException>();
		}

		[Test]
		public async Task ClosingPositionComputesTheCloseValuesCorrectly()
		{
			_position.GetPositionAsync(22).Returns(new Position {
				Id = 22,
				OpenAmount = 200,
				OpenRate = 2,
				FromCurrency = "EUR",
				ToCurrency = "USD",
				OpenStamp = DateTime.UtcNow
			});

			var result = await _underTest.CloseAsync(22, 1, 0.5f);

			await _position.Received().CloseAsync(22, 400, 1, 0.5);

			result.currency.Should().Be("EUR", "that was position origin");
			result.amount.Should().Be(400, "we bought that at position start");
		}
	}
}