using Flexoft.ForexManager.Store.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flexoft.ForexManager.Store
{
	/// <summary> Provides position related functionality</summary>
	public interface IPosition
    {
		/// <summary>Opens a new position.</summary>
		/// <param name="from">Source currency.</param>
		/// <param name="to">Target currency.</param>
		/// <param name="amount">The amount.</param>
		/// <param name="rate">The rate.</param>
		/// <returns>Position identifier</returns>
		Task<int> OpenAsync(string from, string to, float amount, float rate);

		/// <summary>Finds the open positions.</summary>
		/// <param name="from">Source currency.</param>
		/// <param name="to">Target currency.</param>
		/// <param name="rateLimit">The rate limit.</param>
		/// <returns>Found positions</returns>
		Task<List<Position>> FindOpenPositionsAsync(string from, string to, float rateLimit);

		/// <summary>Gets the position by identifier.</summary>
		/// <param name="id">The identifier.</param>
		/// <returns></returns>
		Task<Position> GetPositionAsync(int id);

		/// <summary>Closes a position.</summary>
		/// <param name="id">The position identifier.</param>
		/// <param name="amount">The amount.</param>
		/// <param name="rate">The rate.</param>
		/// <param name="fee">The fee.</param>
		/// <returns>The amount in source currency after the close</returns>
		Task<double> CloseAsync(int id, double amount, double rate, double? fee);
	}
}
