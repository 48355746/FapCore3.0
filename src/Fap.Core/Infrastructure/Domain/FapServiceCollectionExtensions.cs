using Castle.DynamicProxy;
using Fap.Core.DataAccess;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.MultiLanguage;
using Fap.Core.Tracker;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Infrastructure.Domain
{
    //
    // 摘要:
    //     DI extension methods for adding Fap
    public static class FapServiceCollectionExtensions
    {
        /// <summary>
        /// Creates a builder.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IFapBuilder AddFapBuilder(this IServiceCollection services)
        {
            return new FapBuilder(services);
        }
        //
        // 摘要:
        //     Adds Fap services to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        //
        // 参数:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services
        //     to.
        //
        // 返回结果:
        //     An Microsoft.Extensions.DependencyInjection.IMvcBuilder that can be used to further
        //     configure the MVC services.
        public static IFapBuilder AddFapService(this IServiceCollection services)
        {
            var builder = services.AddFapBuilder();
            builder.AddFap();
            return builder;
        }

        public static IFapBuilder AddFap(this IFapBuilder builder)
        {
            //httpcontext,httpclient            
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();
            var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
                }, onRetryAsync: async (outcome, timespan, retryCount, context) =>
                 {
                     context["RetriesInvoked"] = retryCount;
                     await Task.CompletedTask;
                    // ...
                });
            builder.Services.AddHttpClient("Retry").AddPolicyHandler(retryPolicy);
            //数据库访问
            builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>();
            builder.Services.AddSingleton<IDbSession, DbSession>();
            builder.Services.AddSingleton<IDbContext, DbContext>();
            //AOP代理
            builder.Services.AddSingleton<ProxyGenerator>();
            builder.Services.AddSingleton<IInterceptor, TransactionalInterceptor>();
            //应用程序域，需要初始化为单例
            builder.Services.AddSingleton<IFapPlatformDomain, FapPlatfromDomain>();
            //Fap应用上下文
            builder.Services.AddSingleton<IFapApplicationContext, FapApplicationContext>();

            return builder;
        }
        /// <summary>
        /// 数据变化触发（用于第三方系统同步变化数据）
        /// </summary>
        /// <param name="services"></param>
        public static IFapBuilder AddDataTracker(this IFapBuilder builder)
        {
            builder.Services.AddSingleton<EventDataTracker>();
            builder.Services.AddSingleton<EventDataReporter>();           
            return builder;
        }
        public static void UseDataTracker(this IApplicationBuilder app)
        {
            var provider= app.ApplicationServices;
            EventDataReporter reporter = provider.GetService<EventDataReporter>();
            EventDataTracker tracker = provider.GetService<EventDataTracker>();
            reporter.Subscribe(tracker);
        }
        /// <summary>
        /// 添加调度
        /// </summary>
        /// <param name="services"></param>
        //public static IFapBuilder AddFapScheduler(this IFapBuilder builder)
        //{
        //    //任务调度
        //    builder.Services.AddSingleton<ISchedulerService, SchedulerService>();
        //    return builder;
        //}

        public static void BuilderMultiLanguageJsFile(this IApplicationBuilder app)
        {
            //初始化多语言js文件
            IMultiLangService multiLang = app.ApplicationServices.GetService<IMultiLangService>();
            multiLang.InitMultiLangResJS();
        }


    }
    /// <summary>
    /// 单独开启后台线程处理 Scheduler
    /// </summary>
    //public class BackgroundSchedulerService : BackgroundService
    //{
    //    private readonly ISchedulerService _schedulerService;
    //    public BackgroundSchedulerService(ISchedulerService schedulerService)
    //    {
    //        _schedulerService = schedulerService;
    //    }
    //    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        _schedulerService.Init();
    //        return Task.CompletedTask;
    //    }
    //    public override Task StopAsync(CancellationToken cancellationToken)
    //    {
    //        _schedulerService.ShutdownJobs();
    //        return base.StopAsync(cancellationToken);
    //    }
    //}
    /// <summary>
    /// 启用后台三方同步
    /// </summary>
    //public class BackgroundSynchronizeService : BackgroundService
    //{
    //    private readonly IEventBus _eventBus;
    //    public BackgroundSynchronizeService(IEventBus eventBus)
    //    {
    //        _eventBus = eventBus;
    //    }
    //    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        _eventBus.Subscribe();
    //        return Task.CompletedTask;
    //    }
    //}
}
