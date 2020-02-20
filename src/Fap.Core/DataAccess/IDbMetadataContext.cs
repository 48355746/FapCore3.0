using Fap.Core.DI;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.DataAccess
{
    public interface IDbMetadataContext
    {
        void CreateTable(int id);
        void AddMultiLangColumn(FapColumn fapColumn);
        void DropMultiLangColumn(FapColumn fapColumn);
        void AddColumn(FapColumn fapColumn);
        void RenameColumn(FapColumn newColumn, string oldName);
        void RenameMultiLangColumn(FapColumn newColumn, string oldName);
        void AlterColumn(FapColumn fapColumn);
        void AlterMultiLangColumn(FapColumn fapColumn);
        void DropColumn(FapColumn fapColumn);
        string ExportSql(DatabaseDialectEnum databaseDialect,string tableName, string tableCategory, bool includCreate, bool includInsert);
        string GeneraterModelClass(FapTable table, IEnumerable<FapColumn> columns);
    }
}
