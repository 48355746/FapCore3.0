using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Fap.Core.Utility
{
    public class SqlUtils
    {
        /// <summary>
        /// sql描述信息转化为sql
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="sqlDesc"></param>
        /// <returns></returns>
        public static string ParsingSql(IEnumerable<FapColumn> cols, string sqlDesc, DatabaseDialectEnum dialect)
        {

            Regex rgx = new Regex(@"\{\S+\}");
            MatchCollection matchs = rgx.Matches(sqlDesc);
            foreach (var mtch in matchs)
            {
                var colLabel = mtch.ToString().TrimStart('{').TrimEnd('}').Trim();
                sqlDesc = sqlDesc.ReplaceIgnoreCase(mtch.ToString(), cols.FirstOrDefault(c => c.ColComment == colLabel)?.ColName ?? "");
            }
            ReplaceConstant();
            ReplaceFunc();
            return sqlDesc;
            void ReplaceFunc()
            {
                if (dialect == DatabaseDialectEnum.MSSQL)
                {
                    sqlDesc = sqlDesc
                    .ReplaceIgnoreCase("[小时](", "DATEDIFF(hh,")
                    .ReplaceIgnoreCase("[天](", "DATEDIFF(dd,")
                    .ReplaceIgnoreCase("[星期](", "TimeStampDiff(wk,")
                    .ReplaceIgnoreCase("[月](", "TimeStampDiff(mm,")
                    .ReplaceIgnoreCase("[季度](", "TimeStampDiff(qq,")
                    .ReplaceIgnoreCase("[年](", "DATEDIFF(yy,")
                    .ReplaceIgnoreCase("[绝对值](", "ABS(");
                }
                else if (dialect == DatabaseDialectEnum.MYSQL)
                {
                    sqlDesc = sqlDesc
                  .ReplaceIgnoreCase("[小时](", "TimeStampDiff(HOUR,")
                  .ReplaceIgnoreCase("[天](", "TimeStampDiff(DAY,")
                  .ReplaceIgnoreCase("[星期](", "TimeStampDiff(WEEK,")
                  .ReplaceIgnoreCase("[月](", "TimeStampDiff(MONTH,")
                  .ReplaceIgnoreCase("[季度](", "TimeStampDiff(QUARTER,")
                  .ReplaceIgnoreCase("[年](", "TimeStampDiff(YEAR,")
                  .ReplaceIgnoreCase("[绝对值](", "ABS(");
                }
            }
            void ReplaceConstant()
            {
                sqlDesc = sqlDesc.ReplaceIgnoreCase("[当前日期]", $"'{DateTimeUtils.CurrentDateStr}'");
            }
        }

    }
}
