using System;
using System.IO;
using System.Reflection;
using DbUp;
using DbUp.Engine.Output;
using Flexoft.ForexManager.Store.Provider;
using Microsoft.Extensions.Logging;

namespace Flexoft.ForexManager.Store
{
    public class DataStoreFactory
    {
        readonly ILogger _logger;

        public DataStoreFactory(ILogger logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public IDataStore GetDataStore(StoreOptions options)
        {
            EnsureRelationalDbUpToDate(options.ConnectionString);
            return new EfDataStore(options, _logger);
        }

        void EnsureRelationalDbUpToDate(string connectionString)
        {
            //_logger.LogDebug($"ConnectionString: {connectionString}");
            _logger.LogInformation($"Checking DB for state with embedded scripts");

            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithTransaction()
                    //.WithScriptsFromFileSystem(scriptFolderPath)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), s => s.EndsWith("sql"))
                    .LogTo(new UpgradeLogger(_logger))
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new OperationCanceledException($"Failed to update DB due following reason: {result.Error}");
            }
        }

        class UpgradeLogger : IUpgradeLog
        {
            readonly ILogger _logger;

            public UpgradeLogger(ILogger logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            public void WriteInformation(string format, params object[] args)
            {
                _logger.LogInformation(format, args);
            }

            public void WriteError(string format, params object[] args)
            {
                _logger.LogError(format, args);
            }

            public void WriteWarning(string format, params object[] args)
            {
                _logger.LogWarning(format, args);
            }
        }
    }
}