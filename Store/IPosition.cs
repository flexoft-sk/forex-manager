using Flexoft.ForexManager.Store.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flexoft.ForexManager.Store
{
	public interface IPosition
    {
        Task OpenAsync(string from, string to, float amount, float rate);
		Task<List<Position>> FindOpenPositionsAsync(string from, string to, float rateLimit);
		Task<Position> GetPositionAsync(int id);
		Task<double> CloseAsync(int id, double amount, double rate, double? fee);
	}
}
