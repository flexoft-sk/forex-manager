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

		public async Task<int> OpenAsync(string from, string to, float amount, float rate)
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
            var position = new Contracts.Position
            {
                FromCurrency = from,
                ToCurrency = to,
                OpenAmount = amount,
                OpenRate = rate,
                OpenStamp = DateTime.UtcNow
            };
            ctx.Position.Add(position);

            await ctx.SaveChangesAsync();
            return position.Id;
        }

        public async Task<List<Position>> FindOpenPositionsAsync(string from, string to, float rateLimit) 
        {
            using var ctx = GetContext();

            return await ctx.Position.Where(p => !p.CloseAmount.HasValue && p.FromCurrency == from && p.ToCurrency == to && p.OpenRate > rateLimit)
                .ToListAsync();
        }

        public async Task<Position> GetPositionAsync(int id)
        {
            using var ctx = GetContext();

            var position = await ctx.Position.AsNoTracking().SingleOrDefaultAsync(predicate => predicate.Id == id);

            if (position == null)
            {
                throw new ArgumentException("Position does not exist", nameof(id));
            }

            return position;
        }

        public async Task<double> CloseAsync(int id, double amount, double rate, double? fee) 
        {
            using var outerCtx = GetContext();
            var strategy = outerCtx.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var ctx = GetContext();
                var txn = await ctx.Database.BeginTransactionAsync();

                try
                {
                    var position = await ctx.Position.SingleAsync(predicate => predicate.Id == id);

                    if (position.CloseAmount.HasValue)
                    {
                        throw new InvalidOperationException($"Position {id} is already closed.");
                    }

                    position.CloseAmount = amount;
                    position.CloseRate = rate;
                    position.CloseStamp = DateTime.UtcNow;
                    position.Fee = fee;

                    position.Diff = position.CloseAmount - position.OpenAmount - (fee ?? 0);

                    await ctx.SaveChangesAsync();
                    await txn.CommitAsync();

                    return position.Diff.Value;
                }
                catch
                {
                    // no need to await here
                    try { txn?.RollbackAsync(); } catch { }
                    throw;
                }
            });
        }
    }
}