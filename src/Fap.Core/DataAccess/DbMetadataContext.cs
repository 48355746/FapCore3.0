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
            string sql= generator.AddColumnSql(fapColumn);
            _dbSession.Execute(sql);
        }

        public void AlterColumn(FapColumn fapColumn)
        {
            string sql= GetSqlGenerator().AlterColumnSql(fapColumn);
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
    }
}
