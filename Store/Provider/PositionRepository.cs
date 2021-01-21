using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Flexoft.ForexManager.Store.Contracts;
using Flexoft.ForexManager.Store.Provider.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flexoft.ForexManager.Store.Provider
{
    class PositionRepository : RepositoryBase, IPosition
    {
		readonly EfDataStore _contextProvider;

		public PositionRepository(EfDataStore contextProvider, ILogger logger) :
            base(contextProvider, logger)
		{
			_contextProvider = contextProvider;
		}

		public async Task OpenAsync(string from, string to, float amount, float rate)
		{
            if (string.IsNullOrEmpty(from))
			{
                throw new ArgumentNullException(nameof(from));
			}

            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentNullException(nameof(to));
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be a positive number", nameof(amount));
            }

            if (rate <= 0)
            {
                throw new ArgumentException("Rate must be a positive number", nameof(rate));
            }

            using var ctx = GetContext();
            ctx.Position.Add(new Contracts.Position 
            { 
                FromCurrency = from,
                ToCurrency = to,
                OpenAmount = amount,
                OpenRate = rate,
                OpenStamp = DateTime.UtcNow
            });

            await ctx.SaveChangesAsync();
        }

        public async Task<List<Position>> FindOpenPositionsAsync(string from, string to, float rateLimit) 
        {
            using var ctx = GetContext();

            return await ctx.Position.Where(p => !p.CloseAmount.HasValue && p.FromCurrency == from && p.ToCurrency == to && p.OpenRate < rateLimit)
                .ToListAsync();
        }
    }
}