using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.MultiLanguage;
using Fap.Core.Rbac.Model;
using Fap.Model.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Rbac
{
    [Service(ServiceLifetime.Singleton)]
    public class LoginService : ILoginService
    {
        private readonly IDbContext _dbContext;
        private readonly IRbacService _rbacService;
        private readonly IFapApplicationContext _applicationContext;
        private readonly IFapPlatformDomain _platformDomain;
        private readonly IMultiLangService _multiLangService;
        public LoginService(IDbContext dbContext, IRbacService rbacService,IFapApplicationContext applicationContext,IFapPlatformDomain platformDomain, IMultiLangService multiLangService)
        {
            _dbContext = dbContext;
            _rbacService = rbacService;
            _applicationContext = applicationContext;
            _platformDomain = platformDomain;
            _multiLangService = multiLangService;
        }
        //[Transactional]
        public FapUser Login(string userName)
        {
            string where = "UserName=@UserName";
            DynamicParameters param = new DynamicParameters();
            param.Add("UserName", userName);
            var user = _dbContext.QueryFirstOrDefaultWhere<FapUser>(where, param);
            if (user != null)
            {
                if (user.UserRole.IsMissing())
                {
                    user.UserRole = FapPlatformConstants.CommonUserRoleFid;
                }
                else
                {
                    //已经去除的角色还是默认角色的话 就会被替换为普通用户
                    var role = GetUserRoles(user.Fid).FirstOrDefault(r => r.Fid == user.UserRole);
                    if (role == null)
                    {
                        user.UserRole = FapPlatformConstants.CommonUserRoleFid;
                    }
                }
            }
            return user;
        }
        /// <summary>
        /// 添加尝试次数
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public FapUser AddTryTimes(FapUser user)
        {
            user.PasswordTryTimes += 1;
            //大于5次就冻结
            if (user.PasswordTryTimes > 5)
            {
                user.IsLocked = 1;
            }
            string sql = "update FapUser set passwordtrytimes=@trytimes,islocked=@islocked where id=@id";
            _dbContext.Execute(sql, new DynamicParameters(new { trytimes = user.PasswordTryTimes, islocked = user.IsLocked, id = user.Id }));
            return user;
        }
        public void UpdateUserDefaultRole(string roleUid, string userUid)
        {
            _dbContext.Execute($"update FapUser set {nameof(FapUser.UserRole)}=@RoleUid where Fid=@Fid", new DynamicParameters(new { RoleUid = roleUid, Fid = userUid }));
        }
        /// <summary>
        /// 更新最后登录时间
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public FapUser UpdateLastLoginTime(FapUser user)
        {
            string sql = "update FapUser set LastLoginTime=@lastTime, passwordtrytimes=@tryTimes where id=@id";
            _dbContext.Execute(sql, new DynamicParameters(new { lastTime = user.LastLoginTime, tryTimes = user.PasswordTryTimes, id = user.Id }));
            return user;
        }
        public void Logout()
        {

        }
        /// <summary>
        /// 获取用户默认角色
        /// </summary>
        /// <param name="userUid"></param>
        /// <returns></returns>
        public string GetUserDefaultRole(string userUid)
        {
            string roleUid = _dbContext.ExecuteScalar<string>($"select {nameof(FapUser.UserRole)} from {nameof(FapUser)} where Fid=@Fid", new DynamicParameters(new { Fid = userUid }));
            if (roleUid.IsMissing())
            {
                roleUid = FapPlatformConstants.CommonUserRoleFid;
            }
            return roleUid;
        }
        /// <summary>
        /// 获取用户的角色列表
        /// </summary>
        /// <param name="userUid"></param>
        /// <returns></returns>
        public IEnumerable<FapRole> GetUserRoles(string userUid)
        {

            string sql = "select * from FapRole where Fid in(select RoleUid  from FapRoleUser where UserUid=@UserUid)";
            DynamicParameters param = new DynamicParameters();
            param.Add("UserUid", userUid);
            var list = _dbContext.Query<FapRole>(sql, param);
            if (list == null)
            {
                list = new List<FapRole>();
            }
            var tempList = list.AsList();
            tempList.Insert(0, new FapRole { Id = -1, Fid = FapPlatformConstants.CommonUserRoleFid, RoleCode = "000", RoleName = "普通用户", RoleNote = "用户普通用户的授权" });
            return tempList;
        }
        /// <summary>
        /// 获取角色菜单
        /// </summary>
        /// <param name="roleUid"></param>
        /// <returns></returns>
        public IEnumerable<FapRoleMenu> GetRoleMenus(string roleUid)
        {
            return _rbacService.GetRoleMenuList(roleUid);
        }
        public List<MenuItem> BuildMenus()
        {
            string ceoModuleUid = "f4ce11e59cad54031d2f";
            //经理自主模块Fid
            string mgrModuleUid = "23b611e6b65e40b5abc8";
            List<MenuItem> menus = new List<MenuItem>();
            List<FapMenu> threeLevel = new List<FapMenu>();
            List<FapModule> modules = new List<FapModule>();
            bool isDeptManager = _rbacService.IsDeptManager();
            bool isCEO = _rbacService.IsCEO();
            //获取权限菜单
            IEnumerable<FapRoleMenu> roleMenuUids = _rbacService.GetRoleMenuList(_applicationContext.CurrentRoleUid);
            if (roleMenuUids.Any())
            {
                List<FapMenu> roleMenus = new List<FapMenu>();
                foreach (var rm in roleMenuUids)
                {
                    FapMenu fm;
                    if (_platformDomain.MenuSet.TryGetValue(rm.MenuUid, out fm))
                    {
                        //经理自助菜单权限 只有部门经理或者负责人具有
                        if (!isDeptManager && fm.ModuleUid == mgrModuleUid)
                        {
                            continue;
                        }
                        if (!isCEO && fm.ModuleUid == ceoModuleUid)
                        {
                            continue;
                        }
                        if (fm.MenuCode.Length > 5)
                        {
                            //如果没有二级菜单授权，这里要加上。
                            string parentCode = fm.MenuCode.Substring(0, 5);
                            if (!roleMenus.Exists(m => m.MenuCode == parentCode))
                            {
                                var pmenu = _platformDomain.MenuSet.FirstOrDefault<FapMenu>(m => m.MenuCode == parentCode);
                                if (pmenu != null)
                                {
                                    roleMenus.Add(pmenu);
                                }
                            }
                        }
                        if (!roleMenus.Contains(fm))
                        {
                            roleMenus.Add(fm);
                        }
                    }
                }
                var roleMenusOrder = roleMenus.OrderBy(m => m.MenuOrder);
                foreach (var fm in roleMenusOrder)
                {
                    //仅仅处理二级菜单
                    if (fm.MenuUrl.IsMissing())
                    {
                        continue;
                    }
                    if (fm.ActiveFlag == 0)
                    {
                        continue;
                    }
                    if (fm.MenuCode.Length > 5)
                    {
                        threeLevel.Add(fm);
                        continue;
                    }
                    string menuText = _multiLangService.GetMultiLangValue(MultiLanguageOriginEnum.Menu, fm.Fid);
                    MenuItem fmm = new MenuItem { Text = menuText, Value = fm.MenuCode, NavigateUrl = fm.MenuUrl, ToolTip = fm.BadgePlusClass };

                    FapModule fmd;
                    //是模块的时候
                    if (_platformDomain.ModuleSet.TryGetValue(fm.ModuleUid, out fmd))
                    {
                        MenuItem mmd = null;
                        menuText = _multiLangService.GetMultiLangValue(MultiLanguageOriginEnum.Module, fmd.Fid);
                        if (!modules.Contains(fmd))
                        {
                            modules.Add(fmd);
                            mmd = new MenuItem { Text = menuText, NavigateUrl = "", Value = fmd.ModuleCode, ImageUrl = fmd.Icon, Target = fmd.ModuleOrder.ToString() };
                            mmd.ChildItems.Add(fmm);
                            menus.Add(mmd);
                        }
                        else
                        {
                            mmd = menus.Find(m => m.Text == menuText);
                            mmd.ChildItems.Add(fmm);
                        }
                    }
                }
            }
            //管理员账号 增加系统管理菜单
            if (_applicationContext.UserName == FapPlatformConstants.Administrator)
            {
                //检查系统菜单，如果有就移除
                var sysModule = _platformDomain.ModuleSet.FirstOrDefault(m => m.ModuleCode == "099");
                if (sysModule != null)
                {
                    var sysMenu = menus.Find(m => m.Value == sysModule.ModuleCode);
                    if (sysMenu != null)
                    {
                        menus.Remove(sysMenu);
                        modules.Remove(sysModule);
                    }
                }
                //重新增加系统菜单
                IEnumerator<FapMenu> ms = _platformDomain.MenuSet.Where(m => m.MenuCode.StartsWith("099", System.StringComparison.OrdinalIgnoreCase)).OrderBy(m => m.MenuOrder).GetEnumerator();
                while (ms.MoveNext())
                {
                    FapMenu fm = ms.Current;
                    if (fm.ActiveFlag == 0) continue;

                    //仅仅处理二级菜单
                    if (fm.MenuUrl.IsMissing())
                    {
                        continue;
                    }
                    if (fm.MenuCode.Length > 5)
                    {
                        threeLevel.Add(fm);
                        continue;
                    }
                    string mx = _multiLangService.GetMultiLangValue(MultiLanguageOriginEnum.Menu, fm.Fid);
                    //菜单
                    MenuItem fmm = new MenuItem { Text = mx, NavigateUrl = fm.MenuUrl, Value = fm.MenuCode, ToolTip = fm.BadgePlusClass };

                    FapModule fmd;
                    if (_platformDomain.ModuleSet.TryGetValue(fm.ModuleUid, out fmd))
                    {
                        mx = _multiLangService.GetMultiLangValue(MultiLanguageOriginEnum.Module, fmd.Fid);
                        if (!modules.Contains(fmd))
                        {
                            modules.Add(fmd);
                            //模块
                            MenuItem mmd = new MenuItem { Text = mx, NavigateUrl = "", ImageUrl = fmd.Icon, Value = fmd.ModuleCode, Target = fmd.ModuleOrder.ToString() };

                            mmd.ChildItems.Add(fmm);
                            menus.Add(mmd);
                        }
                        else
                        {
                            menus.Find(m => m.Text == mx).ChildItems.Add(fmm);
                        }
                    }
                }
            }
            //处理三级菜单
            if (threeLevel.Any())
            {
                var threeMenus = threeLevel.OrderBy(m => m.MenuOrder);
                foreach (var menu in threeMenus)
                {
                    //找到所在的模块
                    var module = menus.Find(m => menu.MenuCode.StartsWith(m.Value, System.StringComparison.OrdinalIgnoreCase));
                    if (module != null && module.ChildItems != null && module.ChildItems.Count > 0)
                    {
                        foreach (MenuItem item in module.ChildItems)
                        {
                            if (menu.MenuCode.StartsWith(item.Value, System.StringComparison.OrdinalIgnoreCase) && item.Value.Length == 5)
                            {
                                string mx = _multiLangService.GetMultiLangValue(MultiLanguageOriginEnum.Menu, menu.Fid);
                                MenuItem fmm = new MenuItem { Text = mx, Value = menu.MenuCode, NavigateUrl = menu.MenuUrl, ToolTip = menu.BadgePlusClass };

                                item.ChildItems.Add(fmm);
                                break;
                            }
                        }

                    }

                }
            }
            return menus;           
        }
    }
}
