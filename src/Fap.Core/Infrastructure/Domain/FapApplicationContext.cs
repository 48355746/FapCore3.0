
using Fap.Core.Extensions;
using Fap.Core.MultiLanguage;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Fap.Core.Infrastructure.Domain
{
    public class FapApplicationContext : IFapApplicationContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FapApplicationContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// 员工Fid
        /// </summary>
        public string EmpUid => _httpContextAccessor?.HttpContext == null ? "~" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;
        /// <summary>
        /// 员工姓名
        /// </summary>
        public string EmpName => _httpContextAccessor?.HttpContext == null ? "~" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
        /// <summary>
        /// 员工照片
        /// </summary>
        public string EmpPhoto => _httpContextAccessor?.HttpContext == null ? "~" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Actor)?.Value;
        /// <summary>
        /// 用户Fid
        /// </summary>
        public string UserUid => _httpContextAccessor?.HttpContext == null ? "~" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName => _httpContextAccessor?.HttpContext == null ? "~" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        /// <summary>
        /// 用户类型
        /// </summary>
        public string UserType => _httpContextAccessor?.HttpContext == null ? "~" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

        /// <summary>
        /// 部门Fid
        /// </summary>
        public string DeptUid => _httpContextAccessor?.HttpContext == null ? "~" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid)?.Value;
        /// <summary>
        /// 部门Code
        /// </summary>
        public string DeptCode => _httpContextAccessor?.HttpContext == null ? "~" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimaryGroupSid)?.Value;
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName => _httpContextAccessor?.HttpContext == null ? "~" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.System)?.Value;
        /// <summary>
        /// 组织
        /// </summary>
        public string OrgUid => _httpContextAccessor?.HttpContext == null ? "~" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.DenyOnlyPrimarySid)?.Value;
        /// <summary>
        /// 集团
        /// </summary>
        public string GroupUid => _httpContextAccessor?.HttpContext == null ? "~" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.DenyOnlyPrimaryGroupSid)?.Value;
        /// <summary>
        /// 租户
        /// </summary>
        public string TenantID => _httpContextAccessor?.HttpContext == null ? "0" : _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationInstant)?.Value;
        /// <summary>
        /// 当前语言
        /// </summary>
        public MultiLanguageEnum Language
        {
            get
            {
                if (_httpContextAccessor?.HttpContext?.Items["language"] != null)
                {
                    return (MultiLanguageEnum)Enum.Parse(typeof(MultiLanguageEnum), _httpContextAccessor?.HttpContext.Items["language"].ToString());
                }
                return _httpContextAccessor?.HttpContext == null ? MultiLanguageEnum.ZhCn : (MultiLanguageEnum)Enum.Parse(typeof(MultiLanguageEnum), _httpContextAccessor?.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value ?? "ZhCn");
            }
        }
        /// <summary>
        /// 所有角色UID
        /// </summary>
        public IEnumerable<string> Roles => _httpContextAccessor?.HttpContext == null ? Array.Empty<string>() : _httpContextAccessor?.HttpContext.User.FindAll(c => c.Type == ClaimTypes.Role)?.Select(r => r.Value);
        /// <summary>
        /// 当前角色
        /// </summary>
        public string CurrentRoleUid
        {
            get
            {
                string roleUid= Session.GetString(FapPlatformConstants.CurrentSessionRoleKey);
                if (roleUid.IsPresent())
                {
                    return roleUid;
                }
                return  HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;                
            }
            set
            {
                Session.SetString(FapPlatformConstants.CurrentSessionRoleKey, value);
            }
        }
        /// <summary>
        /// HttpRequest
        /// </summary>
        public HttpRequest Request => _httpContextAccessor?.HttpContext?.Request;
        public HttpResponse Response => _httpContextAccessor?.HttpContext.Response;
        public ISession Session => _httpContextAccessor?.HttpContext.Session;
        public HttpContext HttpContext => _httpContextAccessor?.HttpContext;
        public string ClientIpAddress => Request.Headers["X-Forwarded-For"].FirstOrDefault().IsMissing() ? HttpContext.Connection.RemoteIpAddress.ToString() : Request.Headers["X-Forwarded-For"].FirstOrDefault();
        public string Broswer => Request.Headers["User-Agent"].ToString();
        public string BaseUrl =>(Request.Headers["X-Forwarded-Proto"].FirstOrDefault().IsMissing()? $"{Request.Scheme}": Request.Headers["X-Forwarded-Proto"].FirstOrDefault()) +$"://{Request.Host.Host}:{Request.Host.Port}";
        /// <summary>
        /// 是否为管理员
        /// </summary>
        public bool IsAdministrator => UserName == FapPlatformConstants.Administrator;
    }
}
