using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.Assess.Controllers
{
    [Area("Assess")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api")]
    public class AssessApiController : FapController
    {
        public AssessApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}