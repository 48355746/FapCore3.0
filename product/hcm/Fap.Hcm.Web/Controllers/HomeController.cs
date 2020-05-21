using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fap.Hcm.Web.Models;
using Fap.Core.Extensions;
using Fap.Core.Rbac;
using Fap.AspNetCore.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Fap.Core.Rbac.Model;
using Fap.Core.Utility;
using Fap.Core.Infrastructure.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Fap.Core.Infrastructure.Model;
using System.IO;
using Fap.AspNetCore.ViewModel;
using Fap.Core.MultiLanguage;

namespace Fap.Hcm.Web.Controllers
{
    public class HomeController : FapController
    {
        private const string LoginUrl = "LoginUrl";
        private const string HomeUrl = "HomeUrl";
        private readonly ILogger<HomeController> _logger;
        private readonly ILoginService _loginService;
        private readonly IOnlineUserService _onlineUserService;
        public HomeController(IServiceProvider serviceProvider, ILoginService loginService, IOnlineUserService onlineUserService) : base(serviceProvider)
        {
            _logger = _loggerFactory.CreateLogger<HomeController>();
            _loginService = loginService;
            _onlineUserService = onlineUserService;
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
            string currLanguage = language.IsMissing() ? "ZhCn" : language;
            //管理员账号
            var developer = FapPlatformConstants.Administrator;
            //获取用户
            FapUser loginUser = _loginService.Login(username);
            Employee emp = null;
            LocalRedirectResult errorResult = CheckUser();
            if (errorResult != null)
            {
                return errorResult;
            }
            LoginLogging();
            var claimsPrincipal = CreateClaimsPrincipal();
            var authenticationProperties = CreateAuthenticationProperties();
            //设置当前角色为普通员工
            //_applicationContext.CurrentRoleUid =FapPlatformConstants.CommonUserRoleFid;
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal, authenticationProperties).ConfigureAwait(false);
            return Redirect();

            LocalRedirectResult CheckUser()
            {
                PasswordHasher passwordHasher = new PasswordHasher();
                if (loginUser == null)
                {
                    errorMsg = GetOrAddPageMultiLanguageContent("login_page_no_exist_user", "不存在此用户");
                }
                else if (loginUser.EnableState == 0)
                {
                    errorMsg = GetOrAddPageMultiLanguageContent("login_page_forbidden_user", "该账户已被禁用");
                }
                else if (loginUser.IsLocked == 1)
                {
                    errorMsg = GetOrAddPageMultiLanguageContent("login_page_lock_user", "该账户暂被锁定");
                }
                else if (!passwordHasher.VerifyHashedPassword(loginUser.UserPassword, userpwd))
                {
                    errorMsg = GetOrAddPageMultiLanguageContent("login_page_password_error", "密码不正确");
                    //增加尝试次数，超过5次冻结
                    _loginService.AddTryTimes(loginUser);
                }
                else if (loginUser.UserIdentity.IsMissing() && loginUser.UserName != developer)
                {
                    errorMsg = GetOrAddPageMultiLanguageContent("login_page_no_mapping_employee", "此用户没有关联人员信息");
                }
                else
                {
                    if (loginUser.UserIdentity.IsMissing())
                    {
                        if (loginUser.UserName.EqualsWithIgnoreCase(developer))
                        {
                            emp = new Employee { Fid = "00000000000000000000", EmpCode = "Administrator", EmpName = "Administrator" };
                        }
                        else
                        {
                            errorMsg = GetOrAddPageMultiLanguageContent("login_page_no_find_mapping_employee", "用户关联的人员不存在");
                        }
                    }
                    else
                    {
                        emp = _dbContext.QueryFirstOrDefault<Employee>("select Fid,EmpCode,EmpName,DeptUid,DeptCode,EmpPhoto,GroupUid,OrgUid from Employee where Fid=@Fid", new Dapper.DynamicParameters(new { Fid = loginUser.UserIdentity }), true);
                        if (emp == null)
                        {
                            errorMsg = GetOrAddPageMultiLanguageContent("login_page_no_find_mapping_employee", "用户关联的人员不存在"); ;
                        }
                    }
                }
                if (errorMsg.IsPresent())
                {
                    string loginUrl = _configService.GetSysParamValue(LoginUrl);// FapPlatformConfig.PlatformLoginUrl;
                    if (loginUrl.IsMissing())
                    {
                        loginUrl = "~/";
                    }
                    return LocalRedirect(loginUrl + "?msg=" + System.Net.WebUtility.UrlEncode(errorMsg));
                }
                return null;
            }
            void LoginLogging()
            {
                //更新最近登录时间
                loginUser.LastLoginTime = DateTimeUtils.CurrentDateTimeStr;
                loginUser.PasswordTryTimes = 0;
                _loginService.UpdateLastLoginTime(loginUser);                
            }
            ClaimsPrincipal CreateClaimsPrincipal()
            {
                //初始化身份卡片
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, loginUser.UserName),//用户名
                    new Claim(ClaimTypes.UserData,loginUser.Fid),//用户Fid
                    new Claim(ClaimTypes.NameIdentifier, loginUser.UserIdentity),//员工Fid
                    new Claim(ClaimTypes.Surname,emp.EmpName),//员工姓名
                    new Claim(ClaimTypes.PrimarySid,emp.DeptUid??"-"),//员工部门
                    new Claim(ClaimTypes.PrimaryGroupSid,emp.DeptCode??""),//部门编码
                    new Claim(ClaimTypes.System,emp.DeptUidMC??""),//部门名称
                    new Claim(ClaimTypes.DenyOnlyPrimaryGroupSid,emp.GroupUid??""),//集团
                    new Claim(ClaimTypes.DenyOnlyPrimarySid,emp.OrgUid??""),//组织
                    new Claim(ClaimTypes.Sid,currLanguage),//语言
                    new Claim(ClaimTypes.Actor,emp.EmpPhoto),//用户图像
                    new Claim(ClaimTypes.Role,loginUser.UserRole)//角色普通用户
                };
                
