using Fap.Core.Infrastructure.License;
using Fap.Core.Rbac.AC;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Domain
{
    /// <summary>
    /// FAP平台域接口
    /// </summary>
    public interface IFapPlatformDomain
    {
        ///// <summary>
        ///// 宿主标识
        ///// </summary>
        //Guid Id { get; }
        ///// <summary>
        ///// 宿主名称
        ///// </summary>
        //string Name { get; }
        /// <summary>
        /// 产品
        /// </summary>
        string Product { get; set; }
        void InitPlatformDomain();
        /// <summary>
        /// 平台注册码信息
        /// </summary>
        RegisterInfo ServiceRegisterInfo { get; }
        IColumnSet ColumnSet { get; }
        IDictSet DictSet { get; }
        IMenuSet MenuSet { get; }
        IMenuButtonSet MenuButtonSet { get; }
        IModuleSet ModuleSet { get; }
        IMultiLang MultiLangSet { get; }
        IOrgDeptSet OrgDeptSet { get; }
        IRoleColumnSet RoleColumnSet { get; }
        IRoleDataSet RoleDataSet { get; }
        IRoleDeptSet RoleDeptSet { get; }
        IRoleMenuSet RoleMenuSet { get; }
        IRoleReportSet RoleReportSet { get; }
        IRoleRoleSet RoleRoleSet { get; }
        IRoleSet RoleSet { get; }
        ISysParamSet SysParamSet { get; }
        ISysUserSet SysUserSet { get; }
        ITableSet TableSet { get; }
        ICfgBillCodeRuleSet CfgBillCodeRuleSet { get; }
        void Refresh();
    }
}
