using System;
using System.Linq;
using System.Net.Mime;
using Ardalis.GuardClauses;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Fap.Core.Extensions;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Web;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Utility;
using System.IO;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Query;
using Fap.Hcm.Web.Models;

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
        /// 持久化表格数据
        /// </summary>
        /// <param name="formObj"></param>
        /// <returns></returns>
        [HttpPost("Persistence")]
        // POST: api/Common
        public async Task<JsonResult> Persistence(IFormCollection keyValues)
        {
            var rv= await _gridFormService.PersistenceAsync(keyValues);
            return Json(rv);
        }
        [HttpPost("BatchUpdate")]
        // POST: api/Common
        public JsonResult BatchUpdate(IFormCollection keyValues)
        {
            var rv= _gridFormService.BatchUpdate(keyValues);
            return Json(rv);
        }
        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("DataList")]
        public JsonResult Datalist(JqGridPostData postDataModel)
        {
            Guard.Against.Null(postDataModel, nameof(postDataModel));
            var result = _gridFormService.QueryPageDataResultView(postDataModel);
            return Json(result);
        }
        /// <summary>
        /// 导出表格数据(不用于导入)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("ExportExcelData")]
        public IActionResult ExportExcelDatalist(JqGridPostData model)
        {
            string fileName = _gridFormService.ExportExcelData(model);
            return Json(new ResponseViewModel { success= true, data=fileName });
        }
        /// <summary>
        /// 导出表格数据（修改后导入）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("ExportExcelTmplData")]
        public JsonResult ExportExcelTemplateAndData(JqGridPostData model)
        {
            string fileName = _gridFormService.ExportExcelData(model);
            return Json(new ResponseViewModel {success = true, data = fileName });
        }
        //导出表格模板
        [HttpPost("ExportExcelTmpl")]
        // POST: api/Common
        public JsonResult ExportExcelTemplate(QuerySet querySet)
        {
            Guard.Against.Null(querySet, nameof(querySet));
            string fileName= _gridFormService.ExportExcelTemplate(querySet);
            return Json(new ResponseViewModel { success = true, data = fileName });

        }
        /// <summary>
        /// 导入数据到表格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("ImportExcelData/{tableName}")]
        public bool ImportExcelData(string tableName)
        {
            Guard.Against.NullOrWhiteSpace(tableName, nameof(tableName));
            _gridFormService.ImportExcelData(tableName);
            return true;
        }
        /// <summary>
        /// 根据tableName 获取属性项
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("FieldList/{tableName}")]
        public JsonResult GetFieldListByTbName(string tableName,string qryCols="*")
        {
            Guard.Against.NullOrWhiteSpace(tableName, nameof(tableName));           
            var colCacheList = _dbContext.Columns(tableName);
            if (!qryCols.Equals("*"))
            {
                var queryColList =HttpUtility.UrlDecode(qryCols).ToLower().SplitComma();
                colCacheList = colCacheList.Where(c => queryColList.Contains(c.ColName.ToLower()));
            }
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