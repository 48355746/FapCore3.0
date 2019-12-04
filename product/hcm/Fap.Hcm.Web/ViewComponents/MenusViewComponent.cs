using Dapper;
using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.MultiLanguage;
using Fap.Core.Rbac;
using Fap.Core.Rbac.Model;
using Fap.Model.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Metadata;

namespace Fap.Hcm.Web.ViewComponents
{
    public  class MenusViewComponent : ViewComponent
    {

        protected readonly IDbContext _dataAccessor;
        protected readonly IFapApplicationContext _applicationContext;
        protected readonly ILogger<MenusViewComponent> _logger;
        protected readonly IRbacService _rbacService;
        protected readonly IFapPlatformDomain _appDomain;
        protected readonly IMultiLangService _multiLangService;
        public MenusViewComponent(IFapPlatformDomain appDomain, IDbContext dataAccessor, IFapApplicationContext applicationContext, IRbacService rbacService, IMultiLangService multiLangService, ILogger<MenusViewComponent> logger)
        {
            _appDomain = appDomain;
            _dataAccessor = dataAccessor;
            _applicationContext = applicationContext;
            _rbacService = rbacService;
            _multiLangService = multiLangService;
            _logger = logger;
        }


        /// <summary>
        /// 异步调用
        /// </summary>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return await Task.FromResult(View(new MenuViewModel { Menus = BuildMenus().OrderBy(m => m.Target.ToInt()).ToList() }));
        }
        private List<MenuItem> BuildMenus()
        {
            //经理自主模块Fid
            string mgrModuleUid = "23b611e6b65e40b5abc8";
            List<MenuItem> menus = new List<MenuItem>();
            List<FapMenu> threeLevel = new List<FapMenu>();
            List<FapModule> modules = new List<FapModule>();
            //判断是否为部门负责人或者经理
            DynamicParameters param = new DynamicParameters();
            param.Add("EmpUid",_applicationContext.EmpUid);
            int mi = _dataAccessor.Count("OrgDept", "Director=@EmpUid or DeptManager=@EmpUid", param);

            //获取权限菜单
            IEnumerable<FapRoleMenu> roleMenuUids =_rbacService.GetUserMenuList();
            if (roleMenuUids.Any())
            {
                List<FapMenu> roleMenus = new List<FapMenu>();
                foreach (var rm in roleMenuUids)
                {
                    FapMenu fm;
                    if (_appDomain.MenuSet.TryGetValue(rm.MenuUid, out fm))
                    {
                        //经理自助菜单权限 只有部门经理或者负责人具有
                        if (mi == 0 && fm.ModuleUid == mgrModuleUid)
                        {
                            continue;
                        }
                        if (fm.MenuCode.Length > 5)
                        {
                            //如果没有二级菜单授权，这里要加上。
                            string parentCode = fm.MenuCode.Substring(0, 5);
                            if (!roleMenus.Exists(m => m.MenuCode == parentCode))
                            {
                                var pmenu = _appDomain.MenuSet.FirstOrDefault<FapMenu>(m => m.MenuCode == parentCode);
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
                    MenuItem fmm = new MenuItem { Text = _multiLangService.GetLangMenuName(fm), Value = fm.MenuCode, NavigateUrl = Url.Content(fm.MenuUrl), ToolTip = fm.BadgePlusClass };

                    FapModule fmd;
                    //是模块的时候
                    if (_appDomain.ModuleSet.TryGetValue(fm.ModuleUid, out fmd))
                    {
                        MenuItem mmd = null;
                        if (!modules.Contains(fmd))
                        {
                            modules.Add(fmd);
                            mmd = new MenuItem { Text = _multiLangService.GetLangModuleName(fmd), NavigateUrl = "", Value = fmd.ModuleCode, ImageUrl = fmd.Icon, Target = fmd.ModuleOrder.ToString() };
                            mmd.ChildItems.Add(fmm);
                            menus.Add(mmd);
                        }
                        else
                        {
                            mmd = menus.Find(m => m.Text == _multiLangService.GetLangModuleName(fmd));
                            mmd.ChildItems.Add(fmm);
                        }
                    }
                }
            }
            //管理员账号 增加系统管理菜单
            if (_applicationContext.UserName==FapPlatformConstants.Administrator)
            {
                //检查系统菜单，如果有就移除
                var sysModule = _appDomain.ModuleSet.FirstOrDefault(m => m.ModuleCode == "099");
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
                IEnumerator<FapMenu> ms = _appDomain.MenuSet.Where(m => m.MenuCode.StartsWith("099")).OrderBy(m => m.MenuOrder).GetEnumerator();
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

                    //菜单
                    MenuItem fmm = new MenuItem { Text = _multiLangService.GetLangMenuName(fm), NavigateUrl = Url.Content(fm.MenuUrl), Value = fm.MenuCode, ToolTip = fm.BadgePlusClass };

                    FapModule fmd;
                    if (_appDomain.ModuleSet.TryGetValue(fm.ModuleUid, out fmd))
                    {
                        if (!modules.Contains(fmd))
                        {
                            modules.Add(fmd);
                            //模块
                            MenuItem mmd = new MenuItem { Text = _multiLangService.GetLangModuleName(fmd), NavigateUrl = "", ImageUrl = fmd.Icon, Value = fmd.ModuleCode, Target = fmd.ModuleOrder.ToString() };

                            mmd.ChildItems.Add(fmm);
                            menus.Add(mmd);
                        }
                        else
                        {
                            menus.Find(m => m.Text == _multiLangService.GetLangModuleName(fmd)).ChildItems.Add(fmm);
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
                    var module = menus.Find(m => menu.MenuCode.StartsWith(m.Value));
                    if (module != null && module.ChildItems != null && module.ChildItems.Count > 0)
                    {
                        foreach (MenuItem item in module.ChildItems)
                        {
                            if (menu.MenuCode.StartsWith(item.Value) && item.Value.Length == 5)
                            {
                                MenuItem fmm = new MenuItem { Text = _multiLangService.GetLangMenuName(menu), Value = menu.MenuCode, NavigateUrl = Url.Content(menu.MenuUrl), ToolTip = menu.BadgePlusClass };

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