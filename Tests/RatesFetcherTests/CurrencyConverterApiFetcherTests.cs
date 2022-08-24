using Flexoft.ForexManager.RatesFetcher;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RatesFetcherTests
{
    public class CurrencyConverterApiFetcherTests
    {
        private ILogger<CurrencyConverterApiFetcher> _logger;
        private RatesFetcherOptions _options;
        private CurrencyConverterApiFetcher _underTest;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<ILogger<CurrencyConverterApiFetcher>>();
            _options = new RatesFetcherOptions
            {
                ApiKey = "your_key_goes_here"
            };

            _underTest = new CurrencyConverterApiFetcher(_logger, _options, null);
        }

        //[Test]
        public async Task IntegrationTest()
        {
            var rate = await _underTest.GetRateAsync(Currency.EUR, Currency.USD);
            rate.Should().BeGreaterThan(0.9f);
            rate.Should().BeLessThan(1.5f);

            rate = await _underTest.GetRateAsync(Currency.USD, Currency.EUR);
            rate.Should().BeGreaterThan(0.5f);
            rate.Should().BeLessThan(1.2f);
        }

        //[Test]
        public async Task IntegrationTestFallback()
        {
            // will succeed only when free.currconv.com is down (which happens quite often)
            var fallbackLogger = Substitute.For<ILogger<ExchangeRateApiFetcher>>();
            var fallbackFetcher = new ExchangeRateApiFetcher(fallbackLogger);

            var underTest = new CurrencyConverterApiFetcher(_logger, _options, fallbackFetcher);

            // it is enough if this doesn't throw exception
            await underTest.GetRateAsync(Currency.EUR, Currency.USD);
        }

        [Test]
        public void InvalidKeyCausesException()
        {
            _options.ApiKey = "XY";
            _underTest = new CurrencyConverterApiFetcher(_logger, _options, null);

            _underTest.Invoking(async ut => await ut.GetRateAsync(Currency.EUR, Currency.USD)).Should().Throw<HttpRequestException>();
        }
    }
}