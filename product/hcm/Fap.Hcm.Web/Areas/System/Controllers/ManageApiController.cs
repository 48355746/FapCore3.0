using Dapper;
using Fap.Core.Extensions;
using Fap.Core.Scheduler;
using Fap.Model.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Sys = System;
using System.Collections.Generic;
using System.Linq;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.AspNetCore.Controls;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Rbac.Model;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.Domain;
using Fap.Hcm.Web.Areas.System.Models;
using Fap.Core.Infrastructure.Model;
using System.Net.Mime;
using Fap.Hcm.Web.Models;
using Fap.Core.Utility;
using Fap.Core.Tracker;
using System.Threading.Tasks;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Rbac.AC;
using Fap.Hcm.Service.System;

namespace Fap.Hcm.Web.Areas.System.Controllers
{
    [Area("System")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api/Manage")]
    public class ManageApiController : FapController
    {
        private ISchedulerService _schedule;
        private IEventDataHandler _dataHandler;
        private readonly IManageService _manageService;
        public ManageApiController(Sys.IServiceProvider serviceProvider, IManageService manageService, ISchedulerService schedulerService, IEventDataHandler dataHandler) : base(serviceProvider)
        {
            _schedule = schedulerService;
            _dataHandler = dataHandler;
            _manageService = manageService;
        }
        #region 用户用户组

        [HttpGet("UserGroup")]
        public JsonResult GetUserGroup()
        {
            var tree = _manageService.GetUserGroupTree();
            return Json(tree);
        }

        [HttpPost("UserGroup")]
        public JsonResult OperUserGroup(TreePostData postData)
        {
            var result = _manageService.OperUserGroup(postData);
            return Json(result);
        }

        [HttpGet("UserTheme")]
        public void SetUserTheme(string theme)
        {
            _manageService.SetTheme(theme, _applicationContext.UserUid);
        }
        /// <summary>
        /// 重置密码为系统设置的默认密码
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        [HttpPost("ResetPassword")]
        public JsonResult ResetPassword(FidsModel model)
        {
            var rv = _manageService.ResetPasswor(model.Fids);
            return Json(rv ? ResponseViewModelUtils.Sueecss() : ResponseViewModelUtils.Failure());
        }
        #endregion

        #region 角色角色组
        [HttpGet("RoleGroup")]
        public JsonResult GetRoleGroup()
        {
            var tree = _manageService.GetRoleGroupTree();
            return Json(tree);
        }
        [HttpGet("RoleAndGroup")]
        public JsonResult GetRoleAndGroup()
        {
            var tree = _manageService.GetRoleAndGroupTree();
            return Json(tree);
        }

        [HttpPost("RoleGroup")]
        public JsonResult GetOperRoleGroupData(TreePostData postData)
        {
            var result = _manageService.OperRoleGroup(postData);

            return Json(result);
        }

        #endregion

        #region 业务角色员工
        [HttpGet("BusinessRole")]
        public JsonResult GetBusinessRole()
        {
            var tree = _manageService.GetBusinessRoleTree();
            return Json(tree);
        }
        [HttpPost("BusinessRole")]
        public JsonResult OperBusinessRole(TreePostData postData)
        {
            var result = _manageService.OperBusinessRole(postData);
            return Json(result);
        }


        /// <summary>
        /// 添加业务角色员工
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("BusinessRoleEmployee")]
        public JsonResult PostBusinessRoleEmployee(IEnumerable<dynamic> emps, string bizRole)
        {
            List<FapBizRoleEmployee> list = new List<FapBizRoleEmployee>();
            foreach (dynamic emp in emps)
            {
                FapBizRoleEmployee model = new FapBizRoleEmployee();
                model.EmpUid = emp.Fid;
                model.BizRoleUid = bizRole;
                list.Add(model);
            }
            _dbContext.InsertBatch<FapBizRoleEmployee>(list);
            return Json(new { success = true });
        }
        [HttpPost]
        [Route("RemoveBizRoleEmployee")]
        public JsonResult PostRemoveBusinessRoleEmployee(IEnumerable<string> emps, string bizRole)
        {
            List<string> listEmp = new List<string>();
            foreach (var item in emps)
            {
                listEmp.Add(item);
            }
            string sql = "delete from FapBizRoleEmployee where BizRoleUid=@BizRoleUid and EmpUid in @EmpUids";
            _dbContext.Execute(sql, new DynamicParameters(new { BizRoleUid = bizRole, EmpUids = listEmp }));
            return Json(new { success = true });
        }
        #endregion

        #region 配置配置组

        [HttpGet("ConfigGroup")]
        public JsonResult GetConfigGroup()
        {
            var tree = _manageService.GetConfigGroupTree();

            return Json(tree);
        }

        [HttpPost("ConfigGroup")]
        public JsonResult OperConfigGroup(TreePostData postData)
        {
            var result = _manageService.OperConfigGroup(postData);
            return Json(result);
        }


        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <param name="cfgrpUid">配置组</param>
        /// <returns></returns>
        [HttpGet("Config")]
        public JsonResult GetFapConfigs(string cfgrpUid)
        {
            var list = _manageService.GetFapConfig(cfgrpUid);
            return Json(list);
        }

        #endregion

        #region 模块菜单
        [HttpGet("Module")]
        public JsonResult GetModule()
        {
            var tree = _manageService.GetModuleTree();
            return Json(tree);
        }

        [HttpGet("ModuleAndMenu")]
        public JsonResult GetModuleAndMenu()
        {
            var tree = _manageService.GetModuleAndMenuTree();
            return Json(tree);
        }
        [Route("AllDepts")]
        public JsonResult GetAllDepts()
        {
            var tree = _manageService.GetAllDeptTree();
            return Json(tree);
        }
        #endregion

        #region 实体属性
        /// <summary>
        /// 权限这里只需要业务表
        /// </summary>
        /// <returns></returns>
        [Route("FapTables")]
        public JsonResult GetFapTable()
        {
            var tree = _manageService.GetMenuEntityTree();
            return Json(tree);
        }
        #endregion

        #region 按钮
        /// <summary>
        /// 权限这里只需要业务表
        /// </summary>
        /// <returns></returns>
        [HttpGet("FapButtons")]
        public JsonResult GetFapButtons()
        {
            var tree = _manageService.GetMenuButtonTree();
            return Json(tree);
        }


        #endregion

        #region 任务组任务
        [Route("JobGroup")]
        // POST: api/Common
        public JsonResult GetJobGroup()
        {
            var tree = _manageService.GetJobGroupTree();

            return Json(tree);
        }
        [HttpGet]
        [Route("OperJobGroup")]
        public JsonResult GetOperJobGroupData(string operation, string id, string parent = "", string text = "")
        {
            string result = _schedule.JobGroupOperation(operation, id, parent, text);
            return Json(new { id = result });
        }


        [HttpPost]
        [Route("OperJobState")]
        public JsonResult PostOperJobState(string fid, string state)
        {
            FapJob fj = _dbContext.Get<FapJob>(fid);
            //禁用
            if (state.EqualsWithIgnoreCase("Disabled"))
            {
                fj.JobState = "Disabled";
                fj.ExecState = ExecuteStatus.NoExec;
                //移除作业
                _schedule.RemoveJob(fj);
            }
            else if (state.EqualsWithIgnoreCase("Enabled"))
            {//启用
                fj.JobState = "Enabled";
                fj.ExecState = ExecuteStatus.Execing;
                _schedule.AddJob(fj);
            }
            else if (state.EqualsWithIgnoreCase("Add"))
            {
                fj.ExecState = ExecuteStatus.Execing;
                //执行作业
                _schedule.AddJob(fj);
            }
            else if (state.EqualsWithIgnoreCase("Suspend"))
            {//暂停
                fj.ExecState = ExecuteStatus.Suspend;
                //暂停作业
                _schedule.PauseJob(fj);
            }
            else if (state.EqualsWithIgnoreCase("Resume"))
            {
                fj.ExecState = ExecuteStatus.Execing;
                _schedule.ResumeJob(fj);
            }
            _dbContext.Update<FapJob>(fj);
            return Json(fj);
        }
        #endregion

        #region 报表

        /// <summary>
        /// 获取报表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("SimpleReport")]
        // POST: api/Common
        public JsonResult GetReportTemplate()
        {
            //List<RptSimpleTemplate> allTemplate = _dbContext.QueryEntityBySql<RptSimpleTemplate>("select Id,Fid,Pid,ReportName,IsDir from RptSimpleTemplate");

            //List<RptSimpleTemplate> templates = allTemplate.Where(r => r.Pid != "0").ToList();
            //if (!_session.AcSession.IsDeveloper())
            //{
            //    var roleReports = _session.AcSession.AccountPrivilege.GetUserReportList();
            //    if (roleReports.Any())
            //    {
            //        var prr = roleReports.Select(r => r.RptUid);
            //        if (prr != null && prr.Any())
            //        {
            //            templates = templates.Where(r => r.IsDir == 1 || (r.IsDir == 0 && prr.Contains(r.Fid))).ToList();
            //        }
            //        else
            //        {
            //            templates = templates.Where(r => r.IsDir == 1).ToList();
            //        }
            //    }
            //    else
            //    {
            //        templates = templates.Where(r => r.IsDir == 1).ToList();
            //    }
            //}
            //List<TreeDataView> oriList = templates.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid.ToString(), Data = new { isdir = "" + t.IsDir, isRpt = (t.IsDir == 0 ? true : false) }, Text = t.ReportName, Icon = (t.IsDir == 1 ? "icon-folder blue ace-icon fa fa-folder" : "icon-folder orange ace-icon fa fa-file-text-o") }).ToList<TreeDataView>();
            //List<TreeDataView> tree = new List<TreeDataView>();

            ////RptSimpleTemplate rootTemplate = _dbContext.QueryFirstOrDefaultEntityBySql<RptSimpleTemplate>("select * from RptSimpleTemplate where Pid='0'");
            //RptSimpleTemplate rootTemplate = allTemplate.FirstOrDefault(r => r.Pid == "0");
            //TreeDataView treeRoot = new TreeDataView()
            //{
            //    Id = "" + rootTemplate.Fid,
            //    Text = rootTemplate.ReportName,
            //    Data = new { isdir = "" + rootTemplate.IsDir, isRpt = false },
            //    State = new NodeState { Opened = true },
            //    Icon = "icon-folder blue ace-icon fa fa-folder",
            //};
            //tree.Add(treeRoot);
            //TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);
            //return Json(tree);
            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "报表",
                Data = new { isdir = 1, isRpt = false },
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-folder",
            };
            tree.Add(treeRoot);
            return Json(tree);
        }

