using System;
using System.Runtime.CompilerServices;
using Flexoft.ForexManager.Store.Provider.Entities;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Flexoft.ForexManager.Store.Tests")]

namespace Flexoft.ForexManager.Store.Provider
{
    class EfDataStore : IDataStore
    {
        readonly Lazy<IPosition> _transaction;
        readonly ILogger _logger;
        readonly ForexStoreContext _outerContext;

        /// <summary>Prevents a default instance of the <see cref="EfDataStore" /> class from being created.</summary>
        EfDataStore()
        {
            _transaction = new Lazy<IPosition>(() => new PositionRepository(this, _logger));
        }

        /// <summary>Initializes a new instance of the <see cref="EfDataStore" /> class.</summary>
        /// <param name="outerContext">The outer context.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">
        ///     logger
        ///     or
        ///     outerContext
        /// </exception>
        /// <remarks>Creates InMemory store</remarks>
        public EfDataStore(ForexStoreContext outerContext, ILogger logger, StoreOptions options) : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _outerContext = outerContext ?? throw new ArgumentNullException(nameof(outerContext));
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>Initializes a new instance of the <see cref="EfDataStore" /> class.</summary>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">
        ///     logger
        ///     or
        ///     connectionString
        /// </exception>
        public EfDataStore(StoreOptions options, ILogger logger) : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Options = options ?? throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(Options.ConnectionString))
            {
                throw new ArgumentNullException(nameof(Options.ConnectionString));
            }
        }

        /// <summary>Gets the options.</summary>
        /// <value>The options.</value>
        public StoreOptions Options { get; }

        /// <summary>Gets the session.</summary>
        /// <value>The session.</value>
        public IPosition Transaction => _transaction.Value;

        /// <summary>Gets the context.</summary>
        /// <returns>ForexStoreContext instance</returns>
        public ForexStoreContext GetContext() => _outerContext ?? new ForexStoreContext(Options.ConnectionString);
    }
}