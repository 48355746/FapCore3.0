using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.Utility;
using Quartz.Impl;

namespace Fap.Core.Scheduler.Jobs
{
    /// <summary>
    /// 用来实现执行sql的定时任务
    /// </summary>
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class ExecSqlJob : IJob
    {
        Task IJob.Execute(IJobExecutionContext context)
        {
            string sql = context.JobDetail.JobDataMap.Get(JobConstants.JobExecSqlKey).ToString();
            IServiceProvider serviceProvider = context.JobDetail.JobDataMap.Get(JobConstants.JobIServiceProviderKey) as IServiceProvider;
            IDbContext _dbContext = serviceProvider.GetService<IDbContext>();
            ILogger<ExecSqlJob> _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<ExecSqlJob>();
            JobDetailImpl jobDetail = context.JobDetail as JobDetailImpl;
            var jobKey = jobDetail.Key;
            _logger.LogInformation("---{0} executing '{1}' at {2}", jobKey, sql, DateTime.Now.ToString("r"));
            try
            {
                if (sql.Contains("@Fid"))
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("Fid", UUIDUtils.Fid);
                    _dbContext.Execute(sql, param);
                }
                else
                {
                    _dbContext.Execute(sql);
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"--- Error in job!----{ex.Message}");
                _dbContext.Insert<FapJobLog>(new FapJobLog { JobId = jobKey.Name, JobName = jobDetail.Description, ExecuteTime = DateTimeUtils.CurrentDateTimeStr, ExecuteResult = "failure", Message=ex.Message });
                JobExecutionException e2 = new JobExecutionException(ex);
                // Quartz will automatically unschedule
                // all triggers associated with this job
                // so that it does not run again
                e2.UnscheduleAllTriggers = true;
                throw e2;
            }
            _dbContext.Insert<FapJobLog>(new FapJobLog { JobId = jobKey.Name, JobName = jobDetail.Description, ExecuteTime = DateTimeUtils.CurrentDateTimeStr, ExecuteResult = "success" });
            _logger.LogInformation("---{0} completed '{1}' at {2}", jobKey, sql, DateTime.Now.ToString("r"));
            return Task.FromResult<int>(0);



        }
    }
}
