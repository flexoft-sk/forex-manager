using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Flexoft.ForexManager.Store
{
    public static class DataStoreModule
    {
        public static void RegisterStore(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var config = provider.GetService<IConfiguration>();
                var storeOptions = new StoreOptions { ConnectionString = config["Store:ConnectionString"] };
                var logger = provider.GetService<ILogger<DataStoreFactory>>();

                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetry(3, r => TimeSpan.FromSeconds(5));

                IDataStore store = null;
                retryPolicy.Execute(() => store = new DataStoreFactory(logger).GetDataStore(storeOptions));

                return store;
            });
        }
    }
}