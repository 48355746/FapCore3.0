using Fap.Core.DataAccess.SqlParser;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Metadata;
using System;
using Dapper;
using System.Collections.Generic;
using System.Text;
using Fap.Core.Extensions;
using System.Linq;
using Fap.Core.Utility;

namespace Fap.Core.DataAccess
{
    [Service]
    public class DbMetadataContext : IDbMetadataContext
    {
        private IDbSession _dbSession;
        public DbMetadataContext(IDbSession dbSession)
        {
            _dbSession = dbSession;
        }
        private IDDLSqlGenerator GetSqlGenerator()
        {
            return _dbSession.DatabaseDialect switch
            {
                DatabaseDialectEnum.MSSQL => new DDLMSSQLGenerator(),
                DatabaseDialectEnum.MYSQL => new DDLMYSQLGenerator(),
                _ => throw new NotImplementedException()
            };
        }
        private IDDLSqlGenerator GetSqlGenerator(DatabaseDialectEnum specifyDialect)
        {
            return specifyDialect switch
            {
                DatabaseDialectEnum.MSSQL => new DDLMSSQLGenerator(),
                DatabaseDialectEnum.MYSQL => new DDLMYSQLGenerator(),
                _ => throw new NotImplementedException()
            };
        }
        public void CreateTable(int id)
        {
            _dbSession.TransactionProxy((connection, transaction) =>
            {
                FapTable table = connection.QueryFirstOrDefault<FapTable>("select * from FapTable where Id=@Id", new { Id = id }, transaction);
                if (table != null)
                {
                    var columns = connection.Query<FapColumn>("select * from FapColumn where TableName=@TableName", new { TableName = table.TableName }, transaction);
                    var sql = GetSqlGenerator().CreateTableSql(table, columns);
                    connection.Execute(sql, null, transaction);
                    connection.Execute("Update FapTable set IsSync=1 where Id=@Id", new { Id = id }, transaction);
                }
            });
        }

        public void AddColumn(FapColumn fapColumn)
        {
            var generator = GetSqlGenerator();
            string sql = generator.AddColumnSql(fapColumn);
            _dbSession.Execute(sql);
        }

        public void AlterColumn(FapColumn fapColumn)
        {
            string sql = GetSqlGenerator().AlterColumnSql(fapColumn);
            _dbSession.Execute(sql);
        }

        public void DropColumn(FapColumn column)
        {
            string sql = GetSqlGenerator().DropColumnSql(column);
            _dbSession.Execute(sql);
        }

        public void RenameColumn(FapColumn newColumn, string oldName)
        {
            string sql = GetSqlGenerator().RenameColumnSql(newColumn, oldName);
            _dbSession.Execute(sql);
        }

        public void AddMultiLangColumn(FapColumn fapColumn)
        {
            string sql = GetSqlGenerator().AddMultiLangColumnSql(fapColumn);
            _dbSession.Execute(sql);
        }

        public void DropMultiLangColumn(FapColumn fapColumn)
        {
            string sql = GetSqlGenerator().DropMultiLangColumnSql(fapColumn);
            _dbSession.Execute(sql);
        }
        public string ExportSql(DatabaseDialectEnum databaseDialect, string tableName, string tableCategory, bool includCreate, bool includInsert)
        {
            StringBuilder sql = new StringBuilder();
            var generator = GetSqlGenerator(databaseDialect);
            if (tableName.IsPresent())
            {
                ExportSingleTableSql(tableName);
            }
            else if (tableCategory.IsPresent())
            {
                var tables = _dbSession.Query("select TableName from FapTable where TableCategory=@Category", new DynamicParameters(new { Category = tableCategory }));
                //创建表在前，元数据，表数据在后
                if (includCreate)
                {
                    foreach (var table in tables)
                    {
                        sql.AppendLine(ExportCreateSql(generator, table.TableName));
                    }
                }
                foreach (var table in tables)
                {
                    ExportSingleTableSql(table.TableName);
                }
            }
            return sql.ToString();
            void ExportSingleTableSql(string tn)
            {
                sql.AppendLine(ExportMetadataSql(generator, tn));
                if (includInsert)
                {
                    sql.AppendLine(ExportInsertSql(generator, tn));
                }
            }
        }
        private string ExportCreateSql(IDDLSqlGenerator generator, string tableName)
        {
            FapTable table = _dbSession.QueryFirstOrDefault<FapTable>("select * from FapTable where TableName=@TableName and Dr=0", new DynamicParameters(new { TableName = tableName }));
            var columns = _dbSession.Query<FapColumn>("select * from FapColumn where TableName=@TableName and Dr=0", new DynamicParameters(new { TableName = tableName }));
            return new StringBuilder(generator.DropTableSql(table)).AppendLine().AppendLine("GO").AppendLine(generator.CreateTableSql(table, columns)).AppendLine("GO").ToString();
        }
        private string ExportInsertSql(IDDLSqlGenerator generator, string tableName)
        {
            //元数据表中的数据 不导出insert所有语句
            if (tableName.EqualsWithIgnoreCase(nameof(FapTable)) || tableName.EqualsWithIgnoreCase(nameof(FapColumn)))
            {
                return string.Empty;
            }
            string sql = generator.PhysicalTableColumnSql(tableName);
            var fapColumns = _dbSession.Query<FapColumn>(sql);
            sql = $"select * from {tableName}";
            var datas = _dbSession.Query(sql);
            StringBuilder tableSql = new StringBuilder();
            foreach (var d in datas)
            {
                tableSql.AppendLine(generator.InsertSql(d as IDictionary<string, object>, fapColumns)).AppendLine("GO");
            }
            return tableSql.ToString();
        }
        private string ExportMetadataSql(IDDLSqlGenerator generator, string tableName)
        {
            var data = _dbSession.QueryFirstOrDefault("select * from FapTable where TableName=@TableName and Dr=0", new DynamicParameters(new { TableName = tableName }));
            var dataList = _dbSession.Query("select * from FapColumn where TableName=@TableName and Dr=0", new DynamicParameters(new { TableName = tableName }));
            string sql = generator.PhysicalTableColumnSql(nameof(FapTable));
            var fapColumns = _dbSession.Query<FapColumn>(sql);
            StringBuilder metaSql = new StringBuilder();
            metaSql.AppendLine("--FapTable元数据");
            metaSql.AppendLine(generator.InsertSql(data as IDictionary<string, object>, fapColumns)).AppendLine("GO");
            metaSql.AppendLine("--FapColumn元数据");
            sql = generator.PhysicalTableColumnSql(nameof(FapColumn));
            fapColumns = _dbSession.Query<FapColumn>(sql);
            foreach (var d in dataList)
            {
                metaSql.AppendLine(generator.InsertSql(d as IDictionary<string, object>, fapColumns)).AppendLine("GO");
            }
            return metaSql.ToString();
        }
    }
}
