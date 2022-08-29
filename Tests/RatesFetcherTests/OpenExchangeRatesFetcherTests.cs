using Flexoft.ForexManager.RatesFetcher;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System.Threading.Tasks;

namespace RatesFetcherTests
{
    public class OpenExchangeRatesFetcherTests
    {
        private ILogger<OpenExchangeRatesFetcher> _logger;
        private OpenExchangeRatesFetcher _underTest;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<ILogger<OpenExchangeRatesFetcher>>();

            var options = new RatesFetcherOptions
            {
                AltApiKey = "your key here"
            };

            _underTest = new OpenExchangeRatesFetcher(_logger, options);
        }

        [Test]
        public async Task IntegrationTest()
        {
            var rate = await _underTest.GetRateAsync(Currency.EUR, Currency.USD);

            rate.Should().BeGreaterThan(0.9f);
            rate.Should().BeLessThan(1.5f);
        }
    }
}
