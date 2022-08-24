using System.Collections.Generic;

namespace Flexoft.ForexManager.RatesFetcher.Model
{
    class ExchangeRateApiResponse
    {
        public bool Success { get; set; }

        public Dictionary<string, float> Rates { get; set; }
    }
}
