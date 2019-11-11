using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.DataAccess
{
    public class DbTransactionProxy
    {
        private readonly DbContext _dbContext;
        private readonly ILogger<DbTransactionProxy> _logger;
        public DbTransactionProxy(DbContext dbContext,ILogger<DbTransactionProxy> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public void DbTransaction(Action<DbContext> action)
        {
            try
            {
                _dbContext.BeginTransaction();
                action?.Invoke(_dbContext);
                _dbContext.CommitTransaction();
            }
            catch (Exception ex)
            {
                _dbContext.RollbackTransction();
                _logger.LogError(ex.Message);
            }
            finally
            {
                _dbContext.Dispose();
            }


        }
    }
}
