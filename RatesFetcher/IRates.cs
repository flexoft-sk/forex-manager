using System.Threading.Tasks;

namespace Flexoft.ForexManager.RatesFetcher
{
	/// <summary> Provides functionality related to rates</summary>
	public interface IRates
    {
		/// <summary>Gets the current rate for two currencies.</summary>
		/// <param name="from">Source currency.</param>
		/// <param name="to">Target currency.</param>
		/// <returns></returns>
		Task<float> GetRateAsync(Currency from, Currency to);
    }
}
