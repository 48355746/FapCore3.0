using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Fap.Core.Rbac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Rbac.Model;
using Fap.Core.Infrastructure.Query;
using Fap.Core.Infrastructure.Config;
using Fap.AspNetCore.Model;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Infrastructure.Model;

namespace Fap.Hcm.Web.Areas.System.Controllers
{
    [Area("System")]
    public class ManageController : FapController
    {
        public ManageController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 用户管理 
        /// </summary>
        /// <returns></returns>
        // [OutputCache(CacheProfile = "DefaultCache")]
        public IActionResult User()
        {
            string strWhere = string.Empty;
            if (!_applicationContext.IsAdministrator)
            {
                strWhere = "UserIdentity in (select fid from Employee where DeptUid in(" + FapPlatformConstants.DepartmentAuthority + ") )";

            }
            JqGridViewModel model = this.GetJqGridModel("FapUser", (qs) =>
            {
                qs.QueryCols = "Id,Fid,UserCode,UserName,UserEmail, UserPhone,UserIdentity,UserGroupUid,UserType,UserNote,ContentLang,EnableState";
                qs.GlobalWhere = strWhere;
            });
            //JqGridViewModel model = this.GetJqGridModel("FapUser");
            return View(model);
        }
        /// <summary>
        /// 角色管理 
        /// </summary>
        /// <returns></returns>
        public IActionResult Role()
        {
            string initWhere = string.Empty;
            if (!_applicationContext.IsAdministrator)
            {
                initWhere = "CreateBy='" + _applicationContext.EmpUid + "' ";
                List<FapRole> roles = new List<FapRole>();
                IEnumerable<FapRoleRole> rrs = null;
                //授予的角色
                if (_platformDomain.RoleRoleSet.TryGetValueByRole(_applicationContext.CurrentRoleUid, out rrs))
                {
                    if (rrs != null && rrs.Any())
                    {
                        roles = roles.Where(r => rrs.FirstOrDefault(rr => rr.PRoleUid == r.Fid) != null).ToList();
                    }
                }
                if (roles.Any())
                {
                    initWhere += " or Fid in(" + string.Join(",", roles.Select(r => "'" + r.Fid + "'")) + ")";
                }
            }
            JqGridViewModel model = this.GetJqGridModel("FapRole", (qs) =>
            {
                qs.GlobalWhere = initWhere;
            });
            return View(model);
        }
        public IActionResult Menu()
        {
            JqGridViewModel model = this.GetJqGridModel("FapMenu");
            return View(model);
        }
        /// <summary>
        /// 集团组织
        /// </summary>
        /// <returns></returns>
        public IActionResult GroupOrganization()
        {
            MultiJqGridViewModel multiModel = new MultiJqGridViewModel();
            JqGridViewModel orgModel = this.GetJqGridModel("FapOrg");
            multiModel.JqGridViewModels.Add("FapOrg", orgModel);
            JqGridViewModel groupModel = this.GetJqGridModel("FapGroup");
            multiModel.JqGridViewModels.Add("FapGroup", groupModel);
            return View(multiModel);
        }
        /// <summary>
        /// 权限管理
        /// </summary>
        /// <returns></returns>
        public IActionResult Permissions()
        {
            JqGridViewModel model = this.GetJqGridModel("FapRoleData", (qs) =>
            {
                qs.InitWhere = "1=2";
            });
            return View(model);
        }
        /// <summary>
        /// 业务角色，用于处理业务中使用的角色，不用于权限分配
        /// </summary>
        /// <returns></returns>
        public IActionResult BusinessRole()
        {

            MultiJqGridViewModel model = new MultiJqGridViewModel();
            JqGridViewModel bizRole = this.GetJqGridModel("Employee", (qs) =>
            {
                qs.InitWhere = "1=2";
                qs.GlobalWhere = "DeptUid in(" + FapPlatformConstants.DepartmentAuthority + ") and IsMainJob=1";
                qs.QueryCols = "Id,Fid,EmpCategory,EmpCode,EmpName,Gender,DeptUid,EmpPosition";
            });

            JqGridViewModel dynRole = this.GetJqGridModel("FapBizDynRole");
            model.JqGridViewModels.Add("FapBizRole", bizRole);
            model.JqGridViewModels.Add("FapBizDynRole", dynRole);
            return View(model);
        }
        /// <summary>
        /// 在线用户
        /// </summary>
        /// <returns></returns>
        public IActionResult OnlineUser()
        {
            //30分钟之内状态为登陆
            JqGridViewModel model = this.GetJqGridModel("FapOnlineUser", (queryOption) =>
            {
                queryOption.GlobalWhere = "OnlineState =@OnlineState and LoginTime>=@LoginTime";
                queryOption.AddParameter("OnlineState", FapOnlineUser.CONST_LOGON);
                queryOption.AddParameter("LoginTime", DateTime.Now.AddMinutes(-30).ToString("yyyy-MM-dd HH:mm:ss"));
                queryOption.OrderByList.Add(new OrderBy { Field = "LoginTime", Direction = "DESC" });
            });
            return View(model);
        }
        /// <summary>
        /// 系统设置
        /// </summary>
        /// <returns></returns>
        public IActionResult Config()
        {
            return View();
        }
        /// <summary>
        /// 任务调度
        /// </summary>
        /// <returns></returns>
        public IActionResult Job()
        {
            JqGridViewModel model = this.GetJqGridModel("FapJob");
            return View(model);
        }
        /// <summary>
        /// Job日志
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public PartialViewResult JobLog(string fid)
        {
            JqGridViewModel model = this.GetJqGridModel("FapJobLog", (q) =>
            {
                q.GlobalWhere = $"JobId=@JobId";
                q.AddParameter("JobId", fid);
                q.AddOrderBy("ExecuteTime", "desc");
            });
            return PartialView(model);
        }
        /// <summary>
        /// 隐私设置
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            JqGridViewModel model = this.GetJqGridModel("FapDataEncrypt");
            return View(model);
        }
        /// <summary>
        /// 单据设置
        /// </summary>
        /// <returns></returns>
        public IActionResult BillSet()
        {
            MultiJqGridViewModel model = new MultiJqGridViewModel();
            JqGridViewModel billcode = this.GetJqGridModel("CfgBillCodeRule");
            JqGridViewModel billbw = this.GetJqGridModel("CfgBillWriteBackRule");
            JqGridViewModel mailTemplate = this.GetJqGridModel("CfgEmailTemplate", (qs) =>
            {
                qs.GlobalWhere = "ModuleUid='BillMailTmpl'";
                qs.AddDefaultValue("ModuleUid", "BillMailTmpl");
                //qs.QueryCols = "Id,Fid,Code,Name,ModuleUid,TableName,Enabled";
            });
            JqGridViewModel ffmodel = GetJqGridModel("CfgFreeForm", (qs) =>
            {
                //qs.QueryCols = "Id, Fid, FFCode, FFName, BillTable, Enabled";
            }, true);
            model.JqGridViewModels.Add("WfFreeForm", ffmodel);
            model.JqGridViewModels.Add("CfgBillCodeRule", billcode);
            model.JqGridViewModels.Add("CfgBillWriteBackRule", billbw);
            model.JqGridViewModels.Add("CfgEmailTemplate", mailTemplate);
            return View(model);
        }
        /// <summary>
        /// 单据回写设置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult BillWriteBackSet()
        {
            return View();
        }
        public IActionResult FreeFormSet(string fid)
        {
            CfgFreeForm ffModel = _dbContext.Get<CfgFreeForm>(fid);
            IEnumerable<FapTable> childTables = _platformDomain.TableSet.Where(t => t.MainTable == ffModel.BillTable);
            ViewBag.DbFields = _dbContext.Columns(ffModel.BillTable);
            ViewBag.ChildTables = childTables;
            return View(ffModel);
        }
        /// <summary>
        /// 单据邮件模板
        /// </summary>
        /// <param name="id">fid</param>
        /// <returns></returns>
        public ViewResult BillMailTemplate(string fid)
        {
            ViewBag.Fid = fid;
            return View();
        }
      

