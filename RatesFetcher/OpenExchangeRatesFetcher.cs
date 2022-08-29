using Flexoft.ForexManager.RatesFetcher.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Flexoft.ForexManager.RatesFetcher
{
    public class OpenExchangeRatesFetcher : IRates
    {
        private const string UrlBase = "https://openexchangerates.org/api/latest.json?app_id={0}&symbols={1}";

        private readonly ILogger<OpenExchangeRatesFetcher> _logger;
        private readonly RatesFetcherOptions _options;

        public OpenExchangeRatesFetcher(ILogger<OpenExchangeRatesFetcher> logger, RatesFetcherOptions options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<float> GetRateAsync(Currency from, Currency to)
        {
            using var client = new HttpClient();

            if (to != Currency.USD)
            {
                throw new NotSupportedException("This provider supports only USD as base");
            }

            var url = string.Format(UrlBase, _options.AltApiKey, from, to);

            var response = await client.GetStringAsync(url);

            var model = JsonConvert.DeserializeObject<OpenExchangeRateModel>(response);

            if (model.Rates?.ContainsKey(from.ToString()) == true)
            {
                return 1f / model.Rates[from.ToString()];
            }

            throw new FormatException($"Unexpected response: {response}");
        }
    }
}
