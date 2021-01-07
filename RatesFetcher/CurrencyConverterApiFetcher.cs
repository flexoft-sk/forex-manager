using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Flexoft.ForexManager.RatesFetcher
{
    public class CurrencyConverterApiFetcher : IRates
    {
        private const string UrlBase = "https://free.currconv.com/api/v7/convert?apiKey={0}&q={1}_{2}&compact=y";

        private readonly ILogger<CurrencyConverterApiFetcher> _logger;
        private readonly RatesFetcherOptions _options;

        public CurrencyConverterApiFetcher(ILogger<CurrencyConverterApiFetcher> logger, RatesFetcherOptions options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<float> GetRateAsync(Currency from, Currency to)
        {
            using var client = new HttpClient();

            var url = string.Format(UrlBase, _options.ApiKey, from, to);
            var response = await client.GetStringAsync(url);

            var dataObj = JObject.Parse(response);

            var pairKey = $"{from}_{to}";
            if (dataObj.ContainsKey(pairKey) && dataObj[pairKey]["val"] != null 
                && float.TryParse(dataObj[pairKey]["val"].Value<string>(), out var rate))
            {
                return rate;
            }

            _logger.LogWarning($"Fetching {pairKey} [{url}] returned invalid response.");

            throw new FormatException($"Unexpected response: {response}");
        }
    }
}
