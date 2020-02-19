using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.DataAccess.SqlParser
{
    interface IDDLSqlGenerator
    {
        string CreateTableSql(FapTable table, IEnumerable<FapColumn> columns);
        string AddMultiLangColumnSql(FapColumn fapColumn);
        string AlterMultiLangColumnSql(FapColumn fapColumn);
        string DropMultiLangColumnSql(FapColumn fapColumn);
        string CreateColumnSql(FapColumn fapColumn);
        string PhysicalTableColumnSql(string tableName);
        string AddColumnSql(FapColumn fapColumn);
        string AlterColumnSql(FapColumn fapColumn);
        string RenameColumnSql(FapColumn newColumn, string oldName);
        string DropColumnSql(FapColumn fapColumn);
        string DropTableSql(FapTable fapTable);
        string InsertSql(IDictionary<string, object> data, IEnumerable<FapColumn> columns);
    }
}
