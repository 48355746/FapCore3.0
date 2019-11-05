using Dapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.DataAccess
{
    public class DbContext
    {
        public DbContext(ILoggerFactory loggerFactory,IDbSession dbSession)
        {
            #region private
            private TResult DbProxy<TResult>(Func<DbSession, string, TResult> func, DbSession dbSession, string sqlOri, bool withMC = false, bool withId = false)
            {
                TResult result = default(TResult);
                string sql = ParseFapSql(sqlOri, withMC, withId);
                if (func != null)
                {
                    if (dbSession == null)
                    {
                        using (dbSession = _sessionFactory.CreateSession())
                        {
                            result = func(dbSession, sql);
                        }
                    }
                    else
                    {
                        result = func(dbSession, sql);
                    }
                }
                return result;
            }
            private async Task<TResult> DbProxyAsync<TResult>(Func<DbSession, string, Task<TResult>> func, DbSession dbSession, string sqlOri, bool withMC = false, bool withId = false)
            {
                string sql = ParseFapSql(sqlOri, withMC, withId);
                if (func != null)
                {
                    if (dbSession == null)
                    {
                        using (dbSession = _sessionFactory.CreateSession())
                        {
                            return await func(dbSession, sql);
                        }
                    }
                    else
                    {
                        return await func(dbSession, sql);
                    }
                }
                return default(TResult);
            }

            #endregion
            #region 基础操作
            public int Execute(string sqlOri, DynamicParameters parameters = null, DbSession dbSession = null)
                => DbProxy<int>((session, sql) => session.Execute(sql, parameters), dbSession, sqlOri);
            public Task<int> ExecuteAsync(string sqlOri, DynamicParameters parameters = null, DbSession dbSession = null)
                => DbProxyAsync<int>((session, sql) => session.ExecuteAsync(sql, parameters), dbSession, sqlOri);
            public object ExecuteScalar(string sqlOri, DynamicParameters parameters = null, DbSession dbSession = null)
                => DbProxy<object>((session, sql) => session.ExecuteScalar(sql, parameters), dbSession, sqlOri);
            public Task<object> ExecuteScalarAsync(string sqlOri, DynamicParameters parameters = null, DbSession dbSession = null)
                => DbProxyAsync<object>((session, sql) => session.ExecuteScalarAsync(sql, parameters), dbSession, sqlOri);
            public T ExecuteScalar<T>(string sqlOri, DynamicParameters parameters = null, DbSession dbSession = null)
                => DbProxy<T>((session, sql) => session.ExecuteScalar<T>(sql, parameters), dbSession, sqlOri);
            public Task<T> ExecuteScalarAsync<T>(string sqlOri, DynamicParameters parameters = null, DbSession dbSession = null)
                => DbProxyAsync<T>((session, sql) => session.ExecuteScalarAsync<T>(sql, parameters), dbSession, sqlOri);
            public IEnumerable<dynamic> Query(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null)
                => DbProxy((session, sql) => session.Query(sql, parameters), dbSession, sqlOri, withMC);
            public Task<IEnumerable<dynamic>> QueryAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null)
                => DbProxyAsync((session, sql) => session.QueryAsync(sql, parameters), dbSession, sqlOri, withMC);

            public IEnumerable<T> Query<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null) where T : class
                => DbProxy((session, sql) => session.Query<T>(sql, parameters), dbSession, sqlOri, withMC);
            public Task<IEnumerable<T>> QueryAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null) where T : class
                => DbProxyAsync((session, sql) => session.QueryAsync<T>(sql, parameters), dbSession, sqlOri, withMC);
            public IEnumerable<T> QueryAll<T>(bool withMC = false, DbSession dbSession = null) where T : class
            {
                string tableName = typeof(T).Name;
                string cacheKey = $"cache_{tableName}".ToUpper();
                string sql = $"select * from {tableName}";
                IEnumerable<T> result = _cacheService.Get<IEnumerable<T>>(cacheKey);
                if (result == null)
                {
                    result = Query<T>(sql, null, withMC, dbSession);
                    _cacheService.Add(cacheKey, result);
                }
                return result;
            }
            public async Task<IEnumerable<T>> QueryAllAsync<T>(bool withMC = false, DbSession dbSession = null) where T : class
            {
                string tableName = typeof(T).Name;
                string cacheKey = $"cache_{tableName}".ToUpper();
                string sql = $"select * from {tableName}";
                IEnumerable<T> result = _cacheService.Get<IEnumerable<T>>(cacheKey);
                if (result == null)
                {
                    result = await QueryAsync<T>(sql, null, withMC, dbSession);
                    _cacheService.Add(cacheKey, result);
                }
                return result;
            }

            public IEnumerable<dynamic> QueryAll(string tableName, bool withMC = false, DbSession dbSession = null)
            {
                string sql = $"select * from {tableName}";
                string cacheKey = $"cache_{tableName}".ToUpper();
                IEnumerable<dynamic> result = _cacheService.Get<IEnumerable<dynamic>>(cacheKey);
                if (result == null)
                {
                    result = Query(sql, null, withMC, dbSession);
                    _cacheService.Add(cacheKey, result);
                }
                return result;
            }
            public async Task<IEnumerable<dynamic>> QueryAllAsync(string tableName, bool withMC = false, DbSession dbSession = null)
            {
                string sql = $"select * from {tableName}";
                string cacheKey = $"cache_{tableName}".ToUpper();
                IEnumerable<dynamic> result = _cacheService.Get<IEnumerable<dynamic>>(cacheKey);
                if (result == null)
                {
                    result = await QueryAsync(sql, null, withMC, dbSession);
                    _cacheService.Add(cacheKey, result);
                }
                return result;
            }
            public dynamic QueryFirst(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null)
                => DbProxy<dynamic>((session, sql) => session.QueryFirst(sql, parameters), dbSession, sqlOri, withMC);
            public Task<dynamic> QueryFirstAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null)
                => DbProxyAsync<dynamic>((session, sql) => session.QueryFirstAsync(sql, parameters), dbSession, sqlOri, withMC);

            public dynamic QueryFirstOrDefault(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null)
                => DbProxy<dynamic>((session, sql) => session.QueryFirstOrDefault(sql, parameters), dbSession, sqlOri, withMC);
            public Task<dynamic> QueryFirstOrDefaultAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null)
                => DbProxyAsync<dynamic>((session, sql) => session.QueryFirstOrDefaultAsync(sql, parameters), dbSession, sqlOri, withMC);

            public dynamic QuerySingle(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null)
                => DbProxy<dynamic>((session, sql) => session.QuerySingle(sql, parameters), dbSession, sqlOri, withMC);
            public Task<dynamic> QuerySingleAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null)
                => DbProxyAsync<dynamic>((session, sql) => session.QuerySingleAsync(sql, parameters), dbSession, sqlOri, withMC);
            public dynamic QuerySingleOrDefault(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null)
                => DbProxy<dynamic>((session, sql) => session.QuerySingleOrDefault(sql, parameters), dbSession, sqlOri, withMC);
            public Task<dynamic> QuerySingleOrDefaultAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null)
                => DbProxyAsync<dynamic>((session, sql) => session.QuerySingleOrDefaultAsync(sql, parameters), dbSession, sqlOri, withMC);
            public T QueryFirst<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null) where T : class
                => DbProxy<T>((session, sql) => session.QueryFirst<T>(sql, parameters), dbSession, sqlOri, withMC);
            public Task<T> QueryFirstAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null) where T : class
                => DbProxyAsync<T>((session, sql) => session.QueryFirstAsync<T>(sql, parameters), dbSession, sqlOri, withMC);
            public T QueryFirstOrDefault<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null) where T : class
                => DbProxy<T>((session, sql) => session.QueryFirstOrDefault<T>(sql, parameters), dbSession, sqlOri, withMC);
            public Task<T> QueryFirstOrDefaultAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null) where T : class
                => DbProxyAsync<T>((session, sql) => session.QueryFirstOrDefaultAsync<T>(sql, parameters), dbSession, sqlOri, withMC);
            public T QuerySingle<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null) where T : class
                => DbProxy<T>((session, sql) => session.QuerySingle<T>(sql, parameters), dbSession, sqlOri, withMC);
            public Task<T> QuerySingleAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null) where T : class
                => DbProxyAsync<T>((session, sql) => session.QuerySingleAsync<T>(sql, parameters), dbSession, sqlOri, withMC);
            public T QuerySingleOrDefault<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null) where T : class
                => DbProxy<T>((session, sql) => session.QuerySingleOrDefault<T>(sql, parameters), dbSession, sqlOri, withMC);
            public Task<T> QuerySingleOrDefaultAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null) where T : class
                => DbProxyAsync<T>((session, sql) => session.QuerySingleOrDefaultAsync<T>(sql, parameters), dbSession, sqlOri, withMC);
            public IEnumerable<T> QueryWhere<T>(string where, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null) where T : class
            {
                string sql = $"select * from {typeof(T).Name}";
                if (where.IsNotNullOrEmpty())
                {
                    sql += $" where {where}";
                }
                return Query<T>(sql, parameters, withMC, dbSession);
            }
            public Task<IEnumerable<T>> QueryWhereAsync<T>(string where, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null) where T : class
            {
                string sql = $"select * from {typeof(T).Name}";
                if (where.IsNotNullOrEmpty())
                {
                    sql += $" where {where}";
                }
                return QueryAsync<T>(sql, parameters, withMC, dbSession);
            }
            public IEnumerable<dynamic> QueryWhere(string tableName, string where, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null)
            {
                string sql = $"select * from {tableName}";
                if (where.IsNotNullOrEmpty())
                {
                    sql += $" where {where}";
                }
                return Query(sql, parameters, withMC, dbSession);
            }
            public Task<IEnumerable<dynamic>> QueryWhereAsync(string tableName, string where, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null)
            {
                string sql = $"select * from {tableName}";
                if (where.IsNotNullOrEmpty())
                {
                    sql += $" where {where}";
                }
                return QueryAsync(sql, parameters, withMC, dbSession);
            }
            public dynamic QueryFirstOrDefaultWhere(string tableName, string where, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null)
            {
                string sql = $"select * from {tableName}";
                if (where.IsNotNullOrEmpty())
                {
                    sql += $" where {where}";
                }
                return QueryFirstOrDefault(sql, parameters, withMC, dbSession);
            }
            public Task<dynamic> QueryFirstOrDefaultWhereAsync(string tableName, string where, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null)
            {
                string sql = $"select * from {tableName}";
                if (where.IsNotNullOrEmpty())
                {
                    sql += $" where {where}";
                }
                return QueryFirstOrDefaultAsync(sql, parameters, withMC, dbSession);
            }
            public T QueryFirstOrDefaultWhere<T>(string where, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null) where T : class
            {
                string sql = $"select * from {typeof(T).Name}";
                if (where.IsNotNullOrEmpty())
                {
                    sql += $" where {where}";
                }
                return QueryFirstOrDefault<T>(sql, parameters, withMC, dbSession);
            }
            public Task<T> QueryFirstOrDefaultWhereAsync<T>(string where, DynamicParameters parameters = null, bool withMC = false, DbSession dbSession = null) where T : class
            {
                string sql = $"select * from {typeof(T).Name}";
                if (where.IsNotNullOrEmpty())
                {
                    sql += $" where {where}";
                }
                return QueryFirstOrDefaultAsync<T>(sql, parameters, withMC, dbSession);
            }
            #endregion


            #region Get
            public dynamic Get(string tableName, int id, bool withMC = false, DbSession dbSession = null)
            {
                string sqlOri = $"select * from {tableName} where id={id}";
                return DbProxy<dynamic>((session, sql) => session.QueryFirstOrDefault(sql), dbSession, sqlOri, withMC);
            }
            public Task<dynamic> GetAsync(string tableName, int id, bool withMC = false, DbSession dbSession = null)
            {
                string sqlOri = $"select * from {tableName} where id={id}";
                return DbProxyAsync<dynamic>((session, sql) => session.QueryFirstOrDefaultAsync(sql), dbSession, sqlOri, withMC);
            }
            public dynamic Get(string tableName, string fid, bool withMC = false, DbSession dbSession = null)
            {
                string sqlOri = $"select * from {tableName} where fid='{fid}'";
                return DbProxy<dynamic>((session, sql) => session.QueryFirstOrDefault(sql), dbSession, sqlOri, withMC);
            }
            public Task<dynamic> GetAsync(string tableName, string fid, bool withMC = false, DbSession dbSession = null)
            {
                string sqlOri = $"select * from {tableName} where fid='{fid}'";
                return DbProxyAsync<dynamic>((session, sql) => session.QueryFirstOrDefaultAsync(sql), dbSession, sqlOri, withMC);
            }
            public T Get<T>(int id, bool withMC = false, DbSession dbSession = null) where T : class
            {
                string tableName = typeof(T).Name;
                string sqlOri = $"select * from {tableName} where Id=@Id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id);
                return DbProxy<T>((session, sql) => session.QueryFirstOrDefault<T>(sql, parameters), dbSession, sqlOri, withMC);
            }
            public Task<T> GetAsync<T>(int id, bool withMC = false, DbSession dbSession = null) where T : class
            {
                string tableName = typeof(T).Name;
                string sqlOri = $"select * from {tableName} where Id=@Id";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Id", id);
                return DbProxyAsync<T>((session, sql) => session.QueryFirstOrDefaultAsync<T>(sql, parameters), dbSession, sqlOri, withMC);
            }
            public T Get<T>(string fid, bool withMC = false, DbSession dbSession = null) where T : class
            {
                string tableName = typeof(T).Name;
                string sqlOri = $"select * from {tableName} where Fid=@Fid";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Fid", fid);
                return DbProxy<T>((session, sql) => session.QueryFirstOrDefault<T>(sql, parameters), dbSession, sqlOri, withMC);
            }
            public Task<T> GetAsync<T>(string fid, bool withMC = false, DbSession dbSession = null) where T : class
            {
                string tableName = typeof(T).Name;
                string sqlOri = $"select * from {tableName} where Fid=@Fid";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("Fid", fid);
                return DbProxyAsync<T>((session, sql) => session.QueryFirstOrDefaultAsync<T>(sql, parameters), dbSession, sqlOri, withMC);
            }
            public int Count(string tableName, string where = "", DynamicParameters parameters = null, DbSession dbSession = null)
            {
                string sqlOri = $"select count(1) from {tableName}";
                if (where.IsNotNullOrEmpty())
                {
                    sqlOri += $" where {where}";
                }
                return DbProxy<int>((session, sql) => session.ExecuteScalar<int>(sql, parameters), dbSession, sqlOri);
            }
            public Task<int> CountAsync(string tableName, string where = "", DynamicParameters parameters = null, DbSession dbSession = null)
            {
                string sqlOri = $"select count(1) from {tableName}";
                if (where.IsNotNullOrEmpty())
                {
                    sqlOri += $" where {where}";
                }
                return DbProxyAsync<int>((session, sql) => session.ExecuteScalarAsync<int>(sql, parameters), dbSession, sqlOri);
            }
            public int Count<T>(string where = "", DynamicParameters parameters = null, DbSession dbSession = null) where T : class
            {
                string tableName = typeof(T).Name;
                string sqlOri = $"select count(1) from {tableName}";
                if (where.IsNotNullOrEmpty())
                {
                    sqlOri += $" where {where}";
                }
                return DbProxy<int>((session, sql) => session.ExecuteScalar<int>(sql, parameters), dbSession, sqlOri);
            }
            public Task<int> CountAsync<T>(string where = "", DynamicParameters parameters = null, DbSession dbSession = null) where T : class
            {
                string tableName = typeof(T).Name;
                string sqlOri = $"select count(1) from {tableName}";
                if (where.IsNotNullOrEmpty())
                {
                    sqlOri += $" where {where}";
                }
                return DbProxyAsync<int>((session, sql) => session.ExecuteScalarAsync<int>(sql, parameters), dbSession, sqlOri);
            }
            public int Sum(string tableName, string colName, string where = "", DynamicParameters parameters = null, DbSession dbSession = null)
            {
                string sqlOri = $"select sum({colName}) from {tableName}";
                if (where.IsNotNullOrEmpty())
                {
                    sqlOri += $" where {where}";
                }
                return DbProxy<int>((session, sql) => session.ExecuteScalar<int>(sql, parameters), dbSession, sqlOri);
            }
            public Task<int> SumAsync(string tableName, string colName, string where = "", DynamicParameters parameters = null, DbSession dbSession = null)
            {
                string sqlOri = $"select sum({colName}) from {tableName}";
                if (where.IsNotNullOrEmpty())
                {
                    sqlOri += $" where {where}";
                }
                return DbProxyAsync<int>((session, sql) => session.ExecuteScalarAsync<int>(sql, parameters), dbSession, sqlOri);
            }
            public int Sum<T>(string colName, string where = "", DynamicParameters parameters = null, DbSession dbSession = null) where T : class
            {
                string tableName = typeof(T).Name;
                string sqlOri = $"select sum({colName}) from {tableName}";
                if (where.IsNotNullOrEmpty())
                {
                    sqlOri += $" where {where}";
                }
                return DbProxy<int>((session, sql) => session.ExecuteScalar<int>(sql, parameters), dbSession, sqlOri);
            }
            public Task<int> SumAsync<T>(string colName, string where = "", DynamicParameters parameters = null, DbSession dbSession = null) where T : class
            {
                string tableName = typeof(T).Name;
                string sqlOri = $"select sum({colName}) from {tableName}";
                if (where.IsNotNullOrEmpty())
                {
                    sqlOri += $" where {where}";
                }
                return DbProxyAsync<int>((session, sql) => session.ExecuteScalarAsync<int>(sql, parameters), dbSession, sqlOri);
            }
            #endregion

        }
    }
}
