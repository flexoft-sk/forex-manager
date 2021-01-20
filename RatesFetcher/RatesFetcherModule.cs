using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flexoft.ForexManager.RatesFetcher
{
    public static class RatesFetcherModule
    {
        public static void RegisterRatesFetcher(this IServiceCollection services)
        {
            services.AddSingleton<IRates, CurrencyConverterApiFetcher>();
            services.AddSingleton(provider => {
                var config = provider.GetService<IConfiguration>();
                return new RatesFetcherOptions
                {
                    ApiKey = config["RatesFetcher:ApiKey"],
                };
            });
        }
    }
}
