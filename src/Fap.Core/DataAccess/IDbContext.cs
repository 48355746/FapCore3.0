using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Infrastructure.Query;
using Fap.Core.Infrastructure.Metadata;

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
        bool Delete<T>(long id) where T : BaseModel;
        bool Delete<T>(string fid) where T : BaseModel;
        bool Delete<T>(T entityToDelete) where T : BaseModel;
        Task<bool> DeleteAsync<T>(long id) where T : BaseModel;
        Task<bool> DeleteAsync<T>(string fid) where T : BaseModel;
        Task<bool> DeleteAsync<T>(T entityToDelete) where T : BaseModel;
        void DeleteBatch<T>(IEnumerable<T> entityListToDelete) where T : BaseModel;
        Task DeleteBatchAsync<T>(IEnumerable<T> entityListToDelete) where T : BaseModel;
        bool DeleteDynamicData(FapDynamicObject dataObject);
        void DeleteDynamicDataBatch(IEnumerable<FapDynamicObject> dataObjects);
        int DeleteExec(string tableName, string where = "", DynamicParameters parameters = null);
        Task<int> DeleteExecAsync(string tableName, string where = "", DynamicParameters parameters = null);
        int Execute(string sqlOri, DynamicParameters parameters = null);
        Task<int> ExecuteAsync(string sqlOri, DynamicParameters parameters = null);
        object ExecuteScalar(string sqlOri, DynamicParameters parameters = null);
        T ExecuteScalar<T>(string sqlOri, DynamicParameters parameters = null);
        Task<object> ExecuteScalarAsync(string sqlOri, DynamicParameters parameters = null);
        Task<T> ExecuteScalarAsync<T>(string sqlOri, DynamicParameters parameters = null);
        dynamic Get(string tableName, long id, bool withMC = false);
        dynamic Get(string tableName, string fid, bool withMC = false);
        T Get<T>(long id, bool withMC = false) where T : BaseModel;
        T Get<T>(string fid, bool withMC = false) where T : BaseModel;
        Task<dynamic> GetAsync(string tableName, long id, bool withMC = false);
        Task<dynamic> GetAsync(string tableName, string fid, bool withMC = false);
        Task<T> GetAsync<T>(long id, bool withMC = false) where T : BaseModel;
        Task<T> GetAsync<T>(string fid, bool withMC = false) where T : BaseModel;
        long Insert<T>(T entityToInsert) where T : BaseModel;
        Task<long> InsertAsync<T>(T entityToInsert) where T : BaseModel;
        List<long> InsertBatch<T>(IEnumerable<T> entityListToInsert) where T : BaseModel;
        Task<List<long>> InsertBatchAsync<T>(IEnumerable<T> entityListToInsert) where T : BaseModel;
        long InsertDynamicData(FapDynamicObject fapDynData);
        List<long> InsertDynamicDataBatch(IEnumerable<FapDynamicObject> dataObjects);
        /// <summary>
        /// 查询原始sql，不进行包装
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IEnumerable<dynamic> QueryOriSql(string sqlOri, DynamicParameters parameters = null);
        IEnumerable<dynamic> Query(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        IEnumerable<T> Query<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        IEnumerable<dynamic> QueryAll(string tableName, bool withMC = false);
        Task<IEnumerable<dynamic>> QueryAllAsync(string tableName, bool withMC = false);
        IEnumerable<T> QueryAll<T>(bool withMC = false) where T : BaseModel;
        Task<IEnumerable<T>> QueryAllAsync<T>(bool withMC = false) where T : BaseModel;
        Task<IEnumerable<dynamic>> QueryAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        Task<IEnumerable<T>> QueryAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        /// <summary>
        /// 仅当元素个数大于等于1时返回第一个元素，否则抛异常InvalidOperationException: Sequence contains no elements
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        /// <remarks>InvalidOperationException: Sequence contains no elements</remarks>
        dynamic QueryFirst(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        /// <summary>
        /// 仅当元素个数大于等于1时返回第一个元素，否则抛异常InvalidOperationException: Sequence contains no elements
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        /// <remarks>InvalidOperationException: Sequence contains no elements</remarks>
        T QueryFirst<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        /// <summary>
        /// 仅当元素个数大于等于1时返回第一个元素，否则抛异常InvalidOperationException: Sequence contains no elements
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        /// <remarks>InvalidOperationException: Sequence contains no elements</remarks>
        Task<dynamic> QueryFirstAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        /// <summary>
        /// 仅当元素个数大于等于1时返回第一个元素，否则抛异常InvalidOperationException: Sequence contains no elements
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        /// <remarks>InvalidOperationException: Sequence contains no elements</remarks>
        Task<T> QueryFirstAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        dynamic QueryFirstOrDefault(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        T QueryFirstOrDefault<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        Task<dynamic> QueryFirstOrDefaultAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        Task<T> QueryFirstOrDefaultAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        dynamic QueryFirstOrDefaultWhere(string tableName, string where, DynamicParameters parameters = null, bool withMC = false);
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        T QueryFirstOrDefaultWhere<T>(string where, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        Task<dynamic> QueryFirstOrDefaultWhereAsync(string tableName, string where, DynamicParameters parameters = null, bool withMC = false);
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        Task<T> QueryFirstOrDefaultWhereAsync<T>(string where, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        /// <summary>
        /// 仅仅存在单个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException:Sequence contains no elements,InvalidOperationException: Sequence contains more than one element</remarks>
        dynamic QuerySingle(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        /// <summary>
        /// 仅仅存在单个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException:Sequence contains no elements,InvalidOperationException: Sequence contains more than one element</remarks>
        T QuerySingle<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        /// <summary>
        /// 仅仅存在单个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException:Sequence contains no elements,InvalidOperationException: Sequence contains more than one element</remarks>
        Task<dynamic> QuerySingleAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        /// <summary>
        /// 仅仅存在单个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException:Sequence contains no elements,InvalidOperationException: Sequence contains more than one element</remarks>
        Task<T> QuerySingleAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        /// <summary>
        /// 仅仅存在单个或0个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException: Sequence contains more than one element</remarks>
        dynamic QuerySingleOrDefault(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        /// <summary>
        /// 仅仅存在单个或0个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException: Sequence contains more than one element</remarks>
        T QuerySingleOrDefault<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        /// <summary>
        /// 仅仅存在单个或0个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException: Sequence contains more than one element</remarks>
        Task<dynamic> QuerySingleOrDefaultAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false);
        /// <summary>
        /// 仅仅存在单个或0个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException: Sequence contains more than one element</remarks>
        Task<T> QuerySingleOrDefaultAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        IEnumerable<dynamic> QueryWhere(string tableName, string where, DynamicParameters parameters = null, bool withMC = false);
        IEnumerable<T> QueryWhere<T>(string where, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        Task<IEnumerable<dynamic>> QueryWhereAsync(string tableName, string where, DynamicParameters parameters = null, bool withMC = false);
        Task<IEnumerable<T>> QueryWhereAsync<T>(string where, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel;
        void Rollback();
        decimal Sum(string tableName, string colName, string where = "", DynamicParameters parameters = null);
        decimal Sum<T>(string colName, string where = "", DynamicParameters parameters = null);
        Task<decimal> SumAsync(string tableName, string colName, string where = "", DynamicParameters parameters = null);
        Task<decimal> SumAsync<T>(string colName, string where = "", DynamicParameters parameters = null);
        bool Update<T>(T entityToUpdate) where T : BaseModel;
        Task<bool> UpdateAsync<T>(T entityToUpdate) where T : BaseModel;
        bool UpdateBatch<T>(IEnumerable<T> entityListToUpdate) where T : BaseModel;
        Task<bool> UpdateBatchAsync<T>(IEnumerable<T> entityListToUpdate) where T : BaseModel;
        bool UpdateDynamicData(FapDynamicObject fapDynData);
        void UpdateDynamicDataBatch(IEnumerable<FapDynamicObject> dataObjects);

        PageInfo<T> QueryPage<T>(Pageable pageable) where T : BaseModel;
        PageInfo<dynamic> QueryPage(Pageable pageable);
        /// <summary>
        /// 获取一个默认数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        FapDynamicObject GetDefualtData(string tableName);
        IEnumerable<DataChangeHistory> QueryDataHistory(string tableName, string fid);
    }
}