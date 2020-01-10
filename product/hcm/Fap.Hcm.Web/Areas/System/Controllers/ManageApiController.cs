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
        public bool ResetPassword(FidsModel model)
        {
            return _manageService.ResetPasswor(model.Fids);
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
        /// 获取权限
        /// </summary>
        /// <param name="rolefid"></param>
        /// <returns></returns>
        [HttpGet("Authority/{roleUid}")]
        public JsonResult GetAuthority(string roleUid)
        {
            var authority= _manageService.GetAuthority(roleUid);
            return Json(authority, false);
            //DynamicParameters dparam = new DynamicParameters();
            //dparam.Add("RoleUid", roleUid);
            ////获取角色用户
            //IEnumerable<dynamic> userList = _dbContext.Query("select Fid, UserCode,UserName,UserEmail,UserIdentity from FapUser where fid in(select useruid from FapRoleUser where RoleUid=@RoleUid) order by UserCode", dparam, true);
            ////获取角色菜单
            //IEnumerable<dynamic> menus = _dbContext.Query("select MenuUid from FapRoleMenu where RoleUid= @RoleUid", dparam);
            ////获取角色部门
            //IEnumerable<dynamic> depts = _dbContext.Query("select DeptUid from FapRoleDept where RoleUid=@RoleUid", dparam);
            ////获取角色报表
            //IEnumerable<dynamic> rpts = _dbContext.Query("select RptUid from FapRoleReport where RoleUid=@RoleUid", dparam);
            ////获取角色实体属性

            //IEnumerable<FapRoleColumn> columnList = _rbacService.GetRoleColumnList(roleUid);// _dbContext.Query("select ColumnUid,EditAble,ViewAble from FapRoleColumn where RoleUid=@RoleUid", dparam);
            ////获取角色角色
            //IEnumerable<dynamic> roles = _dbContext.Query("select PRoleUid from FapRoleRole where RoleUid=@RoleUid", dparam);

            ////获取角色按钮
            //IEnumerable<FapRoleButton> roleButtons = _dbContext.Query<FapRoleButton>("select * from FapRoleButton where RoleUid=@RoleUid", dparam);

            ////var userJson = users.Select(x => new { Id = x.Fid }).ToList();
            //var menuJson = menus.Select(x => x.MenuUid).ToList();
            //var deptJson = depts.Select(x => x.DeptUid).ToList();
            //var rptJson = rpts.Select(x => x.RptUid).ToList();
            //var roleJson = roles.Select(x => x.PRoleUid).ToList();
            //var buttonJson = GetRoleButtons();
            //var json = new
            //{
            //    users = userList,
            //    menus = menuJson,
            //    depts = deptJson,
            //    rpts = rptJson,
            //    columns = columnList,
            //    roles = roleJson,
            //    buttons = buttonJson
            //};
            //return Json(json, false);
            //IEnumerable<string> GetRoleButtons()
            //{
            //    foreach (var rbtn in roleButtons)
            //    {
            //        if (rbtn.ButtonType == FapMenuButtonType.Grid || rbtn.ButtonType == FapMenuButtonType.Tree)
            //        {
            //            foreach (var v in rbtn.ButtonValue.SplitComma())
            //            {
            //                yield return $"{rbtn.MenuUid}|{rbtn.ButtonType}|{ rbtn.ButtonId }|{v}";
            //            }
            //        }
            //        else
            //        {
            //            yield return $"{rbtn.MenuUid}|{rbtn.ButtonType}|{ rbtn.ButtonId }|{rbtn.ButtonValue}";
            //        }
            //    }
            //}
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="roleUser"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("DelRoleUser")]
        public JsonResult SetRoleUser(FapRoleUser roleUser)
        {
            _dbContext.DeleteExec(nameof(FapRoleUser), "RoleUid=@RoleUid and UserUid=@UserUid", new DynamicParameters(new { RoleUid = roleUser.RoleUid, UserUid = roleUser.UserUid }));
            var result = new { success = true };
            return Json(result);
        }
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Authority")]
        public JsonResult SetAuthority(Authority auth)
        {
            bool success = false;
            //菜单
            if (auth.AType == 1)
            {
                List<FapRoleMenu> menus = new List<FapRoleMenu>();
                if (auth.MenuUids != null && auth.MenuUids.Any())
                {
                    foreach (var item in auth.MenuUids)
                    {
                        FapRoleMenu fdoMenu = new FapRoleMenu();
                        fdoMenu.RoleUid = auth.RoleUid;
                        fdoMenu.MenuUid = item;
                        menus.Add(fdoMenu);
                    }
                }
                success = _rbacService.AddRoleMenu(auth.RoleUid, menus);
                _platformDomain.RoleMenuSet.Refresh();
            }
            else if (auth.AType == 2)
            {//部门
                List<FapRoleDept> depts = new List<FapRoleDept>();
                if (auth.OrgDeptUids != null && auth.OrgDeptUids.Any())
                {
                    foreach (var item in auth.OrgDeptUids)
                    {
                        FapRoleDept fdoDept = new FapRoleDept();
                        fdoDept.RoleUid = auth.RoleUid;
                        fdoDept.DeptUid = item;
                        depts.Add(fdoDept);
                    }
                }

                success = _rbacService.AddRoleDept(auth.RoleUid, depts);

                _platformDomain.RoleDeptSet.Refresh();
            }
            else if (auth.AType == 3 || auth.AType == 4)
            {
                //实体列
                List<FapRoleColumn> columns = new List<FapRoleColumn>();
                if (auth.ColumnUids != null && auth.ColumnUids.Any())
                {
                    foreach (var item in auth.ColumnUids)
                    {
                        if (auth.AType == 3)
                        {
                            columns.Add(new FapRoleColumn { RoleUid = auth.RoleUid, ColumnUid = item.ColUid,GridId=item.GridId, MenuUid = item.MenuUid, EditAble = 1, ViewAble = 0 });
                        }
                        else
                        {
                            columns.Add(new FapRoleColumn { RoleUid = auth.RoleUid, ColumnUid = item.ColUid, GridId = item.GridId, MenuUid = item.MenuUid, EditAble = 0, ViewAble = 1 });
                        }
                    }
                }
                success = _rbacService.AddRoleColumn(auth.RoleUid, columns, auth.AType);
                //刷新应用程序全局域角色列
                _platformDomain.RoleColumnSet.Refresh();
            }
            else if (auth.AType == 5)
            {
                List<FapRoleUser> users = new List<FapRoleUser>();
                //保存用户
                if (auth.UserUids != null && auth.UserUids.Any())
                {
                    foreach (var item in auth.UserUids)
                    {
                        users.Add(new FapRoleUser() { RoleUid = auth.RoleUid, UserUid = item });
                    }
                }
                if (users.Count > 0)
                {
                    _rbacService.AddRoleUser(users);
                    success = true;
                }
            }
            else if (auth.AType == 6)
            {
                //报表
                List<FapRoleReport> rpts = new List<FapRoleReport>();
                if (auth.RptUids != null && auth.RptUids.Any())
                {
                    foreach (var item in auth.RptUids)
                    {
                        FapRoleReport fdoMenu = new FapRoleReport();
                        fdoMenu.RoleUid = auth.RoleUid;
                        fdoMenu.RptUid = item;
                        rpts.Add(fdoMenu);
                    }
                }
                _rbacService.AddRoleReport(auth.RoleUid, rpts);
                success = true;
                _platformDomain.RoleReportSet.Refresh();
            }
            else if (auth.AType == 7)
            {
                //角色
                List<FapRoleRole> rrs = new List<FapRoleRole>();
                if (auth.PRoleUids != null && auth.PRoleUids.Any())
                {
                    foreach (var item in auth.PRoleUids)
                    {
                        FapRoleRole fdoRR = new FapRoleRole();
                        fdoRR.RoleUid = auth.RoleUid;
                        fdoRR.PRoleUid = item;
                        rrs.Add(fdoRR);
                    }
                }
                _rbacService.AddRoleRole(auth.RoleUid, rrs);
                success = true;
                _platformDomain.RoleRoleSet.Refresh();
            }
            else if (auth.AType == 8)
            {
                //按钮
                var roleButtons = GetRoleButtons(auth.BtnUids);
                _rbacService.AddRoleButton(auth.RoleUid, roleButtons);
                success = true;

                _platformDomain.RoleButtonSet.Refresh();
            }
            return Json(new ResponseViewModel() { success = success });
            IEnumerable<FapRoleButton> GetRoleButtons(IList<string> btnUids)
            {
                if (auth.BtnUids != null)
                {
                    var btnList = btnUids.Select(b =>
                    {
                        string[] s = b.Split('|');
                        return new { MenuUid = s[0], BtnType = s[1], BtnId = s[2], BtnValue = s[3] };
                    });
                    foreach (var btnGrp in btnList.GroupBy(b => b.MenuUid))
                    {
                        foreach (var buttons in btnGrp.GroupBy(b => b.BtnType))
                        {
                            if (buttons.Key == FapMenuButtonType.Grid || buttons.Key == FapMenuButtonType.Tree)
                            {
                                FapRoleButton roleButton = new FapRoleButton { RoleUid = auth.RoleUid, MenuUid = btnGrp.Key };
                                roleButton.ButtonType = buttons.Key;
                                roleButton.ButtonValue = string.Join(',', buttons.ToList().Select(b => b.BtnValue));
                                roleButton.ButtonId = buttons.First().BtnId;
                                yield return roleButton;
                            }
                            else
                            {
                                foreach (var button in buttons)
                                {
                                    FapRoleButton roleButton = new FapRoleButton { RoleUid = auth.RoleUid, MenuUid = btnGrp.Key };
                                    roleButton.ButtonType = buttons.Key;
                                    roleButton.ButtonValue = button.BtnValue;
                                    roleButton.ButtonId = button.BtnId;
                                    yield return roleButton;
                                }
                            }
                        }
                    }
                }
            }
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
            return Json(new { success = "success" });
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