        #endregion

        #region 授权

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="roleUser"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DelRoleUser")]
        public JsonResult DeleteRoleUser(FapRoleUser roleUser)
        {
            var sc = _rbacService.DeleteRoleUser(roleUser.RoleUid, roleUser.UserUid);

            return Json(sc ? ResponseViewModelUtils.Sueecss() : ResponseViewModelUtils.Failure());
        }
        /// <summary>
        /// 获取权限
        /// </summary>
        /// <param name="rolefid"></param>
        /// <returns></returns>
        [HttpGet("Authority/{roleUid}")]
        public JsonResult GetAuthority(string roleUid)
        {
            var authority = _manageService.GetAuthority(roleUid);
            return Json(authority, false);

        }
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        [HttpPost("Authority")]
        public JsonResult SetAuthority(AuthorityModel authority)
        {
            var rv = _manageService.SaveAuthority(authority);
            return Json(rv);
        }

        #endregion
        #region 邮件模板
        /// <summary>
        /// 获取邮件模板
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        [Route("MailTemplate/{fid}")]
        public JsonResult GetMailTemplate(string fid)
        {
            CfgEmailTemplate model = _dbContext.Get<CfgEmailTemplate>(fid);
            return Json(model);
        }
        #endregion
        #region 配置设置

        /// <summary>
        /// 配置设置
        /// </summary>
        /// <param name="formObj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ConfigSet")]
        public JsonResult PostConfigSet(IEnumerable<FapConfig> configs)
        {
            ResponseViewModel rv = new ResponseViewModel { success = true };
            if (configs == null)
            {
                return Json(rv);
            }
            try
            {
                _dbContext.UpdateBatch(configs);
            }
            catch (Sys.Exception ex)
            {
                rv.success = false;
                rv.msg = ex.Message;
            }

            return Json(rv);
        }
        /// <summary>
        /// 保存单据回写规则配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Setting/BillWriteBackRule")]
        public bool PostBillWriteBackRule(CfgBillWriteBackRule model)
        {
            if (model.Fid.IsMissing())
            {
                _dbContext.Insert<CfgBillWriteBackRule>(model);
            }
            else
            {
                CfgBillWriteBackRule newModel = _dbContext.Get<CfgBillWriteBackRule>(model.Fid);
                newModel.DocEntityUid = model.DocEntityUid;
                newModel.BizEntityUid = model.BizEntityUid;
                newModel.Association = model.Association;
                newModel.CustomSql = model.CustomSql;
                newModel.FieldMapping = model.FieldMapping;
                newModel.IsNotifyPayroll = model.IsNotifyPayroll;
                newModel.EmpCategoryFilter = model.EmpCategoryFilter;
                _dbContext.Update<CfgBillWriteBackRule>(newModel);
            }
            return true;
        }

