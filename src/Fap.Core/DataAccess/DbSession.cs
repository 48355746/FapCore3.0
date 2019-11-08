using Ardalis.GuardClauses;
using Dapper;
using Dapper.Contrib.Extensions;
using Fap.Core.DI;
using Fap.Core.Exceptions;
using Fap.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Fap.Core.DataAccess
{
    [Service(ServiceLifetime.Singleton, InterfaceType = typeof(IDbSession))]
    public class DbSession : IDbSession
    {
        private ILogger<DbSession> _logger;
        private int? _commandTimeout;
        /// <summary>
        /// 数据库连接工厂
        /// </summary>
        public IConnectionFactory ConnectionFactory { get; private set; }


        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="conn">连接</param>
        public DbSession(ILoggerFactory loggerFactory, IConnectionFactory connectionFactory)
        {
            _logger = loggerFactory.CreateLogger<DbSession>();
            ConnectionFactory = connectionFactory;
            // _commandTimeout = commandTimeOut;
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
                message = $"SQL语句为：{sql},{Environment.NewLine}参数:{(parameters != null ? parameters.ToString() : "")}";
                message += $"{Environment.NewLine}执行时间：{ timer.ElapsedMilliseconds}毫秒";
                System.Diagnostics.Debug.WriteLine(message);
                _logger.LogInformation(message);
                return result;
            }
            catch (Exception ex)
            {
                message = $"查询SQL异常：{ ex.Message },{Environment.NewLine}SQL语句为：{sql}{Environment.NewLine}参数:{(parameters != null ? parameters.ToString() : "")}";
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
                System.Diagnostics.Debug.WriteLine(message);
                _logger.LogInformation(message);
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


        //private string WrapSelectSql(string sql)
        //{
        //    CommandBuilder commandBuilder = new CommandBuilder();
        //    ICommand command = commandBuilder.GetCommand(sql);
        //    if (command is SelectBuilder)
        //    {
        //        SelectBuilder select = command as SelectBuilder;
        //        var tables = select.Sources;
        //        var where = select.Where;
        //        foreach (AliasedSource tb in tables.Sources)
        //        {
        //            //添加有效期验证
        //            FilterGroup validFilter = new FilterGroup(Conjunction.And,
        //               new LessThanEqualToFilter(tb.Column(FapConstants.FAPCOLUMN_FIELD_EnableDate), new ParameterLiteral("@CurrentDate")),
        //               new GreaterThanEqualToFilter(tb.Column(FapConstants.FAPCOLUMN_FIELD_DisableDate), new ParameterLiteral("@CurrentDate")),
        //               new EqualToFilter(tb.Column(FapConstants.FAPCOLUMN_FIELD_Dr), new ParameterLiteral("@Dr")));
        //            select.AddWhere(validFilter);
        //        }
        //    }
        //    Formatter formatter = new Formatter();
        //    string wrapSql = formatter.GetCommandText(command);
        //    _logger.LogInformation(wrapSql);
        //    return wrapSql;
        //}
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
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return DbExecProxy<int>((param) => connection.Execute(sql, param, null, null, commandType), sql, parameters);
        }
        //    => DbExecProxy<int>((param) => Connection.Execute(sql, param, null, _commandTimeout, commandType), sql, parameters);
        public Task<int> ExecuteAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return DbExecProxyAsync<int>((param) => connection.ExecuteAsync(sql, param, null, null, commandType), sql, parameters);
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
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<object>((param) => connection.ExecuteScalar(sql, param, null, null, commandType), sql, parameters);
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
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<object>((param) => connection.ExecuteScalarAsync(sql, param, null, _commandTimeout, commandType), sql, parameters);
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
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<T>((param) => connection.ExecuteScalar<T>(sql, param, null, _commandTimeout, commandType), sql, parameters);
        }
        public Task<T> ExecuteScalarAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<T>((param) => connection.ExecuteScalarAsync<T>(sql, param, null, _commandTimeout, commandType), sql, parameters);
        }
        #endregion
        #region query
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
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<IEnumerable<dynamic>>((param) => connection.Query(sql, param, null, buffered, _commandTimeout, commandType), sql, parameters);
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
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<IEnumerable<dynamic>>((param) => connection.QueryAsync(sql, param, null, _commandTimeout, commandType), sql, parameters);
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
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy((param) => connection.QueryFirst(sql, param, null, _commandTimeout, commandType), sql, parameters);
        }
        public Task<dynamic> QueryFirstAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync((param) => connection.QueryFirstAsync(sql, param, null, _commandTimeout, commandType), sql, parameters);
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
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy((param) => connection.QueryFirstOrDefault(sql, param, null, _commandTimeout, commandType), sql, parameters);
        }
        public Task<dynamic> QueryFirstOrDefaultAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync((param) => connection.QueryFirstOrDefaultAsync(sql, param, null, _commandTimeout, commandType), sql, parameters);
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
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy((param) => connection.QuerySingle(sql, param, null, _commandTimeout, commandType), sql, parameters);
        }
        public Task<dynamic> QuerySingleAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync((param) => connection.QuerySingleAsync(sql, param, null, _commandTimeout, commandType), sql, parameters);
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
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy((param) => connection.QuerySingleOrDefault(sql, param, null, _commandTimeout, commandType), sql, parameters);
        }
        public Task<dynamic> QuerySingleOrDefaultAsync(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync((param) => connection.QuerySingleOrDefaultAsync(sql, param, null, _commandTimeout, commandType), sql, parameters);
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
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<IEnumerable<T>>((param) => connection.Query<T>(sql, param, null, buffered, _commandTimeout, commandType), sql, parameters);
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<IEnumerable<T>>((param) => connection.QueryAsync<T>(sql, param, null, _commandTimeout, commandType), sql, parameters);
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
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<T>((param) => connection.QueryFirst<T>(sql, param, null, _commandTimeout, commandType), sql, parameters);
        }
        public Task<T> QueryFirstAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<T>((param) => connection.QueryFirstAsync<T>(sql, param, null, _commandTimeout, commandType), sql, parameters);
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
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<T>((param) => connection.QueryFirstOrDefault<T>(sql, param, null, _commandTimeout, commandType), sql, parameters);
        }
        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<T>((param) => connection.QueryFirstOrDefaultAsync<T>(sql, param, null, _commandTimeout, commandType), sql, parameters);
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
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<T>((param) => connection.QuerySingle<T>(sql, param, null, _commandTimeout, commandType), sql, parameters);
        }
        public Task<T> QuerySingleAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<T>((param) => connection.QuerySingleAsync<T>(sql, param, null, _commandTimeout, commandType), sql, parameters);
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
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxy<T>((param) => connection.QuerySingleOrDefault<T>(sql, param, null, _commandTimeout, commandType), sql, parameters);
        }
        public Task<T> QuerySingleOrDefaultAsync<T>(string sql, DynamicParameters parameters = null, CommandType? commandType = null)
        {
            using var connection = GetDbConnection(DataSourceEnum.SLAVE);
            return DbExecProxyAsync<T>((param) => connection.QuerySingleOrDefaultAsync<T>(sql, param, null, _commandTimeout, commandType), sql, parameters);
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
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.Insert<T>(entityToInsert, null, _commandTimeout);
        }
        public Task<int> InsertAsync<T>(T entityToInsert) where T : class
        {
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.InsertAsync<T>(entityToInsert, null, _commandTimeout);
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
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.Insert(tableName, sbColumnList, sbParameterList, entityToInsert, null, _commandTimeout);
        }
        public Task<long> InsertAsync(string tableName, string sbColumnList, string sbParameterList, object entityToInsert)
        {
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return Task.FromResult(connection.Insert(tableName, sbColumnList, sbParameterList, entityToInsert, null, _commandTimeout));
        }


        /// <summary>
        /// 内部方法，sql不处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public bool Update<T>(T entityToUpdate) where T : class
        {
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.Update<T>(entityToUpdate, null, _commandTimeout);
        }
        public Task<bool> UpdateAsync<T>(T entityToUpdate) where T : class
        {
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.UpdateAsync<T>(entityToUpdate, null, _commandTimeout);
        }
        /// <summary>
        /// 内部方法，sql不处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityToDelete"></param>
        /// <returns></returns>
        public bool Delete<T>(T entityToDelete) where T : class
        {
            using var connection = GetDbConnection(DataSourceEnum.MASTER);

            return connection.Delete<T>(entityToDelete, null, _commandTimeout);
        }
        public Task<bool> DeleteAsync<T>(T entityToDelete) where T : class
        {
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.DeleteAsync<T>(entityToDelete, null, _commandTimeout);
        }
        /// <summary>
        /// 内部方法，sql不处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool DeleteAll<T>() where T : class
        {
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.DeleteAll<T>(null, _commandTimeout);
        }
        public Task<bool> DeleteAllAsync<T>() where T : class
        {
            using var connection = GetDbConnection(DataSourceEnum.MASTER);
            return connection.DeleteAllAsync<T>(null, _commandTimeout);

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
    }
}