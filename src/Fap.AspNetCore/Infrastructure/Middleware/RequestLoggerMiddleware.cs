using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.AspNetCore.Infrastructure.Middleware
{
    /// <summary>
    /// 监控请求耗时
    /// </summary>
    public class RequestLoggerMiddleware
    {
        const string MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        readonly ILogger<RequestLoggerMiddleware> _logger;

        readonly RequestDelegate _next;

        public RequestLoggerMiddleware(RequestDelegate next, ILogger<RequestLoggerMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var start = Stopwatch.GetTimestamp();
            try
            {
                await _next(httpContext);
                var elapsedMs = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());

                var statusCode = httpContext.Response?.StatusCode;
                var level = statusCode > 499 ? LogLevel.Error : LogLevel.Information;

                if (level == LogLevel.Error)
                {
                    LogForErrorContext(httpContext);
                }
                _logger.Log(level, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, statusCode, elapsedMs);
            }
            // Never caught, because `LogException()` returns false.
            catch (Exception ex) when (LogException(httpContext, GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()), ex)) { }
        }

        bool LogException(HttpContext httpContext, double elapsedMs, Exception ex)
        {
            LogForErrorContext(httpContext);
            _logger.LogError(ex, MessageTemplate, httpContext.Request.Method, httpContext.Request.Path, 500, elapsedMs);

            return false;
        }

        void LogForErrorContext(HttpContext httpContext)
        {
            var request = httpContext.Request;
            _logger.LogInformation($"RequestHeaders:{request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())},RequestHost:{request.Host},RequestProtocol:{request.Protocol}");


            if (request.HasFormContentType)
                _logger.LogInformation($"RequestForm:{request.Form.ToDictionary(v => v.Key, v => v.Value.ToString())}");
        }

        static double GetElapsedMilliseconds(long start, long stop)
        {
            return (stop - start) * 1000 / (double)Stopwatch.Frequency;
        }
    }
}
