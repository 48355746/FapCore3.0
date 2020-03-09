using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fap.Core.Extensions
{  
    public static class FapColumnExtensions
    {
        private static string[] AuditColumns =
        {
            FapDbConstants.FAPCOLUMN_FIELD_OrgUid,
            FapDbConstants.FAPCOLUMN_FIELD_GroupUid,
            FapDbConstants.FAPCOLUMN_FIELD_EnableDate,
            FapDbConstants.FAPCOLUMN_FIELD_DisableDate,
            FapDbConstants.FAPCOLUMN_FIELD_Dr,
            FapDbConstants.FAPCOLUMN_FIELD_Ts,
            FapDbConstants.FAPCOLUMN_FIELD_CreateBy,
            FapDbConstants.FAPCOLUMN_FIELD_CreateDate,
            FapDbConstants.FAPCOLUMN_FIELD_CreateName,
            FapDbConstants.FAPCOLUMN_FIELD_UpdateBy,
            FapDbConstants.FAPCOLUMN_FIELD_UpdateDate,
            FapDbConstants.FAPCOLUMN_FIELD_UpdateName
        };
        private static string[] BaseColumns =
        {
            FapDbConstants.FAPCOLUMN_FIELD_Id,
            FapDbConstants.FAPCOLUMN_FIELD_Fid,
            FapDbConstants.FAPCOLUMN_FIELD_OrgUid,
            FapDbConstants.FAPCOLUMN_FIELD_GroupUid,
            FapDbConstants.FAPCOLUMN_FIELD_EnableDate,
            FapDbConstants.FAPCOLUMN_FIELD_DisableDate,
            FapDbConstants.FAPCOLUMN_FIELD_Dr,
            FapDbConstants.FAPCOLUMN_FIELD_Ts,
            FapDbConstants.FAPCOLUMN_FIELD_CreateBy,
            FapDbConstants.FAPCOLUMN_FIELD_CreateDate,
            FapDbConstants.FAPCOLUMN_FIELD_CreateName,
            FapDbConstants.FAPCOLUMN_FIELD_UpdateBy,
            FapDbConstants.FAPCOLUMN_FIELD_UpdateDate,
            FapDbConstants.FAPCOLUMN_FIELD_UpdateName
        };
        /// <summary>
        /// 排除审计列
        /// </summary>
        /// <param name="fapColumns"></param>
        /// <returns></returns>
        public static IEnumerable<FapColumn> ExcludeAuditColumns(this IEnumerable<FapColumn> fapColumns)
        {
            return fapColumns.Where(f => !AuditColumns.Contains(f.ColName,new FapStringEqualityComparer()));
        }
        public static IEnumerable<string> ExcludeBaseColumns(this IEnumerable<string> columnNames)
        {
            return columnNames.Where(c => !BaseColumns.Contains(c, new FapStringEqualityComparer()));
        }
    }
}
