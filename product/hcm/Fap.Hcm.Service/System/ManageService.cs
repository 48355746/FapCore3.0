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
            List<TreeDataView> treeGroup = roleGroups.Select(t => new TreeDataView { Id = t.Fid.ToString(), Data = new { IsRole = false }, Pid = t.Pid.ToString(), Text = t.RoleGroupName, State = new NodeState { Opened = true }, Icon = "icon-folder purple ace-icon fa fa-users" }).ToList<TreeDataView>();
            if (!_applicationContext.IsAdministrator)
            {
                var roleRoles = _rbacService.GetRoleRoleList(_applicationContext.CurrentRoleUid).Select(r => r.PRoleUid);
                //自已有权限和自己创建的角色
                roles = roles.Where(r => roleRoles.Contains(r.Fid) || r.CreateBy == _applicationContext.EmpUid);

            }

            //普通用户角色加到tree根级
            FapRole commonRole= roles.FirstOrDefault(r => r.Fid == FapPlatformConstants.CommonUserRoleFid);
            if (commonRole != null)
            {
                treeGroup.Insert(0, new TreeDataView { Id = commonRole.Fid, Data = new { IsRole = true }, Pid = "0", Text = commonRole.RoleName, Icon = "icon-folder orange ace-icon fa fa-users" });
            }
            
            List<TreeDataView> treeRole = roles.Select(r => new TreeDataView { Id = r.Fid.ToString(), Data = new { IsRole = true }, Pid = r.RoleGroupUid, Text = r.RoleName, State = new NodeState { Opened = true }, Icon = "icon-folder orange ace-icon fa fa-users" }).ToList<TreeDataView>();
            List<TreeDataView> tree = new List<TreeDataView>();

            List<TreeDataView> treeRoots = treeGroup.Where(g => g.Pid == "0").ToList();

            foreach (var treeRoot in treeRoots)
            {
                TreeViewHelper.MakeTree(treeRoot.Children, treeGroup, treeRoot.Id);
            }
            tree.AddRange(treeRoots);
            List<TreeDataView> emptyGroup = new List<TreeDataView>();
            foreach (var item in tree)
            {
                var rl = treeRole.Where(r => r.Pid == item.Id);
                if (rl.Any())
                {
                    item.Children.AddRange(rl);
                }
                else
                {
                    if (!item.Id.Equals(FapPlatformConstants.CommonUserRoleFid))
                    {
                        emptyGroup.Add(item);
                    }
                }
            }
            if (emptyGroup.Any())
            {
                //移除没有角色的角色组
                emptyGroup.ForEach((d) => { tree.Remove(d); });
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

        public IEnumerable<TreeDataView> GetConfigGroupTree()
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

        public IEnumerable<TreeDataView> GetModuleTree()
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

        public IEnumerable<TreeDataView> GetModuleAndMenuTree()
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

            List<TreeDataView> moduleList = _platformDomain.ModuleSet
                .Where(m => menus.Select(m => m.ModuleUid).Distinct().Contains(m.Fid))
                .Select(t => new TreeDataView { Id = t.Fid.ToString(), Data = new { IsMenu = false }, Pid = t.Pid.ToString(), Text = t.ModuleName, State = new NodeState { Opened = false }, Icon = (t.Icon.IsMissing() ? "icon-folder green ace-icon fa fa-leaf" : "icon-folder green ace-icon " + t.Icon) }).ToList<TreeDataView>();
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

        public IEnumerable<TreeDataView> GetAllDeptTree()
        {
            IEnumerable<OrgDept> powerDepts = null;
            if (_applicationContext.IsAdministrator)
            {
                powerDepts = _platformDomain.OrgDeptSet.OrderBy(d => d.DeptOrder);
            }
            else
            {
                powerDepts = _rbacService.GetRoleDeptList(_applicationContext.CurrentRoleUid).ToList();
                //如果没有授权根节点 需要加载上根节点
                var allDepts = _platformDomain.OrgDeptSet;
                OrgDept rootDept = allDepts.FirstOrDefault(d => d.Pid.IsMissing() || d.Pid == "#" || d.Pid == "~" || d.Pid == "");
                if (!powerDepts.Contains(rootDept))
                {
                    int powerDeptRootCodeLength = powerDepts.Min(d => d.DeptCode.Length);
                    var powerRootDepts = powerDepts.Where(d => d.DeptCode.Length == powerDeptRootCodeLength).ToList();
                    //向上查找部门
                    foreach (var powerRootDept in powerRootDepts)
                    {
                        FindParentDept(powerRootDept);
                    }
                }
                void FindParentDept(OrgDept powerRootDept)
                {
                    var powerParentDept = allDepts.FirstOrDefault(d => d.Fid == powerRootDept.Pid);
                    if (powerParentDept != null)
                    {
                        powerParentDept.HasPartPower = true;
                        if (!powerDepts.AsList().Exists(d => d.Fid == powerParentDept.Fid))
                        {
                            powerDepts.AsList().Add(powerParentDept);
                        }
                        FindParentDept(powerParentDept);
                    }
                }
            }

            IEnumerable<TreeDataView> treeList = powerDepts.Select(data => new TreeDataView() { Id = data.Fid, Text = data.DeptName, Pid = data.Pid, Data = new { Code = data.DeptCode, Ext1 = data.HasPartPower, Ext2 = "" }, Icon = data.HasPartPower ? "icon-folder  ace-icon fa fa-folder grey" : "icon-folder  ace-icon fa fa-folder orange" });
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

        public IEnumerable<TreeDataView> GetMenuButtonTree()
        {
            var menuButtons = _platformDomain.MenuButtonSet;

            var tree = GetModuleAndMenuTree();
            var mmtor = tree.GetEnumerator();
            AddMenuButton(mmtor);
            return tree;
            void AddMenuButton(IEnumerator<TreeDataView> menuEmumertor)
            {
                while (menuEmumertor.MoveNext())
                {
                    TreeDataView curr = menuEmumertor.Current;
                    if (curr.Children.Any())
                    {
                        AddMenuButton(curr.Children.GetEnumerator());
                    }
                    if (curr.Data.IsMenu)
                    {
                        var children = AddOperNode(curr);
                        if (children.Any())
                        {
                            curr.Children = children.ToList();
                        }
                    }
                }
            }
            IEnumerable<TreeDataView> AddOperNode(TreeDataView menuNode)
            {
                var opers = typeof(OperEnum).EnumItems();
                string menuUid = menuNode.Id;
                if (!_applicationContext.IsAdministrator)
                {
                    //权限中获取
                    var roleButtons = _rbacService.GetRoleButtonList(_applicationContext.CurrentRoleUid);
                    if (roleButtons.Any())
                    {
                        var rbtns = roleButtons.Where(r => r.MenuUid == menuUid);
                        foreach (var btn in rbtns.Where(b => b.ButtonType == FapMenuButtonType.Grid || b.ButtonType == FapMenuButtonType.Tree))
                        {
                            var button = menuButtons.FirstOrDefault(b => b.MenuUid == btn.MenuUid && b.ButtonID == btn.ButtonId);
                            if (button != null)
                            {
                                TreeDataView toper = new TreeDataView()
                                {
                                    Id = button.ButtonID,
                                    Data = new { IsBtn = false, IsMenu = false },
                                    Pid = menuNode.Id,
                                    Text = button.Description,
                                    Icon = btn.ButtonType == FapMenuButtonType.Grid ? "fa fa-table" : "fa fa-code-fork"
                                };

                                foreach (var oper in btn.ButtonValue.SplitComma())
                                {
                                    OperEnum operEnum = (OperEnum)oper.ToInt();
                                    TreeDataView tcol = new TreeDataView();
                                    tcol.Id = $"{menuNode.Id}|{button.ButtonType}|{ button.ButtonID }|{oper}";
                                    tcol.Data = new { IsBtn = true, IsMenu = false };
                                    tcol.Pid = toper.Id;
                                    tcol.Text = operEnum.Description();
                                    tcol.Icon = GetOperIcon(operEnum);
                                    toper.Children.Add(tcol);
                                }
                                yield return toper;

                            }
                        }
                        var buttons = rbtns.Where(b => b.ButtonType == FapMenuButtonType.Link || b.ButtonType == FapMenuButtonType.Button);
                        foreach (var btn in buttons)
                        {
                            var button = menuButtons.FirstOrDefault(b => b.MenuUid == btn.MenuUid && b.ButtonID == btn.ButtonId);
                            TreeDataView toper = new TreeDataView()
                            {
                                Id = $"{menuNode.Id}|button|{ button.ButtonID }|1",
                                Icon = "fa  fa-bolt",
                                Data = new { IsBtn = true, IsMenu = false },
                                Pid = menuNode.Id,
                                Text = button.Description,
                            };
                            yield return toper;
                        }
                    }
                }
                else
                {
                    if (menuButtons.TryGetValue(menuUid, out IEnumerable<FapMenuButton> buttons))
                    {
                        foreach (var button in buttons)
                        {
                            TreeDataView toper = new TreeDataView()
                            {
                                Id = button.ButtonID,
                                Data = new { IsBtn = false, IsMenu = false },
                                Pid = menuNode.Id,
                                Text = button.Description,
                            };
                            if (button.ButtonType == FapMenuButtonType.Grid)
                            {
                                toper.Icon = " fa fa-table";
                                foreach (var oper in opers)
                                {
                                    TreeDataView tcol = new TreeDataView();
                                    tcol.Id = $"{menuNode.Id}|{button.ButtonType}|{ button.ButtonID }|{oper.Key}";
                                    tcol.Data = new { IsBtn = true, IsMenu = false };
                                    tcol.Pid = toper.Id;
                                    tcol.Text = oper.Description;
                                    tcol.Icon = GetOperIcon(oper.Value.ParseEnum<OperEnum>());
                                    toper.Children.Add(tcol);
                                }
                            }
                            else if (button.ButtonType == FapMenuButtonType.Tree)
                            {
                                toper.Icon = " fa fa-code-fork";
                                foreach (var oper in opers)
                                {
                                    if (oper.Key == (int)OperEnum.Add
                                        || oper.Key == (int)OperEnum.Update
                                        || oper.Key == (int)OperEnum.Delete
                                        || oper.Key == (int)OperEnum.Refresh)
                                    {
                                        TreeDataView tcol = new TreeDataView();
                                        tcol.Id = $"{menuNode.Id}|{button.ButtonType}|{ button.ButtonID }|{oper.Key}";
                                        tcol.Data = new { IsBtn = true, IsMenu = false };
                                        tcol.Pid = toper.Id;
                                        tcol.Text = oper.Description;
                                        tcol.Icon = GetOperIcon(oper.Value.ParseEnum<OperEnum>());
                                        toper.Children.Add(tcol);
                                    }
                                }
                            }
                            else if (button.ButtonType == FapMenuButtonType.Link || button.ButtonType == FapMenuButtonType.Button)
                            {
                                toper.Id = $"{menuNode.Id}|button|{ button.ButtonID }|1";
                                toper.Icon = "fa  fa-bolt";
                                toper.Data = new { IsBtn = true, IsMenu = false };
                            }
                            yield return toper;
                        }
                    }
                }

            }
        }
        private string GetOperIcon(OperEnum operEnum)
        {
            return operEnum switch
            {
                OperEnum.Add => "fa fa-plus-circle purple",
                OperEnum.BatchUpdate => "fa fa-pencil-square-o",
                OperEnum.Delete => "fa fa-trash-o red",
                OperEnum.ExportExcel => "fa fa-file-excel-o green",
                OperEnum.ExportWord => "fa fa-file-word-o",
                OperEnum.Import => "fa fa-cloud-upload",
                OperEnum.QueryProgram => "fa fa-camera",
                OperEnum.Refresh => "fa fa-refresh green",
                OperEnum.Search => "fa fa-search orange",
                OperEnum.Update => "fa fa-pencil blue",
                OperEnum.View => "fa fa-search-plus grey",
                _ => "fa fa-bolt"
            };
        }
        public IEnumerable<TreeDataView> GetMenuEntityTree()
        {
            var tree = GetModuleAndMenuTree();
            var mmtor = tree.GetEnumerator();
            AddMenuColumn(mmtor);
            return tree;
            void AddMenuColumn(IEnumerator<TreeDataView> menuEmumertor)
            {
                while (menuEmumertor.MoveNext())
                {
                    TreeDataView curr = menuEmumertor.Current;
                    if (curr.Children.Any())
                    {
                        AddMenuColumn(curr.Children.GetEnumerator());
                    }
                    if (curr.Data.IsMenu)
                    {
                        var children = AddColumnNode(curr);
                        if (children.Any())
                        {
                            curr.Children = children.ToList();
                        }
                    }
                }
            }
            IEnumerable<TreeDataView> AddColumnNode(TreeDataView currNode)
            {
                string menuUid = currNode.Id;
                var menuColumns = _platformDomain.MenuColumnSet.Where(r => r.MenuUid == menuUid);
                foreach (var menuColumn in menuColumns)
                {
                    IEnumerable<FapColumn> columns = _platformDomain.ColumnSet.Where(c => c.IsDefaultCol == 0 && c.TableName == menuColumn.TableName);
                    if (!_applicationContext.IsAdministrator)
                    {
                        var roleColumns = _rbacService.GetRoleColumnList(_applicationContext.CurrentRoleUid);

                        columns = columns.Where(c => roleColumns.Select(rc => rc.ColumnUid).Distinct().Contains(c.Fid));

                    }
                    TreeDataView toper = new TreeDataView()
                    {
                        Id = menuColumn.GridId,
                        Data = new { IsCol = false, IsMenu = false },
                        Pid = menuUid,
                        Text = menuColumn.Description,
                        Icon = "fa fa-table"
                    };
                    if (!menuColumn.GridColumn.Trim().Equals("*"))
                    {
                        var colNames = menuColumn.GridColumn.SplitComma();
                        columns = columns.Where(c => colNames.Contains(c.ColName, new FapStringEqualityComparer()));
                    }
                    //实体属性
                    foreach (var column in columns)
                    {
                        TreeDataView tcol = new TreeDataView()
                        {
                            Id =$"{menuUid}|{menuColumn.GridId}|{column.Fid}",
                            Data = new { IsColumn = true, MenuUid = menuUid,GridId= menuColumn.GridId,ColumnUid=column.Fid },
                            Pid = toper.Id,
                            Text = column.ColComment,
                            Icon = "green fa fa-tag"
                        };
                        toper.Children.Add(tcol);
                    }
                    yield return toper;

                }

            }

        }

        public IEnumerable<TreeDataView> GetJobGroupTree()
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
            return tree;
        }
        public object GetAuthority(string roleUid)
        {
            DynamicParameters dparam = new DynamicParameters();
            dparam.Add("RoleUid", roleUid);

            var roleUsers= _rbacService.GetRoleUserList(roleUid);
            var userList= _platformDomain.UserSet.Where(u => roleUsers.Select(r => r.UserUid).Contains(u.Fid));
            var depts = _rbacService.GetRoleDeptList(roleUid).Select(r => r.Fid);
            var menus = _rbacService.GetRoleMenuList(roleUid).Select(r => r.MenuUid);
            var roles = _rbacService.GetRoleRoleList(roleUid).Select(r => r.PRoleUid);
            var roleButtons = _rbacService.GetRoleButtonList(roleUid);
            //获取角色用户
            //IEnumerable<dynamic> userList = _dbContext.Query("select Fid, UserCode,UserName,UserEmail,UserIdentity from FapUser where fid in(select useruid from FapRoleUser where RoleUid=@RoleUid) order by UserCode", dparam, true);
            //获取角色菜单
            //IEnumerable<dynamic> menus = _dbContext.Query("select MenuUid from FapRoleMenu where RoleUid= @RoleUid", dparam);
            //获取角色部门
            //IEnumerable<dynamic> depts = _dbContext.Query("select DeptUid from FapRoleDept where RoleUid=@RoleUid", dparam);
            //获取角色报表
            IEnumerable<dynamic> rpts = _dbContext.Query("select RptUid from FapRoleReport where RoleUid=@RoleUid", dparam);
            //获取角色实体属性

            IEnumerable<FapRoleColumn> columnList = _rbacService.GetRoleColumnList(roleUid);// _dbContext.Query("select ColumnUid,EditAble,ViewAble from FapRoleColumn where RoleUid=@RoleUid", dparam);
            //获取角色角色
            //IEnumerable<dynamic> roles = _dbContext.Query("select PRoleUid from FapRoleRole where RoleUid=@RoleUid", dparam);

            //获取角色按钮
            //IEnumerable<FapRoleButton> roleButtons = _dbContext.Query<FapRoleButton>("select * from FapRoleButton where RoleUid=@RoleUid", dparam);

            //var userJson = users.Select(x => new { Id = x.Fid }).ToList();
            //var menuJson = menus.Select(x => x.MenuUid).ToList();
            //var deptJson = depts.Select(x => x.DeptUid).ToList();
            var rptJson = rpts.Select(x => x.RptUid).ToList();
            //var roleJson = roles.Select(x => x.PRoleUid).ToList();
            var buttonJson = GetRoleButtons();
            var json = new
            {
                users = userList,
                menus = menus,
                depts = depts,
                rpts = rptJson,
                columns = columnList,
                roles = roles,
                buttons = buttonJson
            };
            return json;
            IEnumerable<string> GetRoleButtons()
            {
                foreach (var rbtn in roleButtons)
                {
                    if (rbtn.ButtonType == FapMenuButtonType.Grid || rbtn.ButtonType == FapMenuButtonType.Tree)
                    {
                        foreach (var v in rbtn.ButtonValue.SplitComma())
                        {
                            yield return $"{rbtn.MenuUid}|{rbtn.ButtonType}|{ rbtn.ButtonId }|{v}";
                        }
                    }
                    else
                    {
                        yield return $"{rbtn.MenuUid}|{rbtn.ButtonType}|{ rbtn.ButtonId }|{rbtn.ButtonValue}";
                    }
                }
            }
        }
    }

}
