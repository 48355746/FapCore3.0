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
using Microsoft.Extensions.Primitives;
using Fap.AspNetCore.Binder;
using Fap.AspNetCore.Serivce;
using Fap.Core.Infrastructure.Model;
using System.Text.RegularExpressions;

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
        public async Task<JsonResult> Persistence(FormModel frmModel)
        {
            Guard.Against.Null(frmModel,nameof(frmModel));
            var rv = await _gridFormService.PersistenceAsync(frmModel);
            return Json(rv);
        }
        /// <summary>
        /// 获取表格数据
        /// </summary>
        /// <param name="postDataModel"></param>
        /// <returns></returns>
        [HttpPost("DataList")]
        public JsonResult Datalist(JqGridPostData postDataModel)
        {
            Guard.Against.Null(postDataModel, nameof(postDataModel));
            var result = _gridFormService.QueryPageDataResultView(postDataModel);
            return Json(result.GetJqGridJsonData());
        }
        /// <summary>
        /// 获取树表数据
        /// </summary>
        /// <param name="postDataModel"></param>
        /// <returns></returns>
        [HttpPost("TreeDataList")]
        public JsonResult TreeDatalist(JqGridPostData postDataModel)
        {
            Guard.Against.Null(postDataModel, nameof(postDataModel));
            var result = _gridFormService.QueryPageDataResultView(postDataModel);
            return Json(result.GetJqGridTreeJsonData());
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
            return Json(new ResponseViewModel { success = true, data = $"{FapPlatformConstants.TemporaryFolder}/{fileName}" });
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
            return Json(new ResponseViewModel { success = true, data = $"{FapPlatformConstants.TemporaryFolder}/{fileName}" });
        }
        //导出表格模板
        [HttpPost("ExportExcelTmpl")]
        // POST: api/Common
        public JsonResult ExportExcelTemplate(QuerySet querySet)
        {
            Guard.Against.Null(querySet, nameof(querySet));
            string fileName = _gridFormService.ExportExcelTemplate(querySet);
            return Json(new ResponseViewModel { success = true, data = $"{FapPlatformConstants.TemporaryFolder}/{fileName}" });

        }
        /// <summary>
        /// 导入excel数据到表格
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
        /// 导入数据到表格
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("ImportWordTemplate/{tableName}")]
        public bool ImportWordTemplate(string tableName)
        {
            Guard.Against.NullOrWhiteSpace(tableName, nameof(tableName));
            _gridFormService.ImportWordTemplate(tableName);
            return true;
        }
        /// <summary>
        /// 检查word打印模板是否存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [HttpPost("PrintWordTemplate")]
        public JsonResult PrintWordTemplate(GridModel gridModel)
        {
            Guard.Against.Null(gridModel, nameof(gridModel));
            Guard.Against.NullOrWhiteSpace(gridModel.TableName, nameof(gridModel.TableName));
            string fileName = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.Template, $"{gridModel.TableName.ToLower()}.docx");
            if (!System.IO.File.Exists(fileName))
            {
                return Json(ResponseViewModelUtils.Failure());
            }
            fileName=_gridFormService.PrintWordTemplate(gridModel);
            return Json(new ResponseViewModel() {success=true,data= $"{FapPlatformConstants.TemporaryFolder}/{fileName}" });

        }
        /// <summary>
        /// 根据tableName 获取属性项
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [HttpGet("FieldList/{tableName}")]
        public JsonResult GetFieldListByTbName(string tableName, string qryCols = "*")
        {
            Guard.Against.NullOrWhiteSpace(tableName, nameof(tableName));
            var colCacheList = _multiLangService.GetMultiLanguageColumns(tableName).ExcludeAuditColumns();
            if (!qryCols.Equals("*"))
            {
                var queryColList = HttpUtility.UrlDecode(qryCols).ToLower().SplitComma();
                colCacheList = colCacheList.Where(c => queryColList.Contains(c.ColName.ToLower()));
            }
            return Json(colCacheList);
        }
        [HttpPost("MultiLanguage")]
        public JsonResult RegisterMultiLanguage(string langKey,string langValue)
        {
           string rv= _multiLangService.GetOrAndMultiLangValue(Core.MultiLanguage.MultiLanguageOriginEnum.Javascript, langKey, langValue);
            return Json(new ResponseViewModel { success = true, data = rv });
        }
        #region QueryProgram
        [HttpPost("QueryProgram")]
        public JsonResult QueryProgram(CfgQueryProgram model)
        {
            model.Owner = _applicationContext.EmpUid;
            model.IsGlobal = 0;
            model.UseEmployee = _applicationContext.EmpUid;
            long id= _dbContext.Insert<CfgQueryProgram>(model);
            bool success =id>0?true:false;
            return Json(new ResponseViewModel() { success=success,data=model});
        }
        [HttpGet("QueryProgram/{tableName}")]
        public JsonResult QueryProgram(string tableName)
        {
            string where = "TableName=@TableName and (UseEmployee=@EmpUid or IsGlobal=1)";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("TableName", tableName);
            parameters.Add("EmpUid", _applicationContext.EmpUid);
            var qryList= _dbContext.QueryWhere<CfgQueryProgram>(where, parameters);
            return Json(new ResponseViewModel() { success = true, data = qryList });
        }
        #endregion
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

        #region 校验条件sql设置
        
        [HttpPost("ValideSqlCondition")]
        // POST: api/Common
        public JsonResult PostValideSqlCondition(string tableName,string conditionSql)
        {
            bool success = true;
            string msg = string.Empty;          
            Regex rgx = new Regex(FapPlatformConstants.VariablePattern);
            MatchCollection matchs = rgx.Matches(conditionSql);
            foreach (var mtch in matchs)
            {
                conditionSql = conditionSql.Replace(mtch.ToString(), "'1'");
            }
            string sql = "select 1 from dual where " + conditionSql;

            try
            {
                _dbContext.Execute(sql);
            }
            catch (Exception ex)
            {
                success = false;
                msg = ex.Message;
            }
            return Json(new ResponseViewModel{ success = success, msg = msg });
        }
        #endregion
    }
}