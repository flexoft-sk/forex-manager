using Flexoft.ForexManager.Store.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flexoft.ForexManager.Store.Provider.Entities
{
    public class ForexStoreContext : DbContext
    {
        readonly string _connectionString;
        readonly bool _isInMemory;

        public ForexStoreContext() => _isInMemory = true;


        public ForexStoreContext(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException(nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public virtual DbSet<Position> Position { get; set; }

        public override void Dispose()
        {
            if (!_isInMemory)
            {
                base.Dispose();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (_isInMemory)
            {
                optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
                optionsBuilder.ConfigureWarnings(bldr => bldr.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            }
            else
            {
                optionsBuilder.UseSqlServer(_connectionString,
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            10,
                            TimeSpan.FromSeconds(10),
                            null);
                    });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Position>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FromCurrency)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ToCurrency)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OpenStamp).HasColumnType("datetime");

                entity.Property(e => e.CloseStamp).HasColumnType("datetime");
            });
        }
    }
}
