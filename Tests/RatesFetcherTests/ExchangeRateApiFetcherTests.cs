using Flexoft.ForexManager.RatesFetcher;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System.Threading.Tasks;

namespace RatesFetcherTests
{
    public class ExchangeRateApiFetcherTests
    {
        private ILogger<ExchangeRateApiFetcher> _logger;
        private ExchangeRateApiFetcher _underTest;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<ILogger<ExchangeRateApiFetcher>>();
            _underTest = new ExchangeRateApiFetcher(_logger);
        }

        //[Test]
        public async Task IntegrationTest()
        {
            var rate = await _underTest.GetRateAsync(Currency.EUR, Currency.USD);

            rate.Should().BeGreaterThan(0.9f);
            rate.Should().BeLessThan(1.5f);

            // TBD - at the time of writing this test the service didn't support conversion from USD 
            //rate = await _underTest.GetRateAsync(Currency.USD, Currency.EUR);
            //rate.Should().BeGreaterThan(0.5f);
            //rate.Should().BeLessThan(1.2f);
        }
    }
}
