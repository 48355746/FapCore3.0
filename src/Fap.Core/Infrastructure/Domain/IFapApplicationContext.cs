using System.Collections.Generic;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.MultiLanguage;
using Fap.Core.Rbac.Model;
using Microsoft.AspNetCore.Http;

namespace Fap.Core.Infrastructure.Domain
{
    public interface IFapApplicationContext
    {
        bool IsAdministrator { get; }
        string DeptCode { get; }
        string DeptName { get; }
        string DeptUid { get; }
        string EmpName { get; }
        string EmpUid { get; }
        string EmpPhoto { get; }
        string GroupUid { get; }
        string OrgUid { get; }
        IEnumerable<string> Roles { get; }
        string CurrentRoleUid { get; set; }
        string UserName { get; }
        string UserType { get; }
        string UserUid { get; }
        HttpRequest Request { get; }
        ISession Session { get; }
        HttpResponse Response { get; }
        HttpContext HttpContext { get; }
        public string ClientIpAddress { get; }
        public string Broswer { get; }
        public string BaseUrl { get; }


        /// <summary>
        /// 多语语种
        /// </summary>
        MultiLanguageEnum Language { get; }
    }
}