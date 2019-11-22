using Microsoft.AspNetCore.Mvc;
using System;

namespace Fap.AspNetCore.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public class ErrorController : BaseController
    {

        public ErrorController(IServiceProvider serviceProvider) : base(serviceProvider) { } 
        /// <summary>
        /// 404跳转
        /// </summary>
        /// <param name="unknownAction"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult Http404(string unknownAction, string url)
        {            
                return Redirect("~/SiteStatus/HtmlError404");
        }
    }
}
