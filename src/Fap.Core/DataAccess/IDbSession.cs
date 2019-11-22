using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Fap.Core.Metadata;

namespace Fap.Core.DataAccess
{
    public interface IDbSession
    {
        IConnectionFactory ConnectionFactory { get; }
        DatabaseDialectEnum DatabaseDialect { get; }
        bool Delete<T>(T entityToDelete) where T : class;
        bool DeleteAll<T>() where T : class;
        Task<bool> DeleteAllAsync<T>() where T : class;
        Task<bool> DeleteAsync<T>(T entityToDelete) where T : class;
        int Execute(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        Task<int> ExecuteAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        object ExecuteScalar(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        T ExecuteScalar<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        Task<object> ExecuteScalarAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        Task<T> ExecuteScalarAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        long Insert(string tableName, string sbColumnList, string sbParameterList, object entityToInsert);
        long Insert<T>(T entityToInsert) where T : class;
        Task<long> InsertAsync(string tableName, string sbColumnList, string sbParameterList, object entityToInsert);
        Task<int> InsertAsync<T>(T entityToInsert) where T : class;

        T Get<T>(int id) where T : class;
        IEnumerable<T> GetAll<T>() where T : class;
        Task<T> GetAsync<T>(int id) where T : class;
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
        IEnumerable<dynamic> Query(string sql, DynamicParameters parameters = null, bool buffered = true, CommandType? commandType = null);
        IEnumerable<T> Query<T>(string sql, DynamicParameters parameters = null, bool buffered = true, CommandType? commandType = null);
        Task<IEnumerable<dynamic>> QueryAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        
        dynamic QueryFirst(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        T QueryFirst<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        Task<dynamic> QueryFirstAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        Task<T> QueryFirstAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        dynamic QueryFirstOrDefault(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        T QueryFirstOrDefault<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        Task<dynamic> QueryFirstOrDefaultAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        dynamic QuerySingle(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        T QuerySingle<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        Task<dynamic> QuerySingleAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        Task<T> QuerySingleAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        dynamic QuerySingleOrDefault(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        T QuerySingleOrDefault<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        Task<dynamic> QuerySingleOrDefaultAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null);
        void TransactionProxy(Action<IDbConnection, IDbTransaction> execAction);
        bool Update(FapDynamicObject keyValues);
        bool Update<T>(T entityToUpdate) where T : class;
        bool UpdateWithTimestamp<T>(T entityToUpdate) where T : BaseModel;
        Task<bool> UpdateAsync<T>(T entityToUpdate) where T : class;

        public void BeginTransaction();

        public void Commit();

        public void Rollback();
        public void Dispose();
    }
}