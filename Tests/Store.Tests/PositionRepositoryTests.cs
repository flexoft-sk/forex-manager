using Flexoft.ForexManager.Store;
using Flexoft.ForexManager.Store.Provider;
using Flexoft.ForexManager.Store.Provider.Entities;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

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
        public void Test1()
        {
            Assert.Pass();
        }
    }
}