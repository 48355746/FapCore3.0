using Dapper;
using Fap.Core.DataAccess.SqlParser;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.MetaData;
using Fap.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.DataAccess
{
    public class DbContext
    {
        private readonly ILogger<DbContext> _logger;
        private readonly IDbSession _dbSession;
        private readonly IFapPlatformDomain _fapPlatformDomain;
        public DbContext(ILoggerFactory loggerFactory, IFapPlatformDomain fapPlatformDomain, IDbSession dbSession)
        {
            _logger = loggerFactory.CreateLogger<DbContext>();
            _dbSession = dbSession;
            _fapPlatformDomain = fapPlatformDomain;
        }
        /// <summary>
        /// 历史时间点
        /// </summary>
        public string HistoryDateTime { get; set; }
        #region private
        private string WrapSqlAndParam(string sqlOri, DynamicParameters parameters, bool withMC = false, bool withId = false)
        {
            InitParamers(parameters);
            return ParseFapSql(sqlOri, withMC, withId);
            DynamicParameters InitParamers(DynamicParameters parameters)
            {
                if (parameters == null)
                {
                    parameters = new DynamicParameters();
                }
                if (!parameters.ParameterNames.Contains(FapDbConstants.FAPCOLUMN_FIELD_CurrentDate))
                {
                    if (HistoryDateTime.IsMissing())
                    {
                        parameters.Add(FapDbConstants.FAPCOLUMN_FIELD_CurrentDate, DateTimeUtils.CurrentDateTimeStr);
                    }
                    else
                    {
                        parameters.Add(FapDbConstants.FAPCOLUMN_FIELD_CurrentDate, HistoryDateTime);
                    }
                }
                if (!parameters.ParameterNames.Contains(FapDbConstants.FAPCOLUMN_FIELD_Dr))
                {
                    parameters.Add(FapDbConstants.FAPCOLUMN_FIELD_Dr, 0);
                }

                //日志
                foreach (var paramName in parameters.ParameterNames)
                {
                    _logger.LogInformation($"{parameters}={parameters.Get<object>(paramName)}");
                }

                return parameters;
            }
            string ParseFapSql(string sqlOri, bool withMC = false, bool withId = false)
            {
                _logger.LogInformation($"wrap前的sql:{sqlOri}");
                FapSqlParser parse = new FapSqlParser(_fapPlatformDomain, sqlOri, withMC, withId);
                var sql = parse.GetCompletedSql();
                _logger.LogInformation($"wrap后的sql:{sql}");
                return sql;
            }
        }

        #endregion
        #region 基础操作
        public int Execute(string sqlOri, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.Execute(sql, parameters);
        }
        public Task<int> ExecuteAsync(string sqlOri, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteAsync(sql, parameters);
        }
        public object ExecuteScalar(string sqlOri, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteScalar(sql, parameters);
        }
        public Task<object> ExecuteScalarAsync(string sqlOri, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteScalarAsync(sql, parameters);
        }
        public T ExecuteScalar<T>(string sqlOri, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteScalar<T>(sql, parameters);
        }
        public Task<T> ExecuteScalarAsync<T>(string sqlOri, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteScalarAsync<T>(sql, parameters);
        }
        public IEnumerable<dynamic> Query(string sqlOri, bool withMC = false, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.Query(sql, parameters);
        }
        public Task<IEnumerable<dynamic>> QueryAsync(string sqlOri, bool withMC = false, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryAsync(sql, parameters);
        }

        public IEnumerable<T> Query<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.Query<T>(sql, parameters);
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryAsync<T>(sql, parameters);
        }
        public IEnumerable<T> QueryAll<T>(bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sql = $"select * from {tableName}";
            return Query<T>(sql);
        }
        public Task<IEnumerable<T>> QueryAllAsync<T>(bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sql = $"select * from {tableName}";
            return QueryAsync<T>(sql);
        }
        public dynamic QueryFirst(string sqlOri, bool withMC = false, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirst(sql, parameters);
        }
        public Task<dynamic> QueryFirstAsync(string sqlOri, bool withMC = false, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstAsync(sql, parameters);
        }

        public dynamic QueryFirstOrDefault(string sqlOri, bool withMC = false, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstOrDefault(sql, parameters);
        }
        public Task<dynamic> QueryFirstOrDefaultAsync(string sqlOri, bool withMC = false, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstOrDefaultAsync(sql, parameters);
        }

        public dynamic QuerySingle(string sqlOri, bool withMC = false, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingle(sql, parameters);
        }
        public Task<dynamic> QuerySingleAsync(string sqlOri, bool withMC = false, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleAsync(sql, parameters);
        }
        public dynamic QuerySingleOrDefault(string sqlOri, bool withMC = false, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleOrDefault(sql, parameters);
        }
        public Task<dynamic> QuerySingleOrDefaultAsync(string sqlOri, bool withMC = false, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleOrDefaultAsync(sql, parameters);
        }
        public T QueryFirst<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirst<T>(sql, parameters);
        }
        public Task<T> QueryFirstAsync<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstAsync<T>(sql, parameters);
        }
        public T QueryFirstOrDefault<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstOrDefault<T>(sql, parameters);
        }
        public Task<T> QueryFirstOrDefaultAsync<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstOrDefaultAsync<T>(sql, parameters);
        }
        public T QuerySingle<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingle<T>(sql, parameters);

        }
        public Task<T> QuerySingleAsync<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleAsync<T>(sql, parameters);
        }
        public T QuerySingleOrDefault<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleOrDefault<T>(sql, parameters);
        }
        public Task<T> QuerySingleOrDefaultAsync<T>(string sqlOri, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleOrDefaultAsync<T>(sql, parameters);
        }
        public IEnumerable<T> QueryWhere<T>(string where, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel
        {
            string sql = $"select * from {typeof(T).Name}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return Query<T>(sql, withMC, parameters);
        }
        public Task<IEnumerable<T>> QueryWhereAsync<T>(string where, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel
        {
            string sql = $"select * from {typeof(T).Name}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryAsync<T>(sql, withMC, parameters);
        }
        public IEnumerable<dynamic> QueryWhere(string tableName, string where, bool withMC = false, DynamicParameters parameters = null)
        {
            string sql = $"select * from {tableName}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return Query(sql, withMC, parameters);
        }
        public Task<IEnumerable<dynamic>> QueryWhereAsync(string tableName, string where, bool withMC = false, DynamicParameters parameters = null)
        {
            string sql = $"select * from {tableName}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryAsync(sql, withMC, parameters);
        }
        public dynamic QueryFirstOrDefaultWhere(string tableName, string where, bool withMC = false, DynamicParameters parameters = null)
        {
            string sql = $"select * from {tableName}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryFirstOrDefault(sql, withMC, parameters);
        }
        public Task<dynamic> QueryFirstOrDefaultWhereAsync(string tableName, string where, bool withMC = false, DynamicParameters parameters = null)
        {
            string sql = $"select * from {tableName}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryFirstOrDefaultAsync(sql, withMC, parameters);
        }
        public T QueryFirstOrDefaultWhere<T>(string where, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel
        {
            string sql = $"select * from {typeof(T).Name}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryFirstOrDefault<T>(sql, withMC, parameters);
        }
        public Task<T> QueryFirstOrDefaultWhereAsync<T>(string where, bool withMC = false, DynamicParameters parameters = null) where T : BaseModel
        {
            string sql = $"select * from {typeof(T).Name}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryFirstOrDefaultAsync<T>(sql, withMC, parameters);
        }
        #endregion


        #region Get
        public dynamic Get(string tableName, int id, bool withMC = false)
        {
            string sqlOri = $"select * from {tableName} where id={id}";
            return QueryFirstOrDefault(sqlOri, withMC);
        }
        public Task<dynamic> GetAsync(string tableName, int id, bool withMC = false)
        {
            string sqlOri = $"select * from {tableName} where id={id}";
            return QueryFirstOrDefaultAsync(sqlOri, withMC);
        }
        public dynamic Get(string tableName, string fid, bool withMC = false)
        {
            string sqlOri = $"select * from {tableName} where fid='{fid}'";
            return QueryFirstOrDefault(sqlOri, withMC);
        }
        public Task<dynamic> GetAsync(string tableName, string fid, bool withMC = false)
        {
            string sqlOri = $"select * from {tableName} where fid='{fid}'";
            return QueryFirstOrDefaultAsync(sqlOri, withMC);
        }
        public T Get<T>(int id, bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select * from {tableName} where Id=@Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", id);
            return QueryFirstOrDefault<T>(sqlOri, withMC, parameters);
        }
        public Task<T> GetAsync<T>(int id, bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select * from {tableName} where Id=@Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", id);
            return QueryFirstOrDefaultAsync<T>(sqlOri, withMC, parameters);
        }
        public T Get<T>(string fid, bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select * from {tableName} where Fid=@Fid";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Fid", fid);
            return QueryFirstOrDefault<T>(sqlOri, withMC, parameters);
        }
        public Task<T> GetAsync<T>(string fid, bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select * from {tableName} where Fid=@Fid";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Fid", fid);
            return QueryFirstOrDefaultAsync<T>(sqlOri, withMC, parameters);
        }
        public int Count(string tableName, string where = "", DynamicParameters parameters = null)
        {
            string sqlOri = $"select count(1) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalar<int>(sqlOri, parameters);
        }
        public Task<int> CountAsync(string tableName, string where = "", DynamicParameters parameters = null)
        {
            string sqlOri = $"select count(1) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalarAsync<int>(sqlOri, parameters);
        }
        public int Count<T>(string where = "", DynamicParameters parameters = null)
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select count(1) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalar<int>(sqlOri, parameters);
        }
        public Task<int> CountAsync<T>(string where = "", DynamicParameters parameters = null)
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select count(1) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalarAsync<int>(sqlOri, parameters);
        }
        public int Sum(string tableName, string colName, string where = "", DynamicParameters parameters = null)
        {
            string sqlOri = $"select sum({colName}) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalar<int>(sqlOri, parameters);
        }
        public Task<int> SumAsync(string tableName, string colName, string where = "", DynamicParameters parameters = null)
        {
            string sqlOri = $"select sum({colName}) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalarAsync<int>(sqlOri, parameters);
        }
        public int Sum<T>(string colName, string where = "", DynamicParameters parameters = null)
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select sum({colName}) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalar<int>(sqlOri, parameters);
        }
        public Task<int> SumAsync<T>(string colName, string where = "", DynamicParameters parameters = null)
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select sum({colName}) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalarAsync<int>(sqlOri, parameters);
        }
        #endregion

    }
}

