using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.IO;
using System.Threading.Tasks;
using Fap.Core.Annex.Utility.TempFile;
using Fap.Core.Utility;
using Quartz.Impl;
using Fap.Core.DataAccess;

namespace Fap.Core.Scheduler.Jobs
{
    /// <summary>
    /// 任务调度：定时删除临时文件
    /// </summary>
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class TempFileDeleteJob : IJob
    {
        Task IJob.Execute(IJobExecutionContext context)
        {
            IServiceProvider serviceProvider = context.JobDetail.JobDataMap.Get(JobConstants.JobIServiceProviderKey) as IServiceProvider;

            IDbContext _dbContext = serviceProvider.GetService<IDbContext>();
            ILogger<ExecSqlJob> _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<ExecSqlJob>();
            JobDetailImpl jobDetail = context.JobDetail as JobDetailImpl;
            var jobKey = jobDetail.Key;
            _logger.LogInformation($"---{jobKey}executing at {DateTimeUtils.CurrentDateTimeStr}");
            try
            {
                FileUtility.DeleteYestodayTemporaryFolder();
            }
            catch (Exception ex)
            {
                _dbContext.Insert<FapJobLog>(new FapJobLog { JobId = jobKey.Name, JobName = jobDetail.Description, ExecuteTime = DateTimeUtils.CurrentDateTimeStr, ExecuteResult = "failure", Message = ex.Message });
                _logger.LogInformation($"---{jobKey} Error in job!---{ex.Message}");
            }
            _dbContext.Insert<FapJobLog>(new FapJobLog { JobId = jobKey.Name, JobName = jobDetail.Description, ExecuteTime = DateTimeUtils.CurrentDateTimeStr, ExecuteResult = "success" });
            _logger.LogInformation($"---{jobKey} completed at {DateTimeUtils.CurrentDateTimeStr}");
            return Task.FromResult(true);
        }
    }
}
