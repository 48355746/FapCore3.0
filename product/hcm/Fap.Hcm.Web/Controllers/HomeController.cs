using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fap.Hcm.Web.Models;
using Fap.Core;
using Fap.Core.Extensions;
using Fap.Core.Rbac;
using Fap.AspNetCore.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Fap.Core.Rbac.Model;
using Fap.Core.Utility;
using Fap.Core.Infrastructure.Domain;

namespace Fap.Hcm.Web.Controllers
{
    public class HomeController : FapController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILoginService _loginService;
        private readonly IRbacService _rbacService;
        public HomeController(IServiceProvider serviceProvider, ILoginService loginService, IRbacService rbacService) : base(serviceProvider)
        {
            _logger = _loggerFactory.CreateLogger<HomeController>();
            _loginService = loginService;
            _rbacService = rbacService;
        }
        [AllowAnonymous]
        public IActionResult Index(string Lang, string ReturnUrl, string msg)
        {
            string language = "ZhCn";
            if (Lang.IsPresent())
            {
                language = Lang;
            }
            HttpContext.Items["language"] = language;
            if (ReturnUrl.IsPresent())
            {
                ViewBag.ReturnUrl = ReturnUrl;
            }
            if (msg.IsPresent())
            {
                ViewBag.ErrorMsg = msg;
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Logon(string username, string userpwd, string language, string returnUrl)
        {
            string errorMsg = string.Empty;
            string languge = language.IsMissing() ? "ZhCn" : language;
            //管理员账号
            var developer =FapPlatformConstants.Administrator;
            //获取用户
            FapUser loginUser = _loginService.Login(username);
            Employee emp = null;
            ValideUser();
            if (!string.IsNullOrEmpty(errorMsg))
            {
                string loginUrl = _configService.GetSysParamValue(LoginUrl);// FapPlatformConfig.PlatformLoginUrl;
                if (loginUrl.IsNullOrEmpty())
                {
                    loginUrl = "~/";
                }
                return Redirect(loginUrl + "?msg=" + System.Net.WebUtility.UrlEncode(errorMsg));
            }
            //更新最近登录时间
            loginUser.LastLoginTime = PublicUtils.GetSysDateTimeStr();
            loginUser.PasswordTryTimes = 0;
            _loginService.UpdateLastLoginTime(loginUser);
            //添加在线用户
            var userRoles = _loginService.GetUserRoles(loginUser.Fid);
            FapRole currRole = userRoles.First();
            FapOnlineUser onlineUser = new FapOnlineUser()
            {
                Fid = PublicUtils.GetFid(),
                UserUid = loginUser.Fid,
                ClientIP = this.GetRemoteIPAddress(),
                DeptUid = emp.DeptUid,
                EmpUid = emp.Fid,
                RoleUid = currRole.Fid,
                LoginName = loginUser.UserName,
                LoginTime = PublicUtils.CurrentDateTimeStr,
                OnlineState = FapOnlineUser.CONST_LOGON,
                EnableDate = PublicUtils.CurrentDateTimeStr,
                DisableDate = PublicUtils.PermanentTimeStr,
                Dr = 0,
                Ts = PublicUtils.Ts
            };
            OnlineUserManager oum = new OnlineUserManager(_dataAccessor);
            oum.AddOnlineUser(onlineUser);
            //初始化身份卡片
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginUser.UserName),//用户名
                    new Claim(ClaimTypes.NameIdentifier,loginUser.Fid),//用户Fid
                    new Claim(ClaimTypes.UserData, loginUser.UserIdentity),//员工Fid
                    new Claim(ClaimTypes.Surname,emp.EmpName),//员工姓名
                    new Claim(ClaimTypes.PrimarySid,emp.DeptUid??"-"),//员工部门
                    new Claim(ClaimTypes.PrimaryGroupSid,emp.DeptCode??""),//部门编码
                    new Claim(ClaimTypes.System,emp.DeptUidMC??""),//部门名称
                    new Claim(ClaimTypes.DenyOnlyPrimaryGroupSid,emp.GroupUid??""),//集团
                    new Claim(ClaimTypes.DenyOnlyPrimarySid,emp.OrgUid??""),//组织
                    new Claim(ClaimTypes.Sid,language??"ZhCn"),//语言
                    new Claim(ClaimTypes.Actor,onlineUser.Fid)//在线用户Fid
                };
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Fid));
            }
            //组装身份
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = rememberme,
                // Whether the authentication session is persisted across 
                // multiple requests. Required when setting the 
                // ExpireTimeSpan option of CookieAuthenticationOptions 
                // set with AddCookie. Also required when setting 
                // ExpiresUtc.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal, authProperties);
            #region AcSession设置
            IFapAcSession acSession = new FapAcSession(loginUser, emp, onlineUser, currRole, (MultiLanguageEnum)Enum.Parse(typeof(MultiLanguageEnum), languge));
            //放入缓存把acSession,键-在线用户Fid
            _cache.Add(onlineUser.Fid, acSession, TimeSpan.FromMinutes(30));
            #endregion
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                if (userpwd == _configService.GetSysParamValue("employee.user.password"))
                {
                    //等于默认密码需要跳转到修改密码页
                    return LocalRedirect("~/Home/MainFrame#SelfService/Ess/ResetPassword/true");
                }
                else
                {
                    return LocalRedirect(_configService.GetSysParamValue(HomeUrl));
                }
            }
            else
            {
                return LocalRedirect(returnUrl);
            }
            void ValideUser()
            {
                PasswordHasher passwordHasher = new PasswordHasher();
                if (loginUser == null)
                {
                    errorMsg = "不存在此用户";
                }
                else if (loginUser.EnableState == 0)
                {
                    errorMsg = "对不起，该账户已被禁用";
                }
                else if (loginUser.IsLocked == 1)
                {
                    errorMsg = "对不起，该账户暂被锁定,请联系管理员解锁。";
                }
                else if (!passwordHasher.VerifyHashedPassword(loginUser.UserPassword, userpwd))
                {
                    errorMsg = "密码不正确";
                    //增加尝试次数，超过5次冻结
                    _loginService.AddTryTimes(loginUser);
                }
                else if (loginUser.UserIdentity.IsMissing() && loginUser.UserName != developer)
                {
                    errorMsg = "此用户没有关联人员信息";
                }
                else
                {
                    if (loginUser.UserIdentity.IsMissing())
                    {//开发者情况
                        emp = new Employee { Fid = "00000000000000000000", EmpCode = "Developer", EmpName = "开发者" };
                    }
                    else
                    {
                        emp = _dbContext.Get<Employee>(loginUser.UserIdentity, true);

                    }
                    if (emp == null)
                    {
                        errorMsg = "用户关联的人员不存在";
                    }
                }
            }
        }
        /// <summary>
        /// 集团注册
        /// </summary>
        /// <param name="fapGroup"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(FapGroup fapGroup)
        {
            _dataAccessor.Insert<FapGroup>(fapGroup);
            return LocalRedirect("/" + "?msg=" + System.Net.WebUtility.UrlEncode("注册成功，请登录。"));
        }

        /// <summary>
        /// ajax主框架
        /// </summary>
        /// <returns></returns>
        public IActionResult MainFrame()
        {
            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
