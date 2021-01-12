using System;
using Flexoft.ForexManager.Store.Provider.Entities;
using Microsoft.Extensions.Logging;

namespace Flexoft.ForexManager.Store.Provider
{
    abstract class RepositoryBase
    {
        readonly EfDataStore _contextProvider;

        /// <summary>Initializes a new instance of the <see cref="RepositoryBase" /> class.</summary>
        /// <param name="contextProvider">The context provider.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">contextProvider</exception>
        protected RepositoryBase(EfDataStore contextProvider, ILogger logger)
        {
            _contextProvider = contextProvider ?? throw new ArgumentNullException(nameof(contextProvider));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>Gets the logger.</summary>
        /// <value>The logger.</value>
        protected ILogger Logger { get; }

        protected StoreOptions Options => _contextProvider.Options;

        /// <summary>Creates new context.</summary>
        /// <returns>New DocumentSealingContext instance</returns>
        protected ForexStoreContext GetContext() => _contextProvider.GetContext();
    }
}