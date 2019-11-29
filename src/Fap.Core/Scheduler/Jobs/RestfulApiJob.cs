using Quartz;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz.Impl;
using Fap.Core.Utility;
using Fap.Core.DataAccess;

namespace Fap.Core.Scheduler.Jobs
{
    /// <summary>
    /// Restful Api Job
    /// </summary>
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class RestfulApiJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {            
            string url = context.JobDetail.JobDataMap.Get(JobConstants.JobRestfulApiKey).ToString();
            IServiceProvider serviceProvider = context.JobDetail.JobDataMap.Get(JobConstants.JobIServiceProviderKey) as IServiceProvider;
            ILogger<RestfulApiJob> _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<RestfulApiJob>();
            IDbContext _dbContext = serviceProvider.GetService<IDbContext>();
            JobDetailImpl jobDetail = context.JobDetail as JobDetailImpl;
            var jobKey = jobDetail.Key;
            _logger.LogInformation("---{0} executing '{1}' at {2}", jobKey,url, DateTime.Now.ToString("r"));
            try
            {
                var httpClientFact = serviceProvider.GetService<IHttpClientFactory>();
                var httpClient = httpClientFact.CreateClient();
                var responseMessage =await httpClient.GetAsync(url);
                responseMessage.EnsureSuccessStatusCode();
                _dbContext.Insert<FapJobLog>(new FapJobLog { JobId = jobKey.Name, JobName = jobDetail.Description, ExecuteTime = DateTimeUtils.CurrentDateTimeStr, ExecuteResult = "success",Message=$"{responseMessage.StatusCode.ToString()}--{(int)responseMessage.StatusCode}" });
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"--- Error in RestfulApiJob!-----{ex.Message}");
                _dbContext.Insert<FapJobLog>(new FapJobLog { JobId = jobKey.Name, JobName = jobDetail.Description, ExecuteTime = DateTimeUtils.CurrentDateTimeStr, ExecuteResult = "failure", Message = ex.Message });
                JobExecutionException e2 = new JobExecutionException(ex);
                // Quartz will automatically unschedule
                // all triggers associated with this job
                // so that it does not run again
                e2.UnscheduleAllTriggers = true;
                throw e2;
            }
            _logger.LogInformation("---{0} completed '{1}' at {1}", jobKey,url, DateTimeUtils.CurrentDateTimeStr);
        }
    }
}
