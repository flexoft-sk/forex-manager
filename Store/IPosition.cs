using Flexoft.ForexManager.Store.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flexoft.ForexManager.Store
{
    public interface IPosition
    {
        Task OpenAsync(string from, string to, float amount, float rate);
		Task<List<Position>> FindOpenPositionsAsync(string from, string to, float rateLimit);
	}
}
