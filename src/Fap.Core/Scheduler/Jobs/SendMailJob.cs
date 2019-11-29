using Microsoft.Extensions.Logging;
using Quartz;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fap.Core.Message.Mail.Infrastructure.Internal;
using Fap.Core.Message.Mail.Core;
using Fap.Core.Message.Mail;
using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using MimeKit.Text;
using Fap.Core.Annex;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Infrastructure.Config;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Message;
using Fap.Core.Utility;
using Quartz.Impl;

namespace Fap.Core.Scheduler.Jobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class SendMailJob : IJob
    {
        private IMessageService _messageService;

        public Task Execute(IJobExecutionContext context)
        {
            IServiceProvider serviceProvider = context.JobDetail.JobDataMap.Get(JobConstants.JobIServiceProviderKey) as IServiceProvider;
            ILogger<SendMailJob> _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<SendMailJob>();
            IDbContext _dbContext = serviceProvider.GetService<IDbContext>();
            JobDetailImpl jobDetail = context.JobDetail as JobDetailImpl;
            var jobKey = jobDetail.Key;
            _logger.LogInformation($"---{jobKey}executing at {DateTimeUtils.CurrentDateTimeStr}");
            try
            {
                _messageService = serviceProvider.GetService<IMessageService>();

                _messageService.AutoSendMail();
            }
            catch (Exception ex)
            {
                _dbContext.Insert<FapJobLog>(new FapJobLog { JobId = jobKey.Name, JobName = jobDetail.Description, ExecuteTime = DateTimeUtils.CurrentDateTimeStr, ExecuteResult = "failure", Message = ex.Message });
                _logger.LogInformation($"---{jobKey} Error in job!---{ex.Message}");
                //JobExecutionException e2 = new JobExecutionException(ex);
                //// this job will refire immediately
                //e2.RefireImmediately = true;
                //throw e2;
            }

            _logger.LogInformation($"---{jobKey} completed at {DateTimeUtils.CurrentDateTimeStr}");
            _dbContext.Insert<FapJobLog>(new FapJobLog { JobId = jobKey.Name, JobName = jobDetail.Description, ExecuteTime = DateTimeUtils.CurrentDateTimeStr, ExecuteResult = "success" });
            return Task.FromResult(true);


        }

    }
}
