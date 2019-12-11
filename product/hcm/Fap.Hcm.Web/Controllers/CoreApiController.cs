using System;
using System.Linq;
using System.Net.Mime;
using Ardalis.GuardClauses;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Fap.Core.Extensions;
using Dapper;

namespace Fap.Hcm.Web.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Route("Api/Core")]
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
        /// <summary>
        /// 根据tableName 获取属性项
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("fieldlist/{tableName}")]
        public JsonResult GetFieldListByTbName(string tableName)
        {
            Guard.Against.NullOrWhiteSpace(tableName, nameof(tableName));           
            var colCacheList = _dbContext.Columns(tableName);
            return Json(colCacheList);
        }
        #region validateForm
        /// <summary>
        /// 校验唯一
        /// </summary>
        /// <param name="chktable">校验表</param>
        /// <param name="chkcolumn">校验列</param>
        /// <param name="currcol">当前列</param>
        /// <param name="orivalue">原始值</param>
        /// <returns></returns>
        [HttpGet]
        [Route("UniqueCheck")]
        public JsonResult GetUniqueCheck(string chktable, string chkcolumn, string orivalue, string currcol, string fid, string currTable)
        {
            string currValue = "";
            //根据当前列获取值
            if (Request.Query.ContainsKey(currcol))
            {
                currValue = Request.Query[currcol];
            }
            if (currValue == orivalue)
            {
                return Json(true);
            }
            string where = $"{chkcolumn}=@Value";
            if (currTable.ToBool())
            {
                where += $" and Fid!='{fid}'";
            }
            DynamicParameters param = new DynamicParameters();
            param.Add("Value", currValue);
            int c = _dbContext.Count(chktable, where, param);
            bool rv = c > 0 ? false : true;
            return Json(rv);

        }
        #endregion
    }
}