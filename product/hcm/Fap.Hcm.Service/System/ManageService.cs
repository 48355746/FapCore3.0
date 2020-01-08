using Fap.AspNetCore.Controls;
using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Rbac;
using Fap.Core.Rbac.Model;
using Fap.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Fap.Core.Infrastructure.Domain;
using Newtonsoft.Json.Linq;
using Fap.Model.Infrastructure;
using Fap.Core.Infrastructure.Metadata;

namespace Fap.Hcm.Service.System
{
    [Service]
    public class ManageService : IManageService
    {
        private readonly IRbacService _rbacService;
        private readonly IDbContext _dbContext;
        private readonly IFapConfigService _configService;
        private readonly IFapApplicationContext _applicationContext;
        private readonly IFapPlatformDomain _platformDomain;
        public ManageService(IRbacService rbacService, IDbContext dbContext, IFapConfigService configService, IFapApplicationContext applicationContext, IFapPlatformDomain platformDomain)
        {
            _rbacService = rbacService;
            _dbContext = dbContext;
            _configService = configService;
            _applicationContext = applicationContext;
            _platformDomain = platformDomain;
        }


        public IEnumerable<TreeDataView> GetUserGroupTree()
        {
            IEnumerable<FapUserGroup> userGroups = _rbacService.GetAllUserGroup();
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
            return tree;
        }


        public ResponseViewModel OperUserGroup(TreePostData postData)
        {
            ResponseViewModel vm = new ResponseViewModel();
            if (postData.Operation == TreeNodeOper.DELETE_NODE)
            {
                vm.success = _rbacService.DeleteUserGroup(postData.Id);
            }
            else if (postData.Operation == TreeNodeOper.CREATE_NODE)
            {
                FapUserGroup userGroup = new FapUserGroup()
                {
                    Pid = postData.Id,
                    UserGroupName = postData.Text
                };
                _rbacService.CreateUserGroup(userGroup);
                vm.success = true;
                vm.data = userGroup.Fid;
            }
            else if (postData.Operation == TreeNodeOper.RENAME_NODE)
            {
                var userGroup = _dbContext.Get<FapUserGroup>(postData.Id);
                userGroup.UserGroupName = postData.Text;
                vm.success = _rbacService.EditUserGroup(userGroup);
            }
            else if (postData.Operation == "move_node")
            {
                var userGroup = _dbContext.Get<FapUserGroup>(postData.Id);
                userGroup.Pid = postData.Parent;
                vm.success = _rbacService.EditUserGroup(userGroup);
            }
            else if (postData.Operation == TreeNodeOper.COPY_NODE)
            {
                throw new NotImplementedException();
            }
            return vm;
        }

