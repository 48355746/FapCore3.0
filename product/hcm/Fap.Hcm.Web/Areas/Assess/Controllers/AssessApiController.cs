using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Fap.AspNetCore.Controls;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
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
        [HttpGet("SchemeCategory")]
        public JsonResult SchemeCategory()
        {
            
        }
        [HttpPost("SchemeCategory")]
        public JsonResult OperSchemeCategory(TreePostData postData)
        {
            var result = _manageService.OperUserGroup(postData);
            return Json(result);
        }
    }
}