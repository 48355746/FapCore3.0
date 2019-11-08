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
    internal class FapDbConstants
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
    }
}
