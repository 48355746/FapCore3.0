using Ardalis.GuardClauses;
using Dapper;
using Dapper.Contrib.Extensions;
using Fap.Core.DI;
using Fap.Core.Exceptions;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Fap.Core.DataAccess
{
    public class DbSession : IDbSession
    {
        private ILogger<DbSession> _logger;
        public int? CommandTimeout { get; set; }
        /// <summary>
        /// 数据库连接工厂
        /// </summary>
        public IConnectionFactory ConnectionFactory { get; private set; }

        private static ThreadLocal<IDbConnection> threadLocalConnection = new ThreadLocal<IDbConnection>();
        private static ThreadLocal<IDbTransaction> threadLocalTransaction = new ThreadLocal<IDbTransaction>();
        private IDbConnection CurrentConnection
        {
            set { threadLocalConnection.Value = value; }
            get { return threadLocalConnection.Value; }
        }
        private IDbTransaction CurrentTransaction
        {
            set { threadLocalTransaction.Value = value; }
            get { return threadLocalTransaction.Value; }
        }
        public DatabaseDialectEnum DatabaseDialect { get; private set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="conn">连接</param>
        public DbSession(ILoggerFactory loggerFactory, IConnectionFactory connectionFactory)
        {
            _logger = loggerFactory.CreateLogger<DbSession>();
            ConnectionFactory = connectionFactory;
            DatabaseDialect = connectionFactory.DatabaseDialect;
            // CommandTimeout = commandTimeOut;
        }
        #region private
        Stopwatch timer = new Stopwatch();
        private TResult DbExecProxy<TResult>(Func<DynamicParameters, TResult> func, string sql, DynamicParameters parameters = null)
        {
            Guard.Against.NullOrWhiteSpace(sql, nameof(sql));
            string message = "";
            parameters = parameters ?? new DynamicParameters();
            try
            {
                timer.Restart();//开始计算时间
                var result = func(parameters);
                timer.Stop();//结束点
                message = $"SQL语句为：{sql},{Environment.NewLine}参数:{(parameters != null ? string.Join(",", parameters.ParameterNames.Select((key) => $"{key}={parameters.Get<object>(key)}")) : "")}{Environment.NewLine}执行时间：{ timer.ElapsedMilliseconds}毫秒";
                _logger.LogTrace(message);
                return result;
            }
            catch (Exception ex)
            {
                message = $"查询SQL异常：{ ex.Message },{Environment.NewLine}SQL语句为：{sql}{Environment.NewLine}参数:{(parameters != null ? string.Join(",", parameters.ParameterNames.Select((key) => $"{key}={parameters.Get<object>(key)}")) : "")}";
                _logger.LogError(message);
                Guard.Against.FapRuntime(message, ex);
            }
            return default(TResult);
        }
        private async Task<TResult> DbExecProxyAsync<TResult>(Func<DynamicParameters, Task<TResult>> func, string sql, DynamicParameters parameters = null)
        {
            Guard.Against.NullOrWhiteSpace(sql, nameof(sql));
            string message = "";
            parameters = parameters ?? new DynamicParameters();
            try
            {
                timer.Restart();//开始计算时间
                var tsv = await func(parameters);
                timer.Stop();//结束点
                message = $"SQL语句为：{sql},{Environment.NewLine}参数:{(parameters != null ? parameters.ToString() : "")}";
                message += $"{Environment.NewLine}执行时间：{ timer.ElapsedMilliseconds}毫秒";
                _logger.LogTrace(message);
                return tsv;
            }
            catch (Exception ex)
            {
                message = $"查询SQL异常：{ ex.Message },{Environment.NewLine}SQL语句为：{sql}{Environment.NewLine}参数:{(parameters != null ? parameters.ToString() : "")}";
                _logger.LogError(message);
                Guard.Against.FapRuntime(message, ex);
            }
            return default(TResult);
        }
        private IDbConnection GetDbConnection(DataSourceEnum dataSource)
        {
            ConnectionFactory.DataSource = dataSource;
            return ConnectionFactory.GetDbConnection();
        }

        #endregion
        #region excute
        /// <summary>
        /// 执行选择单个值的参数化SQL。
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public int Execute(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy<int>((param) => CurrentConnection.Execute(sql, param, CurrentTransaction, null, commandType), sql, parameters);
            }
            else
            {
                using var connection = GetDbConnection(DataSourceEnum.MASTER);
                return DbExecProxy<int>((param) => connection.Execute(sql, param, CurrentTransaction, null, commandType), sql, parameters);
            }
        }
        public Task<int> ExecuteAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync<int>((param) => CurrentConnection.ExecuteAsync(sql, param, CurrentTransaction, null, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return DbExecProxyAsync<int>((param) => connection.ExecuteAsync(sql, param, CurrentTransaction, null, commandType), sql, parameters);
        }
        /// <summary>
        /// 执行选择单个值的参数化SQL。
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy<object>((param) => CurrentConnection.ExecuteScalar(sql, param, CurrentTransaction, null, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<object>((param) => connection.ExecuteScalar(sql, param, CurrentTransaction, null, commandType), sql, parameters);
        }
        /// <summary>
        /// 执行选择单个值的参数化SQL。
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Task<object> ExecuteScalarAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync<object>((param) => CurrentConnection.ExecuteScalarAsync(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<object>((param) => connection.ExecuteScalarAsync(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        /// <summary>
        /// 执行选择单个值的参数化SQL。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy<T>((param) => CurrentConnection.ExecuteScalar<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<T>((param) => connection.ExecuteScalar<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        public Task<T> ExecuteScalarAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync<T>((param) => CurrentConnection.ExecuteScalarAsync<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<T>((param) => connection.ExecuteScalarAsync<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        #endregion
        #region query
        public T Get<T>(int id) where T : class
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy<T>((param) => CurrentConnection.Get<T>(id, CurrentTransaction, CommandTimeout), "get by id", null);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<T>((param) => connection.Get<T>(id, CurrentTransaction, CommandTimeout), "get by id", null);
        }
        public IEnumerable<T> GetAll<T>() where T : class
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy<IEnumerable<T>>((param) => CurrentConnection.GetAll<T>(CurrentTransaction, CommandTimeout), "get all", null);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<IEnumerable<T>>((param) => connection.GetAll<T>(CurrentTransaction, CommandTimeout), "get all", null);
        }
        public Task<T> GetAsync<T>(int id) where T : class
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync<T>((param) => CurrentConnection.GetAsync<T>(id, CurrentTransaction, CommandTimeout), "get by id", null);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<T>((param) => connection.GetAsync<T>(id, CurrentTransaction, CommandTimeout), "get by id", null);
        }
        public Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync<IEnumerable<T>>((param) => CurrentConnection.GetAllAsync<T>(CurrentTransaction, CommandTimeout), "get all", null);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<IEnumerable<T>>((param) => connection.GetAllAsync<T>(CurrentTransaction, CommandTimeout), "get all", null);
        }
        /// <summary>
        /// 返回具有与列匹配的属性的动态对象。
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="buffered"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> Query(string sql, DynamicParameters parameters = null, bool buffered = true, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy<IEnumerable<dynamic>>((param) => CurrentConnection.Query(sql, param, CurrentTransaction, buffered, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<IEnumerable<dynamic>>((param) => connection.Query(sql, param, CurrentTransaction, buffered, CommandTimeout, commandType), sql, parameters);
        }
        /// <summary>
        /// 返回具有与列匹配的属性的动态对象。
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public Task<IEnumerable<dynamic>> QueryAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync<IEnumerable<dynamic>>((param) => CurrentConnection.QueryAsync(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<IEnumerable<dynamic>>((param) => connection.QueryAsync(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        /// <summary>
        /// 返回具有与列匹配的属性的动态对象。
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns>dynamic</returns>
        /// <remarks>the row can be accessed via "dynamic", or by casting to an IDictionary<string,object></remarks>
        /// <exception cref="">序列不包含任何元素</exception>
        public dynamic QueryFirst(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy((param) => CurrentConnection.QueryFirst(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy((param) => connection.QueryFirst(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        public Task<dynamic> QueryFirstAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync((param) => CurrentConnection.QueryFirstAsync(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync((param) => connection.QueryFirstAsync(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        /// <summary>
        /// Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns>dynamic or null</returns>
        ///  <remarks>the row can be accessed via "dynamic", or by casting to an IDictionary<string,object></remarks>
        public dynamic QueryFirstOrDefault(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy((param) => CurrentConnection.QueryFirstOrDefault(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy((param) => connection.QueryFirstOrDefault(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        public Task<dynamic> QueryFirstOrDefaultAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync((param) => CurrentConnection.QueryFirstOrDefaultAsync(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync((param) => connection.QueryFirstOrDefaultAsync(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        /// <summary>
        ///  Return a dynamic object with properties matching the columns.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        /// <exception cref="">Sequence contains no elements</exception>
        public dynamic QuerySingle(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy((param) => CurrentConnection.QuerySingle(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy((param) => connection.QuerySingle(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        public Task<dynamic> QuerySingleAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync((param) => CurrentConnection.QuerySingleAsync(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync((param) => connection.QuerySingleAsync(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        /// <summary>
        /// 内部方法，sql不处理，param会处理
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public dynamic QuerySingleOrDefault(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy((param) => CurrentConnection.QuerySingleOrDefault(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy((param) => connection.QuerySingleOrDefault(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        public Task<dynamic> QuerySingleOrDefaultAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync((param) => CurrentConnection.QuerySingleOrDefaultAsync(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync((param) => connection.QuerySingleOrDefaultAsync(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        /// <summary>
        /// 外部方法，sql不处理，param会处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="buffered"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql, DynamicParameters parameters = null, bool buffered = true, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy<IEnumerable<T>>((param) => CurrentConnection.Query<T>(sql, param, CurrentTransaction, buffered, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<IEnumerable<T>>((param) => connection.Query<T>(sql, param, CurrentTransaction, buffered, CommandTimeout, commandType), sql, parameters);
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync<IEnumerable<T>>((param) => CurrentConnection.QueryAsync<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<IEnumerable<T>>((param) => connection.QueryAsync<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        /// <summary>
        /// 内部方法，sql不处理，param会处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public T QueryFirst<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy<T>((param) => CurrentConnection.QueryFirst<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<T>((param) => connection.QueryFirst<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        public Task<T> QueryFirstAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync<T>((param) => CurrentConnection.QueryFirstAsync<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<T>((param) => connection.QueryFirstAsync<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        /// <summary>
        /// 内部方法，sql不处理，param会处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public T QueryFirstOrDefault<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy<T>((param) => CurrentConnection.QueryFirstOrDefault<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<T>((param) => connection.QueryFirstOrDefault<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync<T>((param) => CurrentConnection.QueryFirstOrDefaultAsync<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<T>((param) => connection.QueryFirstOrDefaultAsync<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        /// <summary>
        /// 内部方法，sql不处理，param会处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public T QuerySingle<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy<T>((param) => CurrentConnection.QuerySingle<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<T>((param) => connection.QuerySingle<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        public Task<T> QuerySingleAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync<T>((param) => CurrentConnection.QuerySingleAsync<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<T>((param) => connection.QuerySingleAsync<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        /// <summary>
        /// 内部方法，sql不处理，param会处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public T QuerySingleOrDefault<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxy<T>((param) => CurrentConnection.QuerySingleOrDefault<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<T>((param) => connection.QuerySingleOrDefault<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }
        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            if (CurrentTransaction != null)
            {
                return DbExecProxyAsync<T>((param) => CurrentConnection.QuerySingleOrDefaultAsync<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
            }
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<T>((param) => connection.QuerySingleOrDefaultAsync<T>(sql, param, CurrentTransaction, CommandTimeout, commandType), sql, parameters);
        }


        #endregion

        #region CRUD
        /// <summary>
        /// 内部方法，sql不处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityToInsert"></param>
        /// <returns></returns>
        public long Insert<T>(T entityToInsert) where T : class
        {
            if (CurrentTransaction != null)
            {
                return CurrentConnection.Insert<T>(entityToInsert, CurrentTransaction, CommandTimeout);
            }
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.Insert<T>(entityToInsert, CurrentTransaction, CommandTimeout);
        }
        public Task<int> InsertAsync<T>(T entityToInsert) where T : class
        {
            if (CurrentTransaction != null)
            {
                return CurrentConnection.InsertAsync<T>(entityToInsert, CurrentTransaction, CommandTimeout);
            }
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.InsertAsync<T>(entityToInsert, CurrentTransaction, CommandTimeout);
        }
        /// <summary>
        /// 内部方法，sql不处理
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="sbColumnList"></param>
        /// <param name="sbParameterList"></param>
        /// <param name="entityToInsert"></param>
        /// <returns></returns>
        public long Insert(string tableName, string sbColumnList, string sbParameterList, object entityToInsert)
        {
            if (CurrentTransaction != null)
            {
                return CurrentConnection.Insert(tableName, sbColumnList, sbParameterList, entityToInsert, CurrentTransaction, CommandTimeout);
            }
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.Insert(tableName, sbColumnList, sbParameterList, entityToInsert, CurrentTransaction, CommandTimeout);
        }
        public Task<long> InsertAsync(string tableName, string sbColumnList, string sbParameterList, object entityToInsert)
        {
            if (CurrentTransaction != null)
            {
                return Task.FromResult(CurrentConnection.Insert(tableName, sbColumnList, sbParameterList, entityToInsert, CurrentTransaction, CommandTimeout));
            }
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return Task.FromResult(connection.Insert(tableName, sbColumnList, sbParameterList, entityToInsert, CurrentTransaction, CommandTimeout));
        }

        public bool Update(FapDynamicObject keyValues)
        {
            string tableName = keyValues.TableName;

            Guard.Against.NullOrEmpty(tableName, nameof(tableName));
            dynamic d = keyValues as dynamic;
            long id = d.Get("Id");
            long ts = d.Get("Ts");

            Guard.Against.Zero(id, nameof(id));

            Guard.Against.Zero(ts, nameof(ts));

            if (CurrentTransaction != null)
            {
                return CurrentConnection.Update(keyValues.TableName, keyValues, CurrentTransaction, CommandTimeout);
            }
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.Update(keyValues.TableName, keyValues, CurrentTransaction, CommandTimeout);
        }
        /// <summary>
        /// 加了乐观锁的更新，Id和Ts为条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityUpdate"></param>
        /// <returns></returns>
        public bool UpdateWithTimestamp<T>(T entityUpdate) where T : BaseModel
        {
            long id = entityUpdate.Id;
            long ts = entityUpdate.Ts;

            Guard.Against.Zero(id, nameof(id));
            Guard.Against.Zero(ts, nameof(ts));
            if (CurrentConnection != null)
            {
                return CurrentConnection.Update(entityUpdate, CurrentTransaction, CommandTimeout);
            }
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.Update(entityUpdate, CurrentTransaction, CommandTimeout);
        }
        /// <summary>
        /// 根据主键更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public bool Update<T>(T entityToUpdate) where T : class
        {
            if (CurrentTransaction != null)
            {
                return CurrentConnection.Update<T>(entityToUpdate, CurrentTransaction, CommandTimeout);
            }
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.Update<T>(entityToUpdate, CurrentTransaction, CommandTimeout);
        }
        public Task<bool> UpdateAsync<T>(T entityToUpdate) where T : class
        {
            if (CurrentTransaction != null)
            {
                return CurrentConnection.UpdateAsync<T>(entityToUpdate, CurrentTransaction, CommandTimeout);
            }
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.UpdateAsync<T>(entityToUpdate, CurrentTransaction, CommandTimeout);
        }
        /// <summary>
        /// 内部方法，sql不处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityToDelete"></param>
        /// <returns></returns>
        public bool Delete<T>(T entityToDelete) where T : class
        {
            if (CurrentTransaction != null)
            {
                return CurrentConnection.Delete<T>(entityToDelete, CurrentTransaction, CommandTimeout);
            }
            using var connection = GetDbConnection(DataSourceEnum.MASTER);

            return connection.Delete<T>(entityToDelete, CurrentTransaction, CommandTimeout);
        }
        public Task<bool> DeleteAsync<T>(T entityToDelete) where T : class
        {
            if (CurrentTransaction != null)
            {
                return CurrentConnection.DeleteAsync<T>(entityToDelete, CurrentTransaction, CommandTimeout);
            }
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.DeleteAsync<T>(entityToDelete, CurrentTransaction, CommandTimeout);
        }
        /// <summary>
        /// 内部方法，sql不处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool DeleteAll<T>() where T : class
        {
            if (CurrentTransaction != null)
            {
                return CurrentConnection.DeleteAll<T>(CurrentTransaction, CommandTimeout);
            }
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.DeleteAll<T>(CurrentTransaction, CommandTimeout);
        }
        public Task<bool> DeleteAllAsync<T>() where T : class
        {
            if (CurrentTransaction != null)
            {
                return CurrentConnection.DeleteAllAsync<T>(CurrentTransaction, CommandTimeout);
            }
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.DeleteAllAsync<T>(CurrentTransaction, CommandTimeout);

        }
        #endregion

        #region transaction
        public void TransactionProxy(Action<IDbConnection, IDbTransaction> execAction)
        {
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            using var transaction = connection.BeginTransaction();
            try
            {
                execAction?.Invoke(connection, transaction);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new FapException($"事务执行异常:{ex.Message}", ex);
            }
        }

        #endregion

        #region Transaction
        public void BeginTransaction()
        {
            CurrentConnection = GetDbConnection(DataSourceEnum.MASTER);
            if (CurrentConnection.State == ConnectionState.Closed)
            {
                CurrentConnection.Open();
            }
            CurrentTransaction = CurrentConnection.BeginTransaction();
        }
        public void Commit()
        {
            CurrentTransaction?.Commit();
            CurrentTransaction?.Dispose();
            if (CurrentConnection?.State == ConnectionState.Open)
            {
                CurrentConnection.Close();
            }
            CurrentConnection?.Dispose();
        }
        public void Rollback()
        {
            CurrentTransaction?.Rollback();
            CurrentTransaction?.Dispose();
            if (CurrentConnection?.State == ConnectionState.Open)
            {
                CurrentConnection.Close();
            }
            CurrentConnection?.Dispose();
        }
        public void Dispose()
        {
            CurrentConnection = null;
            CurrentTransaction = null;
        }

        #endregion
    }
}