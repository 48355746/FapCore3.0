using Dapper;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Interface;
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
using Fap.AspNetCore.Extensions;
using Fap.Core.Infrastructure.Model;
using System.Net.Mime;
using Fap.Hcm.Web.Models;
using Fap.Core.Utility;
using Fap.Core.Tracker;
using System.Threading.Tasks;

namespace Fap.Hcm.Web.Areas.System.Controllers
{
    [Area("System")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api/Manage")]
    public class ManageApiController : FapController
    {
        private ISchedulerService _schedule;
        private IEventDataHandler _dataHandler;
        public ManageApiController(Sys.IServiceProvider serviceProvider, ISchedulerService schedulerService,IEventDataHandler dataHandler) : base(serviceProvider)
        {
            _schedule = schedulerService;
            _dataHandler = dataHandler;
        }


        #region 用户用户组

        [Route("UserGroup")]
        // POST: api/Common
        public JsonResult GetUserGroup()
        {
            IEnumerable<FapUserGroup> userGroups = _rbacService.GetUserGroups();
            List<TreeDataView> oriList = userGroups.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid.ToString(), Data = new { group = "1" }, Text = t.UserGroupName, Icon = "icon-folder orange ace-icon fa fa-users" }).ToList<TreeDataView>();

            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "用户组",
                Data = new { group = "0" },
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-sitemap",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);

            return Json(tree);
        }

        [HttpGet]
        [Route("OperUserGroup")]
        public JsonResult GetOperUserGroupData(string operation, string id, string parent = "", string text = "", string position = "")
        {
            string result = _rbacService.UserGroupOperation(operation, id, parent, text);
            return Json(new { id = result });
        }


        [HttpGet]
        [Route("SetUserTheme")]
        // POST: api/Common
        public bool PostFapUserUpdate(string theme)
        {
            FapUser user = _dbContext.Get<FapUser>(_applicationContext.UserUid);
            user.Theme = theme;
            _dbContext.Update<FapUser>(user);
            return true;
        }
        /// <summary>
        /// 重置密码为系统设置的默认密码
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ResetPassword")]
        public bool PostResetPassword(FidsModel model)
        {
            string password = _configService.GetSysParamValue("employee.user.password");
            if (password.IsMissing())
            {
                password = "1";
            }
            PasswordHasher pwdHasher = new PasswordHasher();
            password = pwdHasher.HashPassword(password);
            int r = _dbContext.Execute("update FapUser set UserPassword=@Pwd where Fid in @Fids", new DynamicParameters(new { Pwd = password, Fids = model.Fids }));
            return r > 0;
        }
        #endregion

