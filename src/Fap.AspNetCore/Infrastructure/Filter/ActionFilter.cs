using Fap.Core.Infrastructure.Domain;
using Fap.Model.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Fap.AspNetCore.Infrastructure.Filter
{
    public class FapActionFilter : IActionFilter
    {
        ILogger logger;
        Stopwatch stopwatch;
        IFapPlatformDomain _platformDomain;
        public FapActionFilter(ILoggerFactory loggerFactory, IFapPlatformDomain domain)
        {
            logger = loggerFactory.CreateLogger("actionFilter");
            stopwatch = new Stopwatch();
            _platformDomain = domain;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            stopwatch.Stop();
            string path = context.HttpContext.Request.Path.Value;
            long ms = stopwatch.ElapsedMilliseconds;
            if (_platformDomain.MenuSet.TryGetValueByPath($"~{path}", out FapMenu menu))
            {
                logger.LogInformation($"{menu.ModuleUid}--{menu.MenuName}--{menu.MenuUrl}耗时:{ms}毫秒");
            }
            else
            {
                logger.LogInformation($"{path}耗时:{ms}毫秒");
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            stopwatch.Restart();
            string path = context.HttpContext.Request.Path.Value;
            if (_platformDomain.MenuSet.TryGetValueByPath($"~{path}", out FapMenu menu))
            {
                logger.LogInformation($"{menu.ModuleUid}--{menu.MenuName}--{menu.MenuUrl}进入");
            }
            else
            {
                logger.LogInformation($"{path}进入");
            }
        }
    }
    public class SampleAsyncActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // do something before the action executes
            await next();
            // do something after the action executes
        }
    }
}