        /// <summary>
        /// 保存动态角色配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Setting/BusinessDynRole")]
        public bool PostBusinessDynRole(FapBizDynRole model)
        {
            if (model.Fid.IsMissing())
            {
                _dbContext.Insert<FapBizDynRole>(model);
            }
            else
            {
                FapBizDynRole newModel = _dbContext.Get<FapBizDynRole>(model.Fid);
                newModel.RoleName = model.RoleName;
                newModel.CustomSql = model.CustomSql;
                newModel.State = model.State;
                newModel.BindType = model.BindType;
                _dbContext.Update<FapBizDynRole>(newModel);
            }
            return true;
        }
        /// <summary>
        /// 清除系统配置缓存 config
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Setting/ClearConfigCache")]
        public JsonResult ClearConfigCache()
        {
            _platformDomain.ParamSet.Refresh();
            return Json(new { success = "success" });
        }

        /// <summary>
        /// 清除系统缓存，table，column，dict，config
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Setting/ClearCache")]
        public JsonResult ClearCache()
        {
            _platformDomain.Refresh();
            return Json(new { success = "success" });
        }

        /// <summary>
        /// 清除权限相关缓存
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Setting/ClearPermissionsCache")]
        public JsonResult ClearPermissionsCache()
        {
            _platformDomain.Refresh();
            return Json(ResponseViewModelUtils.Sueecss());
        }

        [HttpPost]
        [Route("Setting/GetBusinessDynRole")]
        public JsonResult GetBusinessDynRole(string fid)
        {
            FapBizDynRole dyRole = _dbContext.Get<FapBizDynRole>(fid);
            return Json(dyRole);
        }

        #endregion

        #region 实时同步
        [HttpPost]
        [Route("RealtimeSyn")]
        public async Task<JsonResult> PostRealtimeSyn(RealtimeSynLog model)
        {
            List<FapRealtimeSynLog> ls = model.Logs;
            ResponseViewModel rv = new ResponseViewModel { success = true };
            if (ls != null && ls.Count > 0)
            {
                foreach (var log in ls)
                {
                    SynchResult result = await _dataHandler.PostEventData(log.RemoteUrl, log.SynData);
                    if (!result.Success)
                    {
                        log.SynState = 0;
                        log.SynLog = result.ErrMsg;
                    }
                    else
                    {
                        log.SynState = 1;
                        log.SynLog = "同步成功";
                    }
                    _dbContext.Update<FapRealtimeSynLog>(log);
                }
            }
            return Json(rv);
        }
        #endregion

    }
}