        #region 角色角色组
        [Route("RoleGroup/")]
        // POST: api/Common
        public JsonResult GetRoleGroup()
        {
            FapRole currRole = _rbacService.GetCurrentRole();
            IEnumerable<FapRoleGroup> roleGroups = _dbContext.QueryAll<FapRoleGroup>();
            if (!_applicationContext.IsAdministrator)
            {
                List<string> grps = new List<string>();

                var groupUid = currRole.RoleGroupUid;
                grps.Add(groupUid);
                IEnumerable<FapRoleRole> rrs = null;
                //授予的角色
                if (_platformDomain.RoleRoleSet.TryGetValueByRole(currRole.Fid, out rrs))
                {
                    if (rrs != null && rrs.Any())
                    {
                        var roles = _platformDomain.RoleSet.ToList().Where(r => rrs.FirstOrDefault(rr => rr.PRoleUid == r.Fid) != null).ToList();
                        grps.AddRange(roles.Select(r => r.RoleGroupUid));
                    }
                }
                roleGroups = roleGroups.Where<FapRoleGroup>(grp => grps.Contains(grp.Fid)).ToList();
            }

            List<TreeDataView> oriList = roleGroups.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid.ToString(), Text = t.RoleGroupName, Icon = "icon-folder orange ace-icon fa fa-users" }).ToList<TreeDataView>();

            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "角色组",
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-sitemap",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);

            return Json(tree);
        }
        [Route("RoleAndGroup/")]
        public JsonResult GetRoleAndGroup()
        {
            IEnumerable<FapRoleGroup> roleGroups = _dbContext.QueryAll<FapRoleGroup>();
            List<FapRole> roles = new List<FapRole>();
            if (_applicationContext.IsAdministrator)
            {
                roles = _platformDomain.RoleSet.ToList();// _dbContext.QueryEntityByWhere<FapRole>();                
            }
            else
            {
                IEnumerable<FapRoleRole> rrs = null;
                //授予的角色
                if (_platformDomain.RoleRoleSet.TryGetValueByRole(_applicationContext.CurrentRoleUid, out rrs))
                {
                    if (rrs != null && rrs.Any())
                    {
                        roles = _platformDomain.RoleSet.ToList().Where(r => rrs.FirstOrDefault(rr => rr.PRoleUid == r.Fid) != null).ToList();
                    }
                }
                DynamicParameters param = new DynamicParameters();
                param.Add("Employee", _applicationContext.EmpUid);
                var selfRoles = _dbContext.QueryWhere<FapRole>("CreateBy=@Employee", param);
                selfRoles.AsList().ForEach((r) =>
                {
                    if (!roles.Contains(r))
                    {
                        roles.Add(r);
                    }
                });
            }
            List<TreeDataView> oriList = roleGroups.Select(t => new TreeDataView { Id = t.Fid.ToString(), Data = new { IsRole = false }, Pid = t.Pid.ToString(), Text = t.RoleGroupName, State = new NodeState { Opened = true }, Icon = "icon-folder purple ace-icon fa fa-users" }).ToList<TreeDataView>();
            List<TreeDataView> roleList = roles.Select(r => new TreeDataView { Id = r.Fid.ToString(), Data = new { IsRole = true }, Pid = r.RoleGroupUid, Text = r.RoleName, State = new NodeState { Opened = true }, Icon = "icon-folder orange ace-icon fa fa-users" }).ToList<TreeDataView>();
            List<TreeDataView> tree = new List<TreeDataView>();
            //超级管理员可以授权普通用户角色
            if (_applicationContext.IsAdministrator)
            {
                //加普通用户角色
                oriList.Insert(0, new TreeDataView { Id = FapPlatformConstants.CommonUserRoleFid, Data = new { IsRole = true }, Pid = "0", Text = "普通用户", Icon = "icon-folder orange ace-icon fa fa-users" });
            }
            List<TreeDataView> treeRoots = oriList.Where(g => g.Pid == "0").ToList();

            foreach (var treeRoot in treeRoots)
            {
                TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);
            }
            tree.AddRange(treeRoots);
            List<TreeDataView> tempGroup = new List<TreeDataView>();
            foreach (var item in tree)
            {
                var rl = roleList.Where(r => r.Pid == item.Id);
                if (rl.Any())
                {
                    item.Children.AddRange(rl);
                }
                else
                {
                    if (!item.Text.Equals("普通用户"))
                    {
                        tempGroup.Add(item);
                    }
                }
            }
            if (tempGroup.Any())
            {
                //移除没有角色的角色组
                tempGroup.ForEach((d) => { tree.Remove(d); });
            }
            return Json(tree);
        }

        [HttpGet]
        [Route("OperRoleGroup")]
        public JsonResult GetOperRoleGroupData(string operation, string id, string parent = "", string text = "", string position = "")
        {
            string result = _rbacService.RoleGroupOperation(operation, id, parent, text);
            //刷新全局域角色集合
            _platformDomain.RoleSet.Refresh();

            return Json(new { id = result });
        }



        #endregion

        #region 业务角色员工
        [Route("BusinessRole")]
        public JsonResult GetBusinessRole()
        {
            IEnumerable<FapBizRole> bizRoles = _dbContext.QueryAll<FapBizRole>();
            List<TreeDataView> oriList = bizRoles.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid.ToString(), Data = new { isRole = true }, Text = t.BizRoleName, Icon = "icon-folder orange ace-icon fa fa-users" }).ToList<TreeDataView>();

            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "业务角色",
                Data = new { isRole = false },
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-sitemap",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);

            return Json(tree);
        }
        [HttpGet]
        [Route("OperBusinessRole")]
        public JsonResult GetOperBusinessRole(string operation, string id, string parent = "", string text = "", string position = "")
        {
            string result = _rbacService.BusinessRoleOperation(operation, id, parent, text);
            return Json(new { id = result });
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

        [Route("ConfigGroup/")]
        // POST: api/Common
        public JsonResult GetConfigGroup()
        {
            IEnumerable<FapConfigGroup> configGroups = _dbContext.QueryAll<FapConfigGroup>();
            List<TreeDataView> oriList = configGroups.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid.ToString(), Data = new { group = "1" }, Text = t.CfName, Icon = "icon-folder  ace-icon fa fa-cog" }).ToList<TreeDataView>();

            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "配置组",
                Data = new { group = "0" },
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-cogs",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);

            return Json(tree);
        }

        [HttpGet]
        [Route("OperConfigGroup")]
        public JsonResult GetOperConfigGroupData(string operation, string id, string parent = "", string text = "", string position = "")
        {
            string result = _configService.ConfigGroupOperation(operation, id, parent, text);
            return Json(new { id = result });
        }


        /// <summary>
        /// 获取配置项
        /// </summary>
        /// <param name="cfgrpUid">配置组</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Config/")]
        // POST: api/Common
        public JsonResult PostFapConfigs(string cfgrpUid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("ConfigGroup", cfgrpUid);
            IEnumerable<FapConfig> configs = _dbContext.QueryWhere<FapConfig>("ConfigGroup=@ConfigGroup", param);
            var dictList = configs.Where(c => c.DictList.IsPresent());
            if (dictList != null && dictList.Any())
            {
                foreach (var item in dictList)
                {
                    JObject jsonObj = JObject.Parse(item.DictList);
                    Dictionary<string, string> dictObj = jsonObj.ToObject<Dictionary<string, string>>();
                    List<SelectModel> source = new List<SelectModel>();
                    foreach (var dic in dictObj)
                    {
                        SelectModel model = new SelectModel() { Code = dic.Key, Name = dic.Value };
                        if (dic.Key == item.ParamValue)
                        {
                            model.Selected = true;
                        }
                        source.Add(model);
                    }
                    item.DictSource = source;
                }
            }
            var grpConfigs = configs.OrderBy(f => f.SortBy).GroupBy(c => c.ParamGroup);
            List<GrpConfig> list = new List<GrpConfig>();
            foreach (var item in grpConfigs)
            {
                GrpConfig gc = new GrpConfig();
                gc.Grp = item.Key;
                gc.Configs = item.ToList();
                list.Add(gc);
            }

            return Json(list);
        }

        #endregion

        #region 模块菜单
        [Route("Module/")]
        // POST: api/Common
        public JsonResult GetModule()
        {
            //DataAccessor _dbContext = new DataAccessor();
            List<FapModule> module = _platformDomain.ModuleSet.ToList(); //_dbContext.QueryEntityByWhere<FapModule>();
            List<TreeDataView> moList = module.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid, Text = t.ModuleName, Icon = (t.Icon.IsMissing() ? "icon-folder green ace-icon fa fa-leaf" : "icon-folder green ace-icon " + t.Icon) }).ToList<TreeDataView>();

            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "系统模块",
                State = new NodeState { Opened = true },
                Icon = "icon-folder purple ace-icon fa fa-home bigger-160",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, moList, treeRoot.Id);

            return Json(tree);
        }

        [Route("ModuleAndMenu/")]
        public JsonResult GetModuleAndMenu()
        {
            List<FapModule> modules = _platformDomain.ModuleSet.ToList();// _dbContext.QueryEntityByWhere<FapModule>();
            List<FapMenu> menus = _platformDomain.MenuSet.ToList();// _dbContext.QueryEntityByWhere<FapMenu>();
            //List<string> a = new List<string>();
            if (!_applicationContext.IsAdministrator)
            {
                var roleMenus = _rbacService.GetUserMenuList();
                if (roleMenus.Any())
                {
                    var roleUids = roleMenus.Select(m => m.MenuUid);
                    menus = menus.Where(m => roleUids.Contains(m.Fid)).ToList();
                }
            }

            List<TreeDataView> moduleList = modules.Select(t => new TreeDataView { Id = t.Fid.ToString(), Data = new { IsMenu = false }, Pid = t.Pid.ToString(), Text = t.ModuleName, State = new NodeState { Opened = true }, Icon = (t.Icon.IsMissing() ? "icon-folder green ace-icon fa fa-leaf" : "icon-folder green ace-icon " + t.Icon) }).ToList<TreeDataView>();
            //授权 仅仅授予到二级菜单
            List<TreeDataView> menuList = menus.Where(m => m.MenuCode.Length == 5).Select(r => new TreeDataView { Id = r.Fid.ToString(), Data = new { IsMenu = true }, Pid = r.ModuleUid, Text = r.MenuName, State = new NodeState { Opened = true }, Icon = "icon-folder orange ace-icon fa fa-leaf" }).ToList<TreeDataView>();
            List<TreeDataView> threeLevels = menus.Where(m => m.MenuCode.Length == 7).Select(r => new TreeDataView { Id = r.Fid.ToString(), Data = new { IsMenu = true }, Pid = r.Pid, Text = r.MenuName, State = new NodeState { Opened = true }, Icon = "icon-folder orange ace-icon fa fa-leaf" }).ToList<TreeDataView>();
            List<TreeDataView> tree = new List<TreeDataView>();
            List<TreeDataView> treeRoots = moduleList.Where(g => g.Pid == "0").ToList();

            foreach (var treeRoot in treeRoots)
            {
                TreeViewHelper.MakeTree(treeRoot.Children, moduleList, treeRoot.Id);
            }
            tree.AddRange(treeRoots);
            foreach (var item in tree)
            {
                var rl = menuList.Where<TreeDataView>(r => r.Pid == item.Id);
                if (rl.Any())
                {
                    foreach (var r2 in rl)
                    {

                        //三级菜单
                        if (threeLevels != null && threeLevels.Any())
                        {
                            var rl3 = threeLevels.Where(m => m.Pid == r2.Id);
                            if (rl3 != null && rl3.Any())
                            {
                                r2.Children.AddRange(rl3);
                                //foreach (var r3 in rl3)
                                //{
                                //    r2.Children.Add(r3)
                                //}
                            }
                        }
                    }
                    item.Children.AddRange(rl);
                }
            }
            return Json(tree);
        }
        [Route("AllDepts")]
        public JsonResult GetAllDepts()
        {
            IEnumerable<OrgDept> powerDepts = null;
            if (_applicationContext.IsAdministrator)
            {
                powerDepts = _platformDomain.OrgDeptSet.OrderBy(d => d.DeptOrder);
            }
            else
            {
                powerDepts = _rbacService.GetUserDeptList();
            }


            //将List<dynamic>转换成List<TreeDataView>
            List<TreeDataView> treeList = new List<TreeDataView>();
            foreach (var data in powerDepts)
            {
                treeList.Add(new TreeDataView() { Id = data.Fid, Text = data.DeptName, Pid = data.Pid, Data = new { Code = data.DeptCode, Ext1 = data.HasPartPower, Ext2 = "" }, Icon = "icon-folder  ace-icon fa fa-folder orange" });
            }
            string _rootText = string.Empty;
            List<TreeDataView> tree = new List<TreeDataView>();
            string parentID = "0";
            var pt = powerDepts.FirstOrDefault<OrgDept>(t => t.Pid == "0" || t.Pid.IsMissing() || t.Pid == "#" || t.Pid == "~");
            if (_rootText.IsMissing())
            {
                if (pt != null)
                {
                    _rootText = pt.DeptName;
                    parentID = pt.Fid;
                }
                else
                {
                    _rootText = "无权限";
                }
            }
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = parentID,
                Text = _rootText,
                State = new NodeState { Opened = true },
                Icon = "icon-folder purple ace-icon fa fa-sitemap",

            };
            if (parentID == "0")
            {
                treeRoot.Data = new { Code = "", Ext1 = false, Ext2 = "" };
            }
            else
            {
                treeRoot.Data = new { Code = pt.DeptCode, Ext1 = pt.HasPartPower, Ext2 = "" };
            }
            TreeViewHelper.MakeTree(treeRoot.Children, treeList, treeRoot.Id);
            tree.Add(treeRoot);
            return Json(tree);
        }
        #endregion

        #region 实体属性
        /// <summary>
        /// 权限这里只需要业务表
        /// </summary>
        /// <returns></returns>
        [Route("FapTables/")]
        public JsonResult GetFapTable()
        {
            //业务实体
            DynamicParameters dparam = new DynamicParameters();
            dparam.Add("TableType", "BUSINESS");
            IEnumerable<FapTable> tables = _dbContext.QueryWhere<FapTable>("TableType=@TableType", dparam);
            //实体分类
            DynamicParameters categroyParam = new DynamicParameters();
            categroyParam.Add("Category", "TableCategory");
            IEnumerable<FapDict> tableCategory = _dbContext.QueryWhere<FapDict>("Category=@Category", categroyParam);
            //实体属性
            IEnumerable<FapColumn> columns = _dbContext.QueryWhere<FapColumn>(" IsDefaultCol=0 and TableName in(select tablename from FapTable where TableType='BUSINESS')");
            //构造树
            List<TreeDataView> tree = new List<TreeDataView>();

            var tableGroup = tables.GroupBy(g => g.TableCategory);
            int i = 0;
            foreach (var tg in tableGroup)
            {
                //表分类
                TreeDataView ttc = new TreeDataView();
                ttc.Id = "tablecategory" + i;
                ttc.Data = new { IsColumn = false };
                ttc.Pid = "~";
                ttc.Text = tableCategory.First(c => c.Code == tg.Key).Name;
                ttc.Icon = "blue fa fa-filter";
                ttc.State = new NodeState() { Opened = false };
                int j = 0;
                foreach (var tb in tg)
                {
                    TreeDataView ttb = new TreeDataView();
                    ttb.Id = "table" + i + j;
                    ttb.Data = new { IsColumn = false };
                    ttb.Pid = "tablecategory" + i;
                    ttb.Text = tb.TableComment;
                    ttb.Icon = "purple fa fa-list-alt";
                    var cols = columns.Where(c => c.TableName == tb.TableName).ToList();
                    foreach (var col in cols)
                    {
                        TreeDataView tcol = new TreeDataView();
                        tcol.Id = col.Fid;
                        tcol.Data = new { IsColumn = true, TableName = col.TableName };
                        tcol.Pid = "table" + i + j;
                        tcol.Text = col.ColComment;
                        tcol.Icon = "green fa fa-tag";
                        ttb.Children.Add(tcol);
                    }
                    ttc.Children.Add(ttb);
                    j++;
                }
                tree.Add(ttc);
                i++;
            }
            return Json(tree);
        }
        #endregion

        #region 任务组任务
        [Route("JobGroup/")]
        // POST: api/Common
        public JsonResult GetJobGroup()
        {
            IEnumerable<dynamic> jobGroups = _dbContext.QueryAll("FapJobGroup");
            List<TreeDataView> oriList = jobGroups.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid.ToString(), Text = t.JobGroupName, Icon = "icon-folder purple ace-icon fa fa-clock-o" }).ToList<TreeDataView>();

            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "任务组",
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-sitemap",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);

            return Json(tree);
        }
        [HttpGet]
        [Route("OperJobGroup")]
        public JsonResult GetOperJobGroupData(string operation, string id, string parent = "", string text = "", string position = "")
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
        [Route("SimpleReport/")]
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
        [HttpGet]
        [Route("Authority")]
        public JsonResult GetAuthority(string rolefid)
        {
            DynamicParameters dparam = new DynamicParameters();
            dparam.Add("RoleUid", rolefid);
            //获取角色用户
            IEnumerable<FapUser> users = _dbContext.QueryWhere<FapUser>("fid in(select useruid from FapRoleUser where RoleUid=@RoleUid)", dparam, true).OrderBy(c => c.UserCode);
            //获取角色菜单
            IEnumerable<dynamic> menus = _dbContext.Query("select MenuUid from FapRoleMenu where RoleUid= @RoleUid", dparam);
            //获取角色部门
            IEnumerable<dynamic> depts = _dbContext.Query("select DeptUid from FapRoleDept where RoleUid=@RoleUid", dparam);
            //获取角色报表
            IEnumerable<dynamic> rpts = _dbContext.Query("select RptUid from FapRoleReport where RoleUid=@RoleUid", dparam);
            //获取角色实体属性
            IEnumerable<dynamic> columns = _dbContext.Query("select ColumnUid,EditAble,ViewAble from FapRoleColumn where RoleUid=@RoleUid", dparam);
            //获取角色角色
            IEnumerable<dynamic> roles = _dbContext.Query("select PRoleUid from FapRoleRole where RoleUid=@RoleUid", dparam);

            //var userJson = users.Select(x => new { Id = x.Fid }).ToList();
            var menuJson = menus.Select(x => x.MenuUid).ToList();
            var deptJson = depts.Select(x => x.DeptUid).ToList();
            var rptJson = rpts.Select(x => x.RptUid).ToList();
            var roleJson = roles.Select(x => x.PRoleUid).ToList();
            var json = new
            {
                users = users,
                menus = menuJson,
                depts = deptJson,
                rpts = rptJson,
                columns = columns,
                roles = roleJson
            };
            return Json(json, false);

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
            _dbContext.Execute("delete from FapRoleUser where roleuid=@RoleUid and UserUid=@UserUid", new DynamicParameters(new { RoleUid = roleUser.RoleUid, UserUid = roleUser.UserUid }));
            var result = new { success = true };
            return Json(result);
        }
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SaveAuthority")]
        public JsonResult SetAuthority(Authority auth)
        {
            bool success = false;
            //DataAccessor accessor = new DataAccessor();
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
                            columns.Add(new FapRoleColumn { RoleUid = auth.RoleUid, ColumnUid = item.ColUid, TableUid = item.TableName, EditAble = 1, ViewAble = 0 });
                        }
                        else
                        {
                            columns.Add(new FapRoleColumn { RoleUid = auth.RoleUid, ColumnUid = item.ColUid, TableUid = item.TableName, EditAble = 0, ViewAble = 1 });
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
            success = true;
            return Json(new { success = success });

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
            _platformDomain.SysParamSet.Refresh();
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
                    SynchResult result =await _dataHandler.PostEventData(log.RemoteUrl,log.SynData);
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
