using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.Model;
using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Rbac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fap.Hcm.Web.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Route("api/[controller]")]
    public class CoreApiController : FapController
    {
        public CoreApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("datalist")]
        public JsonResult PostDatalist(JqGridPostData postDataModel)
        {
            Guard.Against.Null(postDataModel, nameof(postDataModel));
            var result = GetJqGridDataList(postDataModel);
            return Json(result);
        }
    }
}