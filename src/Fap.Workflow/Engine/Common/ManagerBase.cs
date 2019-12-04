using Fap.Core.DataAccess;
using Microsoft.Extensions.Logging;

namespace Fap.Workflow.Engine.Common
{
    public class ManagerBase
    {
        protected IDbContext _dataAccessor;
        protected ILoggerFactory _loggerFactory;
        public ManagerBase(IDbContext dataAccessor,ILoggerFactory loggerFactory)
        {
            _dataAccessor = dataAccessor;
            _loggerFactory = loggerFactory;
        }
    }
}
