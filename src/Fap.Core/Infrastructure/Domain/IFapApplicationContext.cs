using System.Collections.Generic;
using Fap.Core.Infrastructure.Enums;

namespace Fap.Core.Infrastructure.Domain
{
    public interface IFapApplicationContext
    {
        string DeptCode { get; }
        string DeptName { get; }
        string DeptUid { get; }
        string EmpName { get; }
        string EmpUid { get; }
        string GroupUid { get; }
        MultiLanguageEnum Language { get; }
        string OnlineUserUid { get; }
        string OrgUid { get; }
        IEnumerable<string> Roles { get; }
        string UserName { get; }
        string UserType { get; }
        string UserUid { get; }
    }
}