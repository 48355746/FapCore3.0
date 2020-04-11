using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.DataAccess
{
    public class DbTransactionProxy
    {
        private readonly IDbContext _dbContext;
        private readonly ILogger<DbTransactionProxy> _logger;
        public DbTransactionProxy(IDbContext dbContext,ILogger<DbTransactionProxy> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public void DbTransaction(Action<IDbContext> action)
        {
            try
            {
                _dbContext.BeginTransaction();
                action?.Invoke(_dbContext);
                _dbContext.Commit();
            }
            catch (Exception ex)
            {
                _dbContext.Rollback();
                _logger.LogError(ex.Message);
            }


        }
    }
}
