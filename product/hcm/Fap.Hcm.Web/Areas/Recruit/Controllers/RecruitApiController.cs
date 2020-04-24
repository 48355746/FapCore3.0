using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.Recruit.Controllers
{
    [Area("Recruit")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api")]
    public class RecruitApiController : FapController
    {
        public RecruitApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}