using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Flexoft.ForexManager.ForexManager.Startup))]

// https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection#working-with-options-and-settings

namespace Flexoft.ForexManager.ForexManager
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            
        }
    }
}