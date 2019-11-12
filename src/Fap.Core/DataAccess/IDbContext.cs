using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Fap.Core.Metadata;

namespace Fap.Core.DataAccess
{
    public interface IDbContext
    {
        string HistoryDateTime { get; set; }

        void BeginTransaction();
        void Commit();
        int Count(string tableName, string where = "", DynamicParameters parameters = null);
        int Count<T>(string where = "", DynamicParameters parameters = null);
        Task<int> CountAsync(string tableName, string where = "", DynamicParameters parameters = null);
        Task<int> CountAsync<T>(string where = "", DynamicParameters parameters = null);
        bool Delete<T>(int id) where T : BaseModel;
        bool Delete<T>(string fid) where T : BaseModel;
        bool Delete<T>(T entityToDelete) where T : BaseModel;
        Task<bool> DeleteAsync<T>(int id) where T : BaseModel;
        Task<bool> DeleteAsync<T>(string fid) where T : BaseModel;
        Task<bool> DeleteAsync<T>(T entityToDelete) where T : BaseModel;
        bool DeleteBatch<T>(IEnumerable<T> entityListToDelete) where T : BaseModel;
        Task<bool> DeleteBatchAsync<T>(IEnumerable<T> entityListToDelete) where T : BaseModel;
        int DeleteDynamicData(dynamic dataObject);
        bool DeleteDynamicDataBatch(IEnumerable<dynamic> dataObjects);
        int DeleteExec(string tableName, string where = "", DynamicParameters parameters = null);
        Task<int> DeleteExecAsync(string tableName, string where = "", DynamicParameters parameters = null);
        void Dispose();
        int Execute(string sqlOri, DynamicParameters parameters = null);
        Task<int> ExecuteAsync(string sqlOri, DynamicParameters parameters = null);
        object ExecuteScalar(string sqlOri, DynamicParameters parameters = null);
        T ExecuteScalar<T>(string sqlOri, DynamicParameters parameters = null);
        Task<object> ExecuteScalarAsync(string sqlOri, DynamicParameters parameters = null);
        Task<T> ExecuteScalarAsync<T>(string sqlOri, DynamicParameters parameters = null);
        dynamic Get(string tableName, int id, bool withMC = false);
        dynamic Get(string tableName, string fid, bool withMC = false);
        T Get<T>(int id, bool withMC = false) where T : BaseModel;
        T Get<T>(string fid, bool withMC = false) where T : BaseModel;
        Task<dynamic> GetAsync(string tableName, int id, bool withMC = false);
        Task<dynamic> GetAsync(string tableName, string fid, bool withMC = false);
        Task<T> GetAsync<T>(int id, bool withMC = false) where T : BaseModel;
        Task<T> GetAsync<T>(string fid, bool withMC = false) where T : BaseModel;
        long Insert<T>(T entityToInsert) where T : BaseModel;
        Task<long> InsertAsync<T>(T entityToInsert) where T : BaseModel;
        long InsertBatch<T>(IEnumerable<T> entityListToInsert) where T : BaseModel;
        Task<long> InsertBatchAsync<T>(IEnumerable<T> entityListToInsert) where T : BaseModel;
        long InsertDynamicData(dynamic fapDynData);
        int InsertDynamicDataBatch(IEnumerable<FapDynamicObject> dataObjects);
        IEnumerable<dynamic> Query(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        IEnumerable<T> Query<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        IEnumerable<T> QueryAll<T>(bool withMC = false) where T : BaseModel;
        Task<IEnumerable<T>> QueryAllAsync<T>(bool withMC = false) where T : BaseModel;
        Task<IEnumerable<dynamic>> QueryAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        Task<IEnumerable<T>> QueryAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        dynamic QueryFirst(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        T QueryFirst<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        Task<dynamic> QueryFirstAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        Task<T> QueryFirstAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        dynamic QueryFirstOrDefault(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        T QueryFirstOrDefault<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        Task<dynamic> QueryFirstOrDefaultAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        Task<T> QueryFirstOrDefaultAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        dynamic QueryFirstOrDefaultWhere(string tableName, string where, DynamicParameters parameters = null, bool withMC = false);
        T QueryFirstOrDefaultWhere<T>(string where, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        Task<dynamic> QueryFirstOrDefaultWhereAsync(string tableName, string where, DynamicParameters parameters = null, bool withMC = false);
        Task<T> QueryFirstOrDefaultWhereAsync<T>(string where, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        dynamic QuerySingle(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        T QuerySingle<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        Task<dynamic> QuerySingleAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        Task<T> QuerySingleAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        dynamic QuerySingleOrDefault(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        T QuerySingleOrDefault<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        Task<dynamic> QuerySingleOrDefaultAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        Task<T> QuerySingleOrDefaultAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        IEnumerable<dynamic> QueryWhere(string tableName, string where, DynamicParameters parameters = null, bool withMC = false);
        IEnumerable<T> QueryWhere<T>(string where, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        Task<IEnumerable<dynamic>> QueryWhereAsync(string tableName, string where, DynamicParameters parameters = null, bool withMC = false);
        Task<IEnumerable<T>> QueryWhereAsync<T>(string where, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        void Rollback();
        int Sum(string tableName, string colName, string where = "", DynamicParameters parameters = null);
        int Sum<T>(string colName, string where = "", DynamicParameters parameters = null);
        Task<int> SumAsync(string tableName, string colName, string where = "", DynamicParameters parameters = null);
        Task<int> SumAsync<T>(string colName, string where = "", DynamicParameters parameters = null);
        T Update<T>(T entityToUpdate) where T : BaseModel;
        Task<T> UpdateAsync<T>(T entityToUpdate) where T : BaseModel;
        bool UpdateBatch<T>(IEnumerable<T> entityListToUpdate) where T : BaseModel;
        Task<bool> UpdateBatchAsync<T>(IEnumerable<T> entityListToUpdate) where T : BaseModel;
        bool UpdateDynamicData(dynamic fapDynData);
        int UpdateDynamicDataBatch(IEnumerable<dynamic> dataObjects);
    }
}