                //组装身份
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                return new ClaimsPrincipal(claimsIdentity);

            }
            AuthenticationProperties CreateAuthenticationProperties()
            {
                return new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    // Refreshing the authentication session should be allowed.

                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(1),
                    // The time at which the authentication ticket expires. A 
                    // value set here overrides the ExpireTimeSpan option of 
                    // CookieAuthenticationOptions set with AddCookie.

                    IsPersistent = true,
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
            }
            LocalRedirectResult Redirect()
            {
                if (returnUrl.IsMissing())
                {
                    if (userpwd == _configService.GetSysParamValue("employee.user.password"))
                    {
                        //等于默认密码需要跳转到修改密码页
                        return LocalRedirect("~/Home/MainFrame#Home/ResetPassword/1");
                    }
                    else
                    {
                        if (_rbacService.IsCEO(emp.Fid))
                        {
                            return LocalRedirect("~/Home/MainFrame#System/Report/CEOChart");
                        }
                        else
                        {
                            return LocalRedirect(_configService.GetSysParamValue(HomeUrl));
                        }
                    }
                }
                else
                {
                    return LocalRedirect(returnUrl);
                }
            }
           
        }

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            _applicationContext.Session.Clear();
            return LocalRedirect("/");
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
            _dbContext.Insert<FapGroup>(fapGroup);
            return LocalRedirect("/" + "?msg=" + System.Net.WebUtility.UrlEncode("注册成功，请登录。"));
        }
        /// <summary>
        /// 设置密码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            if (id.IsPresent() && id.ToBool())
            {
                ViewBag.IsOri = 1;
            }
            else
            {
                ViewBag.IsOri = 0;
            }
            return View();
        }
        /// <summary>
        /// 密码重置 
        /// </summary>
        /// <param name="frm"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ResetPassword(string op, string np, string cp)
        {
            PasswordHasher pwdHasher = new PasswordHasher();
            string msg = string.Empty;
            string oriPwd = op;
            string newPwd = np;
            string confirmPwd = cp;
            FapUser user = _dbContext.Get<FapUser>(_applicationContext.UserUid);
            if (!pwdHasher.VerifyHashedPassword(user.UserPassword, oriPwd))
            {
                msg = GetOrAddPageMultiLanguageContent("login_page_ori_password_error", "原始密码错误");
            }
            else
            {
                if (newPwd != confirmPwd)
                {
                    msg = GetOrAddPageMultiLanguageContent("login_page_password_confirm_error", "两次输入密码不一致");
                }
                else
                {
                    user.UserPassword = pwdHasher.HashPassword(newPwd);
                    user.PasswordTryTimes = 0;
                    _dbContext.Update<FapUser>(user);
                    msg = GetOrAddPageMultiLanguageContent("login_page_password_modifysuccess", "修改密码成功");
                }
            }
            return Json(ResponseViewModelUtils.Sueecss(msg));
        }
        /// <summary>
        /// 切换角色
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeRole(string fid)
        {
            //设置当前角色
            _applicationContext.CurrentRoleUid = fid;
            return LocalRedirect(_configService.GetSysParamValue(HomeUrl));
        }
        /// <summary>
        /// 用户角色列表
        /// </summary>
        /// <returns></returns>
        public IActionResult RoleList()
        {
            var userRoles = _loginService.GetUserRoles(_applicationContext.UserUid);
            string defaultRole = _loginService.GetUserDefaultRole(_applicationContext.UserUid);
            userRoles.FirstOrDefault(r => r.Fid == defaultRole).IsDefault = true;
            return PartialView(userRoles);
        }
        [HttpPost]
        public JsonResult UserRole(string roleUid)
        {
            _loginService.UpdateUserDefaultRole(roleUid, _applicationContext.UserUid);
            return Json(ResponseViewModelUtils.Sueecss());
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
        #region 游客
        [AllowAnonymous]
        public IActionResult Tourist(string fid)
        {
            LoginTourist(fid);
            return Redirect("~/Recruit/Manage/Profile/" + fid);
            void LoginTourist(string fid)
            {
                //初始化身份卡片
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, fid),//用户名
                    new Claim(ClaimTypes.NameIdentifier,fid),//员工Fid
                    new Claim(ClaimTypes.UserData,fid),//用户Fid
                    new Claim(ClaimTypes.Surname,"准员工"),//员工姓名
                    new Claim(ClaimTypes.PrimarySid,"-"),//员工部门
                    new Claim(ClaimTypes.PrimaryGroupSid,"-"),//部门编码
                    new Claim(ClaimTypes.System,"-"),//部门名称
                    new Claim(ClaimTypes.DenyOnlyPrimaryGroupSid,""),//集团
                    new Claim(ClaimTypes.DenyOnlyPrimarySid,""),//组织
                    new Claim(ClaimTypes.Sid,"ZhCn"),//语言
                    new Claim(ClaimTypes.Actor,"-"),//用户图像
                    new Claim(ClaimTypes.Role,"-1")//角色普通用户
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                var authenticationProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30),
                    RedirectUri = "~/Home/Tourist/" + fid
                };
                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal, authenticationProperties).ConfigureAwait(false);
            }
        }
        public async Task<IActionResult> Success()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);
            _applicationContext.Session.Clear();
            return View();
        }
        #endregion
    }
}
