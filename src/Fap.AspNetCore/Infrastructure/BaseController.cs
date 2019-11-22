using Fap.Core.Extensions;
using Fap.Core.Rbac;
using Fap.AspNetCore.ViewModel;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.MultiLanguage;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.Query;
using Fap.AspNetCore.Serivce;

namespace Fap.AspNetCore.Infrastructure
{
    /// <summary>
    /// 自定义抽象Controller
    /// </summary>
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// 编码UTF-8
        /// </summary>
        protected Encoding ENCODE_UTF8 = Encoding.GetEncoding("UTF-8");
        protected IDbContext _dbContext => ServiceProvider.GetService<IDbContext>();
        protected IFapPlatformDomain _fapPlatformDomain => ServiceProvider.GetService<IFapPlatformDomain>();
        protected IFapConfigService _fapConfigService => ServiceProvider.GetService<IFapConfigService>();
        protected IMultiLangService _multiLangService => ServiceProvider.GetService<IMultiLangService>();
        protected IFapApplicationContext _applicationContext => ServiceProvider.GetService<IFapApplicationContext>();
        protected ILoggerFactory _loggerFactory => ServiceProvider.GetService<ILoggerFactory>();
        protected IRbacService _rbacService => ServiceProvider.GetService<IRbacService>();

        public IServiceProvider ServiceProvider { get; set; }
        public IGridFormService _gridFormService => ServiceProvider.GetService<IGridFormService>();
        public BaseController(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }


        #region 获取JqGrid的数据集合（通用方法）
        /// <summary>
        /// 获取JqGrid的数据集合（通用方法）
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="model"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        protected JsonResult GetJqGridDataList(JqGridPostData model, Action<SimpleQueryOption> handler = null)
        {
            var jqJson = _gridFormService.QueryPageDataResultView(model, handler);
            return Json(jqJson);
        }

        #endregion

        #region 构造树表数据格式

        /// <summary>
        /// 构造树表数据
        /// </summary>
        /// <param name="tableName">数据表</param>
        /// <param name="rows">构造后的行数据集合</param>
        /// <param name="orginData">原始查询数据集合</param>
        /// <param name="dicData">单行数据</param>
        protected void BuilderTreeGridData(string tableName, List<dynamic> rows, IEnumerable<dynamic> orginData, IDictionary<string, object> dicData)
        {

            string fid = dicData[tableName + "_Fid"].ToString();
            //移除掉RowNumber，行号
            dicData.Remove("RowNumber");

            IDictionary<string, object> newDicData = new Dictionary<string, object>();
            newDicData.Add("Tid", fid);
            foreach (var itd in dicData)
            {
                string colName = itd.Key.Split('_')[1];
                if (colName == "Pid" && (itd.Value == null || itd.Value.ToString().IsMissing()))
                {
                    newDicData.Add(colName, "");
                }
                else
                {
                    newDicData.Add(colName, itd.Value);

                }
            }

            // 添加treegrid特有的列
            string pid = tableName + "_Pid";
            int level = newDicData["TreeLevel"].ToInt();
            newDicData.Add("level", level);
            //if (newDicData[pid] == null || newDicData[pid].ToString() == "" || newDicData[pid].ToString() == "~")
            //{
            //    newDicData.Add("parent", null);
            //}
            //else
            //{
            //    newDicData.Add("parent", newDicData[pid]);
            //}
            newDicData.Add("loaded", true);
            if (newDicData["IsFinal"].ToString() == "1")
            {
                newDicData.Add("isLeaf", true);
            }
            else
            {

                //是否叶子节点
                if (orginData.Any(d => ((IDictionary<string, object>)d)[pid].ToString() == newDicData["Fid"].ToString()))
                {
                    newDicData.Add("isLeaf", false);
                }
                else
                {
                    newDicData.Add("isLeaf", true);
                }
            }
            if (level < 2)
            {
                newDicData.Add("expanded", true);
            }
            else
            {
                newDicData.Add("expanded", false);
            }
            //dynamic row = new object();
            //foreach (var item in newDicData)
            //{
            //    row.SetValue()
            //}
            //var obj = new
            //{
            //    id = newDicData[tableName + "_Id"],
            //    cell = newDicData.Values.ToArray()
            //};
            rows.Add(newDicData);
            //递归顺序寻找子
            TreeGridDataSeq(rows, fid, tableName, orginData);
        }

        private void TreeGridDataSeq(List<dynamic> rows, string fId, string tableName, IEnumerable<dynamic> orginData)
        {
            foreach (IDictionary<string, object> dicData in orginData)
            {
                //获取父id
                string pid = tableName + "_Pid";

                string pidv = dicData[pid].ToString();
                if (pidv != fId)
                {
                    continue;
                }
                BuilderTreeGridData(tableName, rows, orginData, dicData);
            }
        }

        #endregion
        /// <summary>
        /// 过滤Session
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // context.HttpContext.Session.Get<MoUserInfo>(context.HttpContext.Session.SessionKey()); //获取登录session
            //if (_MyUserInfo == null)
            //{
            //    context.Result = new RedirectToActionResult(nameof(MemberController.Login), "Member", new { ReturnUrl = context.HttpContext.Request.Path });
            //}


            base.OnActionExecuting(context);
        }
        /// <summary>
        /// Json序列化忽略空,首字母小写
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected JsonResult JsonIgnoreNull(object obj)
        {
            return Json(obj, new JsonSerializerSettings
            {//日期类型默认格式化处理  
                //DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat,
                //DateFormatString = "yyyy-MM-dd HH:mm:ss",
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()//驼峰 首字母小写
            });
        }
        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isCamelCase">首字母小写（true）</param>
        /// <returns></returns>
        protected JsonResult Json(object data, bool isCamelCase)
        {
            if (isCamelCase)
            {
                return Json(data, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            }
            else
            {
                return Json(data, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() });
            }
        }

    }
}