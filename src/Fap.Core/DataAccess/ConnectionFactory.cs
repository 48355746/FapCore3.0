using Fap.Core.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Threading;
using Dapper.Contrib.Extensions;
using StackExchange.Profiling.Data;
using StackExchange.Profiling;

namespace Fap.Core.DataAccess
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly ILogger _logger;
        private static ThreadLocal<DataSourceEnum> threadLocal = new ThreadLocal<DataSourceEnum>();
        static ConnectionFactory()
        {
            //设置dapper的tableName取值
            SqlMapperExtensions.TableNameMapper = (type) => type.Name;
        } 

        /// <summary>
        /// 当前线程数据源 
        /// </summary>
        /// <param name="sourceEnum"></param>     
        public DataSourceEnum DataSource
        {
            set { threadLocal.Value = value; }
            get { return threadLocal.Value; }
        }

        /// <summary>
        /// 主数据库连接串
        /// </summary>
        private string MasterConnectionString { get; set; }
        /// <summary>
        /// 从数据库连接串集合
        /// </summary>
        private List<string> SlaverConnectionStrings { get; set; } = new List<string>();
        public ConnectionFactory(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ConnectionFactory>();
            var connectionKeys = configuration.GetSection("ConnectionString").GetChildren().Select(s => s.Key).ToArray();
            foreach (var connKey in connectionKeys)
            {
                var connSplit = connKey.Split('_');
                if (connSplit.Length == 1)
                {
                    MasterConnectionString = configuration[$"ConnectionString:{connKey}"];
                    DatabaseDialect = DialectDictionary[connKey];
                }
                else
                {
                    SlaverConnectionStrings.Add(configuration[$"ConnectionString:{connKey}"]);
                }

            }
            GetSlaverConnection();
        }
        /// <summary>
        /// 数据库方言集合
        /// </summary>
        private readonly Dictionary<string, DatabaseDialectEnum> DialectDictionary
          = new Dictionary<string, DatabaseDialectEnum>
          {
              ["sqlconnection"] = DatabaseDialectEnum.MSSQL,
              ["sqlceconnection"] = DatabaseDialectEnum.SQLCE,
              ["npgsqlconnection"] = DatabaseDialectEnum.POSTGRES,
              ["sqliteconnection"] = DatabaseDialectEnum.SQLLITE,
              ["mysqlconnection"] = DatabaseDialectEnum.MYSQL,
              ["fbconnection"] = DatabaseDialectEnum.FIREBASE
          };
        /// <summary>
        /// 数据库方言
        /// </summary>
        public DatabaseDialectEnum DatabaseDialect { get; private set; }

        private IDbConnection GetConnection(string connectionString) => DatabaseDialect switch
        {
            DatabaseDialectEnum.MSSQL =>new ProfiledDbConnection(new SqlConnection(connectionString),MiniProfiler.Current),
            DatabaseDialectEnum.MYSQL => new ProfiledDbConnection(new MySqlConnection(connectionString), MiniProfiler.Current),
            _ => throw new NotImplementedException()
        };
        public IDbConnection GetDbConnection()
        {
            if (DataSource == DataSourceEnum.MASTER)
            {
                return GetMasterConnection();
            }
            else
            {
                return GetSlaverConnection();
            }
        }
        private IDbConnection GetMasterConnection()
        {
            return GetConnection(MasterConnectionString);
        }
        private IDbConnection GetSlaverConnection()
        {
            int sc = SlaverConnectionStrings.Count();
            if (sc > 0)
            {
                Random random = new Random();
                int index = random.Next(0, sc);
                return GetConnection(SlaverConnectionStrings[index]);
            }
            else
            {
                _logger.LogInformation("没有设置从库，将从建立主库连接");
                return GetMasterConnection();
            }
        }    
    }

    public enum DataSourceEnum
    {
        MASTER,
        SLAVE
    }
}
