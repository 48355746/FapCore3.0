using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.DataAccess
{
    internal class DbConstants
    {
        /// <summary>
        /// SqlServer
        /// </summary>
        internal static string DB_DIALECT_MSSQL = "mssql";
        /// <summary>
        /// MySql
        /// </summary>
        internal static string DB_DIALECT_MYSQL = "mysql";
        /// <summary>
        /// Oracle
        /// </summary>
        internal static string DB_DIALECT_ORACLE = "oracle";

    }
    public class FapDbConstants
    {
        public const string FAPCOLUMN_FIELD_Id = "Id";
        public const string FAPCOLUMN_FIELD_Fid = "Fid";
        public const string FAPCOLUMN_FIELD_OrgUid = "OrgUid";
        public const string FAPCOLUMN_FIELD_GroupUid = "GroupUid";
        public const string FAPCOLUMN_FIELD_EnableDate = "EnableDate";
        public const string FAPCOLUMN_FIELD_DisableDate = "DisableDate";
        public const string FAPCOLUMN_FIELD_Dr = "Dr";
        public const string FAPCOLUMN_FIELD_Ts = "Ts";
        public const string FAPCOLUMN_FIELD_CreateBy = "CreateBy";
        public const string FAPCOLUMN_FIELD_CreateName = "CreateName";
        public const string FAPCOLUMN_FIELD_CreateDate = "CreateDate";
        public const string FAPCOLUMN_FIELD_UpdateBy = "UpdateBy";
        public const string FAPCOLUMN_FIELD_UpdateName = "UpdateName";
        public const string FAPCOLUMN_FIELD_UpdateDate = "UpdateDate";
        public const string FAPCOLUMN_FIELD_CurrentDate = "CurrentDate";
        //参数
        public const string FAPCOLUMN_PARAM_CurrentDate = "@CurrentDate";
        public const string FAPCOLUMN_PARAM_Dr = "@Dr";

        #region 默认值常量
        /// <summary>
        /// 当前日期默认值
        /// </summary>
        public const string CurrentDate = "${FAP::CURRENTDATE}";
        /// <summary>
        /// 当前登录员工UID默认值
        /// </summary>
        public const string CurrentEmployee = "${FAP::CURRENTEMPLOYEE}";
        /// <summary>
        /// 当前登录用户UID
        /// </summary>
        public const string CurrentUser = "${FAP::CURRENTUSER}";
        /// <summary>
        /// 当前登录员工所在部门UID
        /// </summary>
        public const string CurrentDept = "${FAP::CURRENTDEPT}";
        /// <summary>
        /// 当前登录员工所在部门编码
        /// </summary>
        public const string CurrentDeptCode = "${FAP::CURRENTDEPTCODE}";
        /// <summary>
        /// 员工不带权限标识符
        /// </summary>
        public const string EmployeeNoPower = "${FAP::EMPLOYEENOPOWER}";
        /// <summary>
        /// UUID 
        /// </summary>
        public const string UUID = "${FAP::UUID}";
        #endregion

        /// <summary>
        /// 变量替换正则表达式${Variable}
        /// </summary>
        public const string VariablePattern = @"\$\{\S+?\}";
        /// <summary>
        /// 集合替换正则表达式
        /// </summary>
        public const string CollectionPattern = @"\{\{\S+?\}\}";
        /// <summary>
        ///部门权限替换字符
        /// </summary>

        public const string DepartmentAuthority = "FAP::&&DEPTAUTHORITY&&";
    }
}
