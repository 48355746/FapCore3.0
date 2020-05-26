using Fap.AspNetCore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace Fap.AspNetCore.Infrastructure.Filter
{
    public class FapExceptionFilter : IExceptionFilter
    {
        private readonly ILoggerFactory _loggerFactory;
        public FapExceptionFilter(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }
        public void OnException(ExceptionContext context)
        {
            //var type = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
            ILogger _logger = _loggerFactory.CreateLogger<FapExceptionFilter>();
            _logger.LogError(context.Exception.StackTrace);
            //var result = new ViewResult { ViewName = "CustomError" };
            //result.ViewData.Add("Exception", context.Exception);
            var value = ResponseViewModelUtils.Failure(context.Exception.Message);
            context.Result = new JsonResult(value);
        }
    }
}
