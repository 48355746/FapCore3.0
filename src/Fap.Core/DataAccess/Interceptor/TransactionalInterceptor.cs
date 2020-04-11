using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Fap.Core.DataAccess.Interceptor
{
    public class TransactionalInterceptor : StandardInterceptor
    {
        private IDbContext _dbContext;
        private ILogger<TransactionalInterceptor> _logger;
        Stopwatch stopwatch = new Stopwatch();
        public TransactionalInterceptor(IDbContext dbContext, ILogger<TransactionalInterceptor> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        protected override void PreProceed(IInvocation invocation)
        {
            stopwatch.Restart();
            var method = invocation.MethodInvocationTarget;
            if (method?.GetCustomAttribute<TransactionalAttribute>() != null)
            {
                //_logger.LogInformation($"{invocation.Method.Name}事务拦截前");

                _dbContext.BeginTransaction();
            }
        }
        protected override void PerformProceed(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                var method = invocation.MethodInvocationTarget;
                if (method?.GetCustomAttribute<TransactionalAttribute>() != null)
                {
                    _logger.LogError($"{invocation.Method.Name}事务拦截后异常:{ex.Message}");
                    _dbContext.Rollback();
                }
                throw ex;
            }
        }
        protected override void PostProceed(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget;
            if (method?.GetCustomAttribute<TransactionalAttribute>() != null)
            {
                //_logger.LogInformation($"{invocation.Method.Name}事务拦截后");
                _dbContext.Commit();
            }
            stopwatch.Stop();
            _logger.LogInformation($"{method}执行时间为：{stopwatch.ElapsedMilliseconds}毫秒");
        }

    }
}
