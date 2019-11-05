using Fap.Core.Cache;
using Fap.Core.Configuration;
using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Constants;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Platform.Domain;
using Fap.Core.Rbac;
using Fap.Core.Utility;
using Fap.Core.Rbac.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Fap.Core.Rbac
{
    /// <summary>
    /// 基于Web的用户上下文贮存器，存储在Http Session中
    /// </summary>
    public class WebFapSessionStorage : IFapSessionStorage
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private ISession _webSession => _httpContextAccessor?.HttpContext == null ? null : _httpContextAccessor?.HttpContext.Session;
        private FapOption _fapOption;
        private ICacheService _cache;
        private ISessionFactory _sessionFactory;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="sessionFactory"></param>
        /// <param name="options"></param>
        /// <param name="cache"></param>
        public WebFapSessionStorage(IHttpContextAccessor httpContextAccessor, ISessionFactory sessionFactory, IOptions<FapOption> options, ICacheService cache)
        {
            _httpContextAccessor = httpContextAccessor;
            _fapOption = options.Value;
            _cache = cache;
            _sessionFactory = sessionFactory;
        }
        private HttpContext _context;
        public HttpContext Context
        {
            get
            {
                var context = _context ?? _httpContextAccessor?.HttpContext;
                if (context == null)
                {
                    throw new InvalidOperationException("HttpContext must not be null.");
                }
                return context;
            }
            set
            {
                _context = value;
            }
        }
        /// <summary>
        /// 员工Fid
        /// </summary>
        public string EmpUid => _httpContextAccessor?.HttpContext == null ? "-" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string EmpName => _httpContextAccessor?.HttpContext == null ? "-" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
        /// <summary>
        /// 用户Fid
        /// </summary>
        public string UserUid => _httpContextAccessor?.HttpContext == null ? "-" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName => _httpContextAccessor?.HttpContext == null ? "-" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        /// <summary>
        /// 用户类型
        /// </summary>
        public string UserType => _httpContextAccessor?.HttpContext == null ? "-" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        /// <summary>
        /// 部门Fid
        /// </summary>
        public string DeptUid => _httpContextAccessor?.HttpContext == null ? "-" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value;
        /// <summary>
        /// 部门Code
        /// </summary>
        public string DeptCode => _httpContextAccessor?.HttpContext == null ? "-" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimaryGroupSid)?.Value;
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName => _httpContextAccessor?.HttpContext == null ? "-" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.System)?.Value;
        /// <summary>
        /// 组织
        /// </summary>
        public string OrgUid => _httpContextAccessor?.HttpContext == null ? "-" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.DenyOnlyPrimarySid)?.Value;
        /// <summary>
        /// 集团
        /// </summary>
        public string GroupUid => _httpContextAccessor?.HttpContext == null ? "-" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.DenyOnlyPrimaryGroupSid)?.Value;
        /// <summary>
        /// 当前语言
        /// </summary>
        public MultiLanguageEnum Language => _httpContextAccessor?.HttpContext == null ? MultiLanguageEnum.ZhCn : (MultiLanguageEnum)Enum.Parse(typeof(MultiLanguageEnum), _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value ?? "ZhCn");
        /// <summary>
        /// 在线用户
        /// </summary>
        public string OnlineUserUid => _httpContextAccessor?.HttpContext == null ? "-" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value ?? "temp";
        /// <summary>
        /// 所有角色UID
        /// </summary>
        public IEnumerable<string> Roles => _httpContextAccessor?.HttpContext == null ? Array.Empty<string>() : _httpContextAccessor?.HttpContext.User.FindAll(c => c.Type == ClaimTypes.Role)?.Select(r=>r.Value);
        /// <summary>
        /// AcSession
        /// </summary>
        public IFapAcSession AcSession
        {
            get
            {
                if (_httpContextAccessor?.HttpContext == null)
                {
                    return null;
                }
                ClaimsPrincipal user = _httpContextAccessor?.HttpContext.User;
                if (!user.Identity.IsAuthenticated)
                {
                    //未登录
                    return null;
                }
                string key = OnlineUserUid;
                IFapAcSession acSession = _cache.Get<IFapAcSession>(key);

                if (acSession == null && user != null && user.Identity.IsAuthenticated)
                {
                    using (var dbSession = _sessionFactory.CreateSession())
                    {
                        FapUser fapUser = dbSession.Get<FapUser>(UserUid);
                        Employee employee = dbSession.Get<Employee>(EmpUid);
                        FapOnlineUser onlineUser = dbSession.Get<FapOnlineUser>(OnlineUserUid);

                        if (fapUser == null || onlineUser == null)
                        {
                            return null;
                            //throw new Exception("请重新登录");
                        }
                        FapRole fapRole = null;
                        if (onlineUser.RoleUid == PlatformConstants.CommonUserRoleFid)
                        {
                            //普通角色
                            fapRole = new FapRole { Id = -1, Fid = PlatformConstants.CommonUserRoleFid, RoleCode = "000", RoleName = "普通用户", RoleNote = "用户普通用户的授权" };
                        }
                        else
                        {
                            fapRole = dbSession.Get<FapRole>(onlineUser.RoleUid);
                        }
                        if (fapRole == null)
                        {
                            //普通角色
                            fapRole = new FapRole { Id = -1, Fid = PlatformConstants.CommonUserRoleFid, RoleCode = "000", RoleName = "普通用户", RoleNote = "用户普通用户的授权" };
                        }
                        acSession = new FapAcSession(fapUser, employee, onlineUser, fapRole, Language);
                        //放入cache
                        _cache.Add(key, acSession, TimeSpan.FromMinutes(30));
                    }

                }
                return acSession;
            }
        }

        /// <summary>
        /// root Url
        /// </summary>
        public string BaseURL
        {
            get
            {
                return _httpContextAccessor?.HttpContext == null ? "-" : _httpContextAccessor?.HttpContext.BaseUrl();
            }
        }
        /// <summary>
        /// 开发者账号
        /// </summary>
        public bool IsDeveloper => _httpContextAccessor?.HttpContext == null ? false : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value == _fapOption.AppSetting.Developer;



        /// <summary>
        /// 获取客户端IP
        /// </summary>
        public string ClientIPAddress {
            get
            {
                var ip = _httpContextAccessor?.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (string.IsNullOrEmpty(ip))
                {
                    ip = _httpContextAccessor?.HttpContext.Connection.RemoteIpAddress.ToString();
                }
                if(ip.Equals("::1"))
                {
                    ip = "127.0.0.1";
                }
                return ip;
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetData<T>(string key)
        {
            return _webSession.GetObjectFromJson<T>(key);
        }

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void SetData<T>(string key, T data)
        {
            _webSession.SetObjectAsJson(key, data);
        }
        /// <summary>
        /// 获取令牌（防止CSRF）
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetAndStoreTokens(string tableName)
        {
            string token = PublicUtils.GetFid();
            //保存的时候校验此值 (加上表名，避免连续打开连个表单，session就不同了)
            _webSession.SetObjectAsJson($"{tableName.ToLower()}formtoken", token);
            return token;
        }
        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            _webSession.Clear();
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key"></param>
        public void RemoveData(string key)
        {
            _webSession.Remove(key);
        }
    }
}
