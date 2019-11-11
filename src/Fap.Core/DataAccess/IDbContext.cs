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
        IEnumerable<dynamic> Query(string sqlOri, bool withMC = false, DynamicParameters parameters = null);
        IEnumerable<T> Query<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel;
        IEnumerable<T> QueryAll<T>(bool withMC = false) where T : BaseModel;
        Task<IEnumerable<T>> QueryAllAsync<T>(bool withMC = false) where T : BaseModel;
        Task<IEnumerable<dynamic>> QueryAsync(string sqlOri, bool withMC = false, DynamicParameters parameters = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel;
        dynamic QueryFirst(string sqlOri, bool withMC = false, DynamicParameters parameters = null);
        T QueryFirst<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel;
        Task<dynamic> QueryFirstAsync(string sqlOri, bool withMC = false, DynamicParameters parameters = null);
        Task<T> QueryFirstAsync<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel;
        dynamic QueryFirstOrDefault(string sqlOri, bool withMC = false, DynamicParameters parameters = null);
        T QueryFirstOrDefault<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel;
        Task<dynamic> QueryFirstOrDefaultAsync(string sqlOri, bool withMC = false, DynamicParameters parameters = null);
        Task<T> QueryFirstOrDefaultAsync<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel;
        dynamic QueryFirstOrDefaultWhere(string tableName, string where, bool withMC = false, DynamicParameters parameters = null);
        T QueryFirstOrDefaultWhere<T>(string where, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel;
        Task<dynamic> QueryFirstOrDefaultWhereAsync(string tableName, string where, bool withMC = false, DynamicParameters parameters = null);
        Task<T> QueryFirstOrDefaultWhereAsync<T>(string where, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel;
        dynamic QuerySingle(string sqlOri, bool withMC = false, DynamicParameters parameters = null);
        T QuerySingle<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel;
        Task<dynamic> QuerySingleAsync(string sqlOri, bool withMC = false, DynamicParameters parameters = null);
        Task<T> QuerySingleAsync<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel;
        dynamic QuerySingleOrDefault(string sqlOri, bool withMC = false, DynamicParameters parameters = null);
        T QuerySingleOrDefault<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel;
        Task<dynamic> QuerySingleOrDefaultAsync(string sqlOri, bool withMC = false, DynamicParameters parameters = null);
        Task<T> QuerySingleOrDefaultAsync<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel;
        IEnumerable<dynamic> QueryWhere(string tableName, string where, bool withMC = false, DynamicParameters parameters = null);
        IEnumerable<T> QueryWhere<T>(string where, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel;
        Task<IEnumerable<dynamic>> QueryWhereAsync(string tableName, string where, bool withMC = false, DynamicParameters parameters = null);
        Task<IEnumerable<T>> QueryWhereAsync<T>(string where, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel;
        void Rollback();
        int Sum(string tableName, string colName, string where = "", DynamicParameters parameters = null);
        int Sum<T>(string colName, string where = "", DynamicParameters parameters = null);
        Task<int> SumAsync(string tableName, string colName, string where = "", DynamicParameters parameters = null);
        Task<int> SumAsync<T>(string colName, string where = "", DynamicParameters parameters = null);
    }
}