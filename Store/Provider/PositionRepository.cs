using System;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flexoft.ForexManager.Store.Provider
{
    class PositionRepository : RepositoryBase, IPosition
    {
        public PositionRepository(EfDataStore contextProvider, ILogger logger) :
            base(contextProvider, logger)
        { }
    }
}