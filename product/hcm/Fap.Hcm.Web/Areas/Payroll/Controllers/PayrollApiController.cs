using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.Payroll.Controllers
{
    [Area("Payroll")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api/Payroll")]
    public class PayrollApiController : FapController
    {
        public PayrollApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}