        public IEnumerable<TreeDataView> GetRoleGroupTree()
        {
            IEnumerable<FapRoleGroup> roleGroups = _rbacService.GetAllRoleGroup();

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
            return tree;
        }
        public IEnumerable<TreeDataView> GetRoleAndGroupTree()
        {
            IEnumerable<FapRoleGroup> roleGroups = _rbacService.GetAllRoleGroup();
            IEnumerable<FapRole> roles = _rbacService.GetAllRole();
            if (!_applicationContext.IsAdministrator)
            {
                var roleRoles = _rbacService.GetRoleRoleList(_applicationContext.CurrentRoleUid).Select(r => r.PRoleUid);
                //自已有权限和自己创建的角色
                roles = roles.Where(r => roleRoles.Contains(r.Fid) || r.CreateBy == _applicationContext.EmpUid);

            }
            List<TreeDataView> treeGroup = roleGroups.Select(t => new TreeDataView { Id = t.Fid.ToString(), Data = new { IsRole = false }, Pid = t.Pid.ToString(), Text = t.RoleGroupName, State = new NodeState { Opened = true }, Icon = "icon-folder purple ace-icon fa fa-users" }).ToList<TreeDataView>();
            List<TreeDataView> treeRole = roles.Select(r => new TreeDataView { Id = r.Fid.ToString(), Data = new { IsRole = true }, Pid = r.RoleGroupUid, Text = r.RoleName, State = new NodeState { Opened = true }, Icon = "icon-folder orange ace-icon fa fa-users" }).ToList<TreeDataView>();
            List<TreeDataView> tree = new List<TreeDataView>();

            List<TreeDataView> treeRoots = treeGroup.Where(g => g.Pid == "0").ToList();

            foreach (var treeRoot in treeRoots)
            {
                TreeViewHelper.MakeTree(treeRoot.Children, treeGroup, treeRoot.Id);
            }
            tree.AddRange(treeRoots);
            List<TreeDataView> tempGroup = new List<TreeDataView>();
            foreach (var item in tree)
            {
                var rl = treeRole.Where(r => r.Pid == item.Id);
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
            return tree;
        }
        public ResponseViewModel OperRoleGroup(TreePostData postData)
        {
            ResponseViewModel vm = new ResponseViewModel();
            if (postData.Operation == TreeNodeOper.DELETE_NODE)
            {
                vm.success = _rbacService.DeleteUserGroup(postData.Id);
            }
            else if (postData.Operation == TreeNodeOper.CREATE_NODE)
            {
                FapRoleGroup roleGroup = new FapRoleGroup()
                {
                    Pid = postData.Id,
                    RoleGroupName = postData.Text
                };
                _rbacService.CreateRoleGroup(roleGroup);
                vm.success = true;
                vm.data = roleGroup.Fid;
            }
            else if (postData.Operation == TreeNodeOper.RENAME_NODE)
            {
                var roleGroup = _dbContext.Get<FapRoleGroup>(postData.Id);
                roleGroup.RoleGroupName = postData.Text;
                vm.success = _rbacService.EditRoleGroup(roleGroup);
            }
            else if (postData.Operation == "move_node")
            {
                var roleGroup = _dbContext.Get<FapRoleGroup>(postData.Id);
                roleGroup.Pid = postData.Parent;
                vm.success = _rbacService.EditRoleGroup(roleGroup);
            }
            else if (postData.Operation == TreeNodeOper.COPY_NODE)
            {
                throw new NotImplementedException();
            }
            return vm;
        }

        public bool ResetPasswor(IList<string> Uids)
        {
            string password = _configService.GetSysParamValue("employee.user.password");
            if (password.IsMissing())
            {
                password = "1";
            }
            PasswordHasher pwdHasher = new PasswordHasher();
            password = pwdHasher.HashPassword(password);
            int r = _dbContext.Execute("update FapUser set UserPassword=@Pwd where Fid in @Fids", new DynamicParameters(new { Pwd = password, Fids = Uids }));
            return r > 0 ? true : false;
        }

        public void SetTheme(string theme, string userUid)
        {
            FapUser user = _dbContext.Get<FapUser>(userUid);
            user.Theme = theme;
            _dbContext.Update<FapUser>(user);
        }

        public IEnumerable<TreeDataView> GetBusinessRoleTree()
        {
            IEnumerable<FapBizRole> bizRoles = _rbacService.GetAllBizRole();
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
            return tree;
        }

        public ResponseViewModel OperBusinessRole(TreePostData postData)
        {
            ResponseViewModel vm = new ResponseViewModel();
            if (postData.Operation == TreeNodeOper.DELETE_NODE)
            {
                vm.success = _rbacService.DeleteBizRole(postData.Id);
            }
            else if (postData.Operation == TreeNodeOper.CREATE_NODE)
            {
                FapBizRole bizRole = new FapBizRole()
                {
                    Pid = postData.Id,
                    BizRoleName = postData.Text
                };
                _rbacService.CreateBizRole(bizRole);
                vm.success = true;
                vm.data = bizRole.Fid;
            }
            else if (postData.Operation == TreeNodeOper.RENAME_NODE)
            {
                var bizRole = _dbContext.Get<FapBizRole>(postData.Id);
                bizRole.BizRoleName = postData.Text;
                vm.success = _rbacService.EditBizRole(bizRole);
            }
            else if (postData.Operation == "move_node")
            {
                var bizRole = _dbContext.Get<FapBizRole>(postData.Id);
                bizRole.Pid = postData.Parent;
                vm.success = _rbacService.EditBizRole(bizRole);
            }
            else if (postData.Operation == TreeNodeOper.COPY_NODE)
            {
                throw new NotImplementedException();
            }
            return vm;
        }

        public IEnumerable<TreeDataView> GetConfigGroup()
        {
            IEnumerable<FapConfigGroup> configGroups = _configService.GetAllFapConfigGroup();
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
            return tree;
        }

        public ResponseViewModel OperConfigGroup(TreePostData postData)
        {
            ResponseViewModel vm = new ResponseViewModel();
            if (postData.Operation == TreeNodeOper.DELETE_NODE)
            {
                vm.success = _configService.DeleteFapConfigGroup(postData.Id);
            }
            else if (postData.Operation == TreeNodeOper.CREATE_NODE)
            {
                FapConfigGroup configGroup = new FapConfigGroup()
                {
                    Pid = postData.Id,
                    CfName = postData.Text
                };
                _configService.CreateFapConfigGroup(configGroup);
                vm.success = true;
                vm.data = configGroup.Fid;
            }
            else if (postData.Operation == TreeNodeOper.RENAME_NODE)
            {
                var configGroup = _dbContext.Get<FapConfigGroup>(postData.Id);
                configGroup.CfName = postData.Text;
                vm.success = _configService.EditFapConfigGroup(configGroup);
            }
            else if (postData.Operation == TreeNodeOper.MOVE_NODE)
            {
                var configGroup = _dbContext.Get<FapConfigGroup>(postData.Id);
                configGroup.Pid = postData.Parent;
                vm.success = _configService.EditFapConfigGroup(configGroup);
            }
            else if (postData.Operation == TreeNodeOper.COPY_NODE)
            {
                throw new NotImplementedException();
            }
            return vm;

        }

        public IEnumerable<GrpConfig> GetFapConfig(string configGroup)
        {
            var configs = _platformDomain.ParamSet.Where(p => p.ConfigGroup == configGroup);
            var dictList = configs.Where(p => p.DictList.IsPresent());

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

            var grpConfigs = configs.OrderBy(f => f.SortBy).GroupBy(c => c.ParamGroup);

            foreach (var item in grpConfigs)
            {
                GrpConfig gc = new GrpConfig();
                gc.Grp = item.Key;
                gc.Configs = item.ToList();
                yield return gc;
            }

        }

        public IEnumerable<TreeDataView> GetModule()
        {
            List<TreeDataView> moList = _platformDomain.ModuleSet.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid, Text = t.ModuleName, Icon = (t.Icon.IsMissing() ? "icon-folder green ace-icon fa fa-leaf" : "icon-folder green ace-icon " + t.Icon) }).ToList<TreeDataView>();

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
            return tree;
        }

