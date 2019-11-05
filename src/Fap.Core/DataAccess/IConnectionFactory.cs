using System.Data;

namespace Fap.Core.DataAccess
{
    public interface IConnectionFactory
    {
        DatabaseDialectEnum DatabaseDialect { get; }
        DataSourceEnum DataSource { get; set; }       
        IDbConnection GetDbConnection();
    }
}