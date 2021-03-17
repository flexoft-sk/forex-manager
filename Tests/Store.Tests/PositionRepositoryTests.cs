using Flexoft.ForexManager.Store;
using Flexoft.ForexManager.Store.Provider;
using Flexoft.ForexManager.Store.Provider.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Tests
{
    public class PositionRepositoryTests
    {
        IPosition _underTest;
        ForexStoreContext _dbContext;
        ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<ILogger>();
            _dbContext = new ForexStoreContext();
            _underTest = new EfDataStore(_dbContext, _logger, new StoreOptions()).Position;
        }

        [Test]
        public async Task OpenWritesExpectedFields()
        {
            var beforeWriteStamp = DateTime.UtcNow;

            await _underTest.OpenAsync("EUR", "USD", 200, 1.22f);

            var afterWriteStamp = DateTime.UtcNow;

            var openPositions = _dbContext.Position.ToList();
            openPositions.Count.Should().Be(1);

            var position = openPositions[0];

            position.Id.Should().BeGreaterThan(0);
            position.FromCurrency.Should().Be("EUR");
            position.ToCurrency.Should().Be("USD");
            position.OpenAmount.Should().Be(200);
            position.OpenRate.Should().Be(1.22f);
            position.OpenStamp.Should().BeOnOrAfter(beforeWriteStamp);
            position.OpenStamp.Should().BeOnOrBefore(afterWriteStamp);
        }

        [Test]
        public async Task FindOpenPositionsWorksCorrectly()
        {
            var p1 = await _underTest.OpenAsync("EUR", "USD", 200, 1.2f);
            var p2 = await _underTest.OpenAsync("EUR", "USD", 200, 1.2f);
            var p3 = await _underTest.OpenAsync("EUR", "USD", 200, 1.3f);
            var p4 = await _underTest.OpenAsync("EUR", "USD", 200, 1.3f);

            await _underTest.CloseAsync(p2, 250, 2, null);
            await _underTest.CloseAsync(p4, 250, 2, null);

            var positions = await _underTest.FindOpenPositionsAsync("EUR", "USD", 1.25f);

            positions.Count.Should().Be(1, "there are just two open and only one from them fits");
            positions[0].Id.Should().Be(p3, "only p3 is open and opened higher");
        }

        [Test]
        public async Task PositionCanBeClosedJustOnce()
        {
            var id = await _underTest.OpenAsync("EUR", "USD", 200, 1.2f);
            await _underTest.CloseAsync(id, 250, 2, null);

            _underTest.Invoking(async ut => await ut.CloseAsync(id, 250, 2, null)).Should().Throw<InvalidOperationException>();
        }
    }
}