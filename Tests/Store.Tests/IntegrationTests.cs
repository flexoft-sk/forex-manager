using Flexoft.ForexManager.Store;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Store.Tests
{
    public class IntegrationTests
    {
        //[Test]
        public void DbCreationTest()
        {
            var serviceCollection = new ServiceCollection();

            var configuration = Substitute.For<IConfiguration>();
            configuration["Store:ConnectionString"].Returns("Server=localhost\\EX2014;Database=TestDb;User ID=;Password=;ConnectRetryCount=0;Integrated Security=True");

            var logger = Substitute.For<ILogger<DataStoreFactory>>();

            serviceCollection.AddSingleton(logger);
            serviceCollection.AddSingleton<IConfiguration>(configuration);

            serviceCollection.RegisterStore();

            var provider = serviceCollection.BuildServiceProvider();

            var store = provider.GetService(typeof(IDataStore)) as IDataStore;

            store.Should().NotBeNull();
        }
    }
}
