using System.Threading.Tasks;

namespace Flexoft.ForexManager.RatesFetcher
{
    public interface IRates
    {
        Task<float> GetRateAsync(Currency from, Currency to);
    }
}
