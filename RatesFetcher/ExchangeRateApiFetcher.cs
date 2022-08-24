using Flexoft.ForexManager.RatesFetcher.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Flexoft.ForexManager.RatesFetcher
{
    public class ExchangeRateApiFetcher : IRates
    {
        const string UrlBase = "https://api.exchangerate.host/latest?base=__from__&symbols=__to__";
        readonly ILogger<ExchangeRateApiFetcher> _logger;

        public ExchangeRateApiFetcher(ILogger<ExchangeRateApiFetcher> logger)
        {
            _logger = logger;
        }

        public async Task<float> GetRateAsync(Currency from, Currency to)
        {
            var url = UrlBase.Replace("__from__", from.ToString()).Replace("__to__", to.ToString());

            using var client = new HttpClient();

            var response = await client.GetStringAsync(url);

            var model = JsonConvert.DeserializeObject<ExchangeRateApiResponse>(response);

            if (model.Success && model.Rates?.ContainsKey(to.ToString()) == true)
            {
                return model.Rates[to.ToString()];
            }
            else 
            {
                _logger.LogWarning($"Fetching currency rate from {url} was not successful. -> {response}");
            }

            throw new FormatException($"Unexpected response: {response}");
        }
    }
}