        public IEnumerable<TreeDataView> GetModuleAndMenu()
        {
            var menus = _platformDomain.MenuSet.ToList();
            if (!_applicationContext.IsAdministrator)
            {
                var roleMenus = _rbacService.GetRoleMenuList(_applicationContext.CurrentRoleUid);
                if (roleMenus.Any())
                {
                    var roleUids = roleMenus.Select(m => m.MenuUid);
                    menus = menus.Where(m => roleUids.Contains(m.Fid)).ToList();
                }
            }

            List<TreeDataView> moduleList = _platformDomain.ModuleSet.Select(t => new TreeDataView { Id = t.Fid.ToString(), Data = new { IsMenu = false }, Pid = t.Pid.ToString(), Text = t.ModuleName, State = new NodeState { Opened = false }, Icon = (t.Icon.IsMissing() ? "icon-folder green ace-icon fa fa-leaf" : "icon-folder green ace-icon " + t.Icon) }).ToList<TreeDataView>();
            //授权 仅仅授予到二级菜单
            List<TreeDataView> menuList = menus.Where(m => m.MenuCode.Length == 5).Select(r => new TreeDataView { Id = r.Fid.ToString(), Data = new { IsMenu = true }, Pid = r.ModuleUid, Text = r.MenuName, State = new NodeState { Opened = false }, Icon = "icon-folder orange ace-icon fa fa-leaf" }).ToList<TreeDataView>();
            List<TreeDataView> threeLevels = menus.Where(m => m.MenuCode.Length == 7).Select(r => new TreeDataView { Id = r.Fid.ToString(), Data = new { IsMenu = true }, Pid = r.Pid, Text = r.MenuName, State = new NodeState { Opened = false }, Icon = "icon-folder orange ace-icon fa fa-leaf" }).ToList<TreeDataView>();
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
            return tree;
        }

        public IEnumerable<TreeDataView> GetAllDept()
        {
            IEnumerable<OrgDept> powerDepts = null;
            if (_applicationContext.IsAdministrator)
            {
                powerDepts = _platformDomain.OrgDeptSet.OrderBy(d => d.DeptOrder);
            }
            else
            {
                powerDepts = _rbacService.GetRoleDeptList(_applicationContext.CurrentRoleUid);
            }

            IEnumerable<TreeDataView> treeList = powerDepts.Select(data => new TreeDataView() { Id = data.Fid, Text = data.DeptName, Pid = data.Pid, Data = new { Code = data.DeptCode, Ext1 = data.HasPartPower, Ext2 = "" }, Icon = "icon-folder  ace-icon fa fa-folder orange" });
            string _rootText = string.Empty;
            List<TreeDataView> tree = new List<TreeDataView>();
            string parentID = "0";
            var pt = powerDepts.FirstOrDefault<OrgDept>(t => t.Pid == "0" || t.Pid.IsMissing() || t.Pid == "#" || t.Pid == "~");

            if (pt != null)
            {
                _rootText = pt.DeptName;
                parentID = pt.Fid;
            }
            else
            {
                _rootText = "无权限";
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
            return tree;
        }
    }
}
