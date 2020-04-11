using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Threading.Tasks;
using Dapper;
using Fap.Core.Utility;
using Fap.Core.DataAccess;
using Quartz.Impl;

namespace Fap.Core.Scheduler.Jobs
{
    /// <summary>
    /// 任务调度：定时删除一个月前的日志
    /// </summary>
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class SyslogClearJob : IJob
    {
        /// <summary>
        /// 定时删除一个月前的日志
        /// </summary>
        /// <param name="context"></param>   
        Task IJob.Execute(IJobExecutionContext context)
        {
            IServiceProvider serviceProvider = context.JobDetail.JobDataMap.Get(JobConstants.JobIServiceProviderKey) as IServiceProvider;

            ILoggerFactory logFactory = serviceProvider.GetService<ILoggerFactory>();
            ILogger<SyslogClearJob> _logger = logFactory.CreateLogger<SyslogClearJob>();
            IDbContext _dbContext = serviceProvider.GetService<IDbContext>();
            JobDetailImpl jobDetail = context.JobDetail as JobDetailImpl;
            var jobKey = jobDetail.Key;
            _logger.LogInformation($"---{jobKey}executing at {DateTimeUtils.CurrentDateTimeStr}");
            try
            {
                DateTime dt = DateTime.Now.AddDays(-7);
                DynamicParameters param = new DynamicParameters();
                param.Add("Date", DateTimeUtils.DateTimeFormat(dt));
                string sql = "DELETE FROM FapSyslog WHERE [Date]<=@Date";
                int c = _dbContext.Execute(sql, param);
                _dbContext.Insert<FapJobLog>(new FapJobLog { JobId = jobKey.Name, JobName = jobDetail.Description, ExecuteTime = DateTimeUtils.CurrentDateTimeStr, ExecuteResult = "success", Message = $"清理行数:{c}" });
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"---{jobKey} Error in job!---{ex.Message}");
                _dbContext.Insert<FapJobLog>(new FapJobLog { JobId = jobKey.Name, JobName = jobDetail.Description, ExecuteTime = DateTimeUtils.CurrentDateTimeStr, ExecuteResult = "failure", Message = ex.Message });
            }
            _logger.LogInformation($"---{jobKey} completed at {DateTimeUtils.CurrentDateTimeStr}");
            return Task.FromResult<int>(0);


        }
    }
}
