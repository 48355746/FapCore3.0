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
using Ardalis.GuardClauses;
using Fap.Core.MultiLanguage;
using Fap.ExcelReport;

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
        private readonly IMultiLangService _multiLangService;
        public ManageService(IRbacService rbacService, IDbContext dbContext, IFapConfigService configService, IFapApplicationContext applicationContext, IFapPlatformDomain platformDomain, IMultiLangService multiLangService)
        {
            _rbacService = rbacService;
            _dbContext = dbContext;
            _configService = configService;
            _applicationContext = applicationContext;
            _platformDomain = platformDomain;
            _multiLangService = multiLangService;
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
            else if (postData.Operation ==TreeNodeOper.MOVE_NODE)
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
            FapRole commonRole = roles.FirstOrDefault(r => r.Fid == FapPlatformConstants.CommonUserRoleFid);
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
                    if (!item.Id.EqualsWithIgnoreCase(FapPlatformConstants.CommonUserRoleFid))
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
            else if (postData.Operation ==TreeNodeOper.MOVE_NODE)
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
            List<TreeDataView> oriList = bizRoles.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid?.ToString(), Data = new { isRole = true }, Text = t.BizRoleName, Icon = "icon-folder orange ace-icon fa fa-users" }).ToList<TreeDataView>();

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
            else if (postData.Operation ==TreeNodeOper.MOVE_NODE)
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
            Guard.Against.Null(postData, nameof(postData));
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
            List<TreeDataView> moList = _platformDomain.ModuleSet.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid, Text = _multiLangService.GetMultiLangValue(MultiLanguageOriginEnum.Module, t.Fid), Icon = (t.Icon.IsMissing() ? "icon-folder green ace-icon fa fa-leaf" : "icon-folder green ace-icon " + t.Icon) }).ToList<TreeDataView>();

            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = _multiLangService.GetOrAndMultiLangValue(MultiLanguageOriginEnum.Page, "page_module_rootname", "系统模块"),
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
                .Select(t => new TreeDataView { Id = t.Fid.ToString(), Data = new { IsMenu = false }, Pid = t.Pid.ToString(), Text = _multiLangService.GetMultiLangValue(MultiLanguageOriginEnum.Module, t.Fid), State = new NodeState { Opened = false }, Icon = (t.Icon.IsMissing() ? "icon-folder green ace-icon fa fa-leaf" : "icon-folder green ace-icon " + t.Icon) }).ToList<TreeDataView>();
            //授权 仅仅授予到二级菜单
            List<TreeDataView> menuList = menus.Where(m => m.MenuCode.Length == 5).Select(r => new TreeDataView { Id = r.Fid.ToString(), Data = new { IsMenu = true }, Pid = r.ModuleUid, Text = _multiLangService.GetMultiLangValue(MultiLanguageOriginEnum.Menu, r.Fid), State = new NodeState { Opened = false }, Icon = "icon-folder orange ace-icon fa fa-leaf" }).ToList<TreeDataView>();
            List<TreeDataView> threeLevels = menus.Where(m => m.MenuCode.Length == 7).Select(r => new TreeDataView { Id = r.Fid.ToString(), Data = new { IsMenu = true }, Pid = r.Pid, Text = _multiLangService.GetMultiLangValue(MultiLanguageOriginEnum.Menu, r.Fid), State = new NodeState { Opened = false }, Icon = "icon-folder orange ace-icon fa fa-leaf" }).ToList<TreeDataView>();
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
            IEnumerable<OrgDept> powerDepts = _rbacService.GetDeptInfoAuthority(_applicationContext.CurrentRoleUid);
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
            var opers = typeof(OperEnum).EnumItems(_multiLangService).ToList();
            //权限中获取
            var roleButtons = _rbacService.GetRoleButtonList(_applicationContext.CurrentRoleUid);
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
               
                string menuUid = menuNode.Id;
                if (!_applicationContext.IsAdministrator)
                {
                   
                    if (roleButtons.Any())
                    {
                        var rbtns = roleButtons.Where(r => r.MenuUid == menuUid);
                        foreach (var btn in rbtns.Where(b => b.ButtonType == FapMenuButtonType.Grid || b.ButtonType == FapMenuButtonType.Tree))
                        {
                            var button = menuButtons.FirstOrDefault(b => b.MenuUid == btn.MenuUid && b.ButtonID == btn.ButtonId);
                            if (button != null)
                            {
                                string langkey = $"{menuUid}_{button.ButtonID}";
                                button.Description = _multiLangService.GetMultiLangValue(MultiLanguageOriginEnum.ButtonTag, langkey);
                                TreeDataView toper = new TreeDataView()
                                {
                                    Id = button.Fid,
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
                            string langkey = $"{menuUid}_{button.ButtonID}";
                            button.Description = _multiLangService.GetMultiLangValue(MultiLanguageOriginEnum.ButtonTag, langkey);
                            TreeDataView toper = new TreeDataView()
                            {
                                Id = button.Fid,
                                Data = new { IsBtn = false, IsMenu = false },
                                Pid = menuNode.Id,
                                Text = button.Description
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
                OperEnum.Chart=> "fa fa-bar-chart green",
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
                    if (!menuColumn.GridColumn.Trim().EqualsWithIgnoreCase("*"))
                    {
                        var colNames = menuColumn.GridColumn.SplitComma();
                        columns = columns.Where(c => colNames.Contains(c.ColName, new FapStringEqualityComparer()));
                    }
                    //实体属性
                    foreach (var column in columns)
                    {
                        TreeDataView tcol = new TreeDataView()
                        {
                            Id = $"{menuUid}|{menuColumn.GridId}|{column.Fid}",
                            Data = new { IsColumn = true, MenuUid = menuUid, GridId = menuColumn.GridId, ColumnUid = column.Fid },
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
        public IEnumerable<TreeDataView> GetSimpleReportTree()
        {
            var templates = _dbContext.QueryAll<RptSimpleTemplate>();
            if (!_applicationContext.IsAdministrator)
            {
                var roleReports = _rbacService.GetRoleReportList(_applicationContext.CurrentRoleUid);
                if (roleReports.Any())
                {
                    var prr = roleReports.Select(r => r.RptUid);
                    templates = templates.Where(r => (prr.Contains(r.Fid)||r.IsDir==1));
                }
                else
                {
                    templates = Enumerable.Empty<RptSimpleTemplate>();
                }
            }
            List<TreeDataView> oriList = templates.Select(t => new TreeDataView { Id = t.Fid, Pid = (t.Pid.IsMissing() ? "0" : t.Pid), Data = new { isRpt = true,isDir=t.IsDir }, Text = t.ReportName, Icon = (t.IsDir == 1 ? "icon-folder blue ace-icon fa fa-folder" : "icon-folder orange ace-icon fa fa-file-text-o") }).ToList<TreeDataView>();
            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "报表",
                Data = new { isRpt = false,isDir=1 },
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-folder",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);
            return tree;
        }
        public AuthorityViewModel GetAuthority(string roleUid)
        {
            DynamicParameters dparam = new DynamicParameters();
            dparam.Add("RoleUid", roleUid);

            //获取角色部门
            var depts = _rbacService.GetRoleDeptList(roleUid).Select(r => r.DeptUid);
            //获取角色菜单
            var menus = _rbacService.GetRoleMenuList(roleUid).Select(r => r.MenuUid);
            //获取角色角色
            var roles = _rbacService.GetRoleRoleList(roleUid).Select(r => r.PRoleUid);
            //获取角色按钮
            var roleButtons = _rbacService.GetRoleButtonList(roleUid);
            //获取角色用户
            IEnumerable<FapUser> userList =_dbContext.Query<FapUser>($"select Fid, UserCode,UserName,UserEmail,UserIdentity from {nameof(FapUser)} where fid in(select {nameof(FapRoleUser.UserUid)} from {nameof(FapRoleUser)} where RoleUid=@RoleUid) order by UserCode", dparam, true);
            //获取角色报表
            var rpts = _rbacService.GetRoleReportList(roleUid).Select(r => r.RptUid);
            //获取角色实体属性
            IEnumerable<FapRoleColumn> columnList = _rbacService.GetRoleColumnList(roleUid);// _dbContext.Query("select ColumnUid,EditAble,ViewAble from FapRoleColumn where RoleUid=@RoleUid", dparam);

            var buttonJson = GetRoleButtons();
            return new AuthorityViewModel()
            {
                Users = userList,
                Menus = menus,
                Depts = depts,
                Rpts = rpts,
                Columns = columnList,
                Roles = roles,
                Buttons = buttonJson
            };
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

        public ResponseViewModel SaveAuthority(AuthorityModel authority)
        {
            bool success = false;
            //菜单
            if (authority.AType == AuthorityTypeEnum.Menu)
            {
                List<FapRoleMenu> menus = new List<FapRoleMenu>();
                if (authority.MenuUids != null && authority.MenuUids.Any())
                {
                    foreach (var item in authority.MenuUids)
                    {
                        FapRoleMenu fdoMenu = new FapRoleMenu();
                        fdoMenu.RoleUid = authority.RoleUid;
                        fdoMenu.MenuUid = item;
                        menus.Add(fdoMenu);
                    }
                }
                success = _rbacService.AddRoleMenu(authority.RoleUid, menus);
                _platformDomain.RoleMenuSet.Refresh();
            }
            else if (authority.AType == AuthorityTypeEnum.Dept)
            {//部门
                List<FapRoleDept> depts = new List<FapRoleDept>();
                if (authority.OrgDeptUids != null && authority.OrgDeptUids.Any())
                {
                    foreach (var item in authority.OrgDeptUids)
                    {
                        FapRoleDept fdoDept = new FapRoleDept();
                        fdoDept.RoleUid = authority.RoleUid;
                        fdoDept.DeptUid = item;
                        depts.Add(fdoDept);
                    }
                }

                success = _rbacService.AddRoleDept(authority.RoleUid, depts);

                _platformDomain.RoleDeptSet.Refresh();
            }
            else if (authority.AType == AuthorityTypeEnum.ColumnEdit || authority.AType == AuthorityTypeEnum.ColumnView)
            {
                //实体列
                List<FapRoleColumn> columns = new List<FapRoleColumn>();
                if (authority.ColumnUids != null && authority.ColumnUids.Any())
                {
                    foreach (var item in authority.ColumnUids)
                    {
                        if (authority.AType == AuthorityTypeEnum.ColumnEdit)
                        {
                            columns.Add(new FapRoleColumn { RoleUid = authority.RoleUid, ColumnUid = item.ColUid, GridId = item.GridId, MenuUid = item.MenuUid, EditAble = 1, ViewAble = 0 });
                        }
                        else
                        {
                            columns.Add(new FapRoleColumn { RoleUid = authority.RoleUid, ColumnUid = item.ColUid, GridId = item.GridId, MenuUid = item.MenuUid, EditAble = 0, ViewAble = 1 });
                        }
                    }
                }
                success = _rbacService.AddRoleColumn(authority.RoleUid, columns, (int)authority.AType);
                //刷新应用程序全局域角色列
                _platformDomain.RoleColumnSet.Refresh();
            }
            else if (authority.AType == AuthorityTypeEnum.User)
            {
                List<FapRoleUser> users = new List<FapRoleUser>();
                //保存用户
                if (authority.UserUids != null && authority.UserUids.Any())
                {
                    foreach (var item in authority.UserUids)
                    {
                        users.Add(new FapRoleUser() { RoleUid = authority.RoleUid, UserUid = item });
                    }
                }
                if (users.Count > 0)
                {
                    _rbacService.AddRoleUser(users);
                    _platformDomain.RoleUserSet.TryGetUserValue(authority.RoleUid, out IEnumerable<string> rus);
                    IEnumerable<FapUser> userList = _dbContext.Query<FapUser>($"select Fid, UserCode,UserName,UserEmail,UserIdentity from {nameof(FapUser)} where Fid in @Fids  order by UserCode", new DynamicParameters(new { Fids =rus}), true);
                    return ResponseViewModelUtils.Sueecss(userList);
                }
                success = true;
            }
            else if (authority.AType == AuthorityTypeEnum.Rpt)
            {
                //报表
                List<FapRoleReport> rpts = new List<FapRoleReport>();
                if (authority.RptUids != null && authority.RptUids.Any())
                {
                    foreach (var item in authority.RptUids)
                    {
                        FapRoleReport fdoMenu = new FapRoleReport();
                        fdoMenu.RoleUid = authority.RoleUid;
                        fdoMenu.RptUid = item;
                        rpts.Add(fdoMenu);
                    }
                }
                _rbacService.AddRoleReport(authority.RoleUid, rpts);
                success = true;
                _platformDomain.RoleReportSet.Refresh();
            }
            else if (authority.AType == AuthorityTypeEnum.Role)
            {
                //角色
                List<FapRoleRole> rrs = new List<FapRoleRole>();
                if (authority.PRoleUids != null && authority.PRoleUids.Any())
                {
                    foreach (var item in authority.PRoleUids)
                    {
                        FapRoleRole fdoRR = new FapRoleRole();
                        fdoRR.RoleUid = authority.RoleUid;
                        fdoRR.PRoleUid = item;
                        rrs.Add(fdoRR);
                    }
                }
                _rbacService.AddRoleRole(authority.RoleUid, rrs);
                success = true;
                _platformDomain.RoleRoleSet.Refresh();
            }
            else if (authority.AType == AuthorityTypeEnum.Button)
            {
                //按钮
                var roleButtons = GetRoleButtons(authority.BtnUids);
                _rbacService.AddRoleButton(authority.RoleUid, roleButtons);
                success = true;

                _platformDomain.RoleButtonSet.Refresh();
            }
            return new ResponseViewModel() { success = success };
            IEnumerable<FapRoleButton> GetRoleButtons(IList<string> btnUids)
            {
                if (authority.BtnUids != null)
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
                                FapRoleButton roleButton = new FapRoleButton { RoleUid = authority.RoleUid, MenuUid = btnGrp.Key };
                                roleButton.ButtonType = buttons.Key;
                                roleButton.ButtonValue = string.Join(',', buttons.ToList().Select(b => b.BtnValue));
                                roleButton.ButtonId = buttons.First().BtnId;
                                yield return roleButton;
                            }
                            else
                            {
                                foreach (var button in buttons)
                                {
                                    FapRoleButton roleButton = new FapRoleButton { RoleUid = authority.RoleUid, MenuUid = btnGrp.Key };
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
    }

}
