using Fap.Core.DataAccess.SqlParser;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

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
            FapTable table= _dbSession.Get<FapTable>(id);
            var columns =_dbSession.Query<FapColumn>("select * from FapColumn where TableName=@TableName",new Dapper.DynamicParameters(new { TableName = table.TableName }));
            var sql = GetSqlGenerator().CreateTable(table, columns);

        }
    }
}
