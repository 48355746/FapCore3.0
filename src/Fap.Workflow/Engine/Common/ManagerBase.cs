using Fap.Core.DataAccess;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Fap.Workflow.Engine.Common
{
    public class ManagerBase
    {
        protected IDbContext _dataAccessor;
        protected ILoggerFactory _loggerFactory;
        protected IServiceProvider _serviceProvider;
        public ManagerBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _dataAccessor =serviceProvider.GetService<IDbContext>();
            _loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        }
    }
}