        /// <summary>
        /// 用户选择
        /// </summary>
        /// <returns></returns>
        public PartialViewResult UserSelector()
        {
            string strWhere = string.Empty;
            if (!_applicationContext.IsAdministrator)
            {
                //DataAccessor da = new DataAccessor();
                //List<dynamic> powers = da.QueryOrginal("select fid from Employee where DeptUid in(" + _httpSession.AcSession.AccountPrivilege.GetUserDeptAuthorityWhere() + ") and " + da.GetValidWhereSql());
                //if (powers != null && powers.Any())
                //{
                //    var power = powers.Select(d => "'" + d.fid + "'");
                //    strWhere = "UserIdentity in(" + string.Join(",", power) + ")";
                //}
                //else
                //{
                //    strWhere = "1=2";
                //}
                strWhere = "UserIdentity in (select fid from Employee where DeptUid in(" + FapPlatformConstants.DepartmentAuthority + "))";

            }
            JqGridViewModel model = this.GetJqGridModel("FapUser", (qs) =>
            {
                qs.QueryCols = "Id,Fid,UserCode,UserName,UserIdentity";
                qs.GlobalWhere = strWhere;
            });
            return PartialView(model);
        }

        /// <summary>
        /// 角色选择
        /// </summary>
        /// <returns></returns>
        public IActionResult RoleSelector()
        {
            JqGridViewModel model = this.GetJqGridModel("FapRole");
            return View(model);
        }

        /// <summary>
        /// 业务角色选择
        /// </summary>
        /// <returns></returns>
        public IActionResult BusinessRoleSelector()
        {
            return View();
        }
        /// <summary>
        /// 统计查询
        /// </summary>
        /// <returns></returns>
        public IActionResult StatisticsQuery()
        {
            JqGridViewModel model = this.GetJqGridModel("CfgStatisticsQuery");
            return View(model);
        }

        /// <summary>
        /// 实时同步设置
        /// </summary>
        /// <returns></returns>
        public IActionResult RealtimeSetting()
        {
            JqGridViewModel modelSetting = this.GetJqGridModel("CfgRTSynchSetting");
            JqGridViewModel modelLog = this.GetJqGridModel("FapRealtimeSynLog", (qs) =>
            {
                qs.InitWhere = "SynState=0";
            });
            MultiJqGridViewModel model = new MultiJqGridViewModel();
            model.JqGridViewModels.Add("setting", modelSetting);
            model.JqGridViewModels.Add("log", modelLog);
            return View(model);
        }

    }
}