using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Fap.AspNetCore.Controls;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Hcm.Service.Assess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Fap.Hcm.Web.Areas.Assess.Controllers
{
    [Area("Assess")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api")]
    public class AssessApiController : FapController
    {
        private readonly IAssessService _assessService;
        public AssessApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _assessService = serviceProvider.GetService<IAssessService>();
        }
        [HttpGet("SchemeCategory")]
        public JsonResult SchemeCategory()
        {
            var tree= _assessService.GetSchemeCategoryTree();
            return Json(tree);
        }
        [HttpPost("SchemeCategory")]
        public JsonResult OperSchemeCategory(TreePostData postData)
        {
            var result = _assessService.OperSchemeCategory(postData);
            return Json(result);
        }
    }
}