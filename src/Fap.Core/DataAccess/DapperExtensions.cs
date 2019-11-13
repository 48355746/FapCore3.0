using Ardalis.GuardClauses;
using Dapper;
using Fap.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fap.Core.DataAccess
{
    public static class DapperExtensions
    {
        public static long Insert(this IDbConnection connection, string tableName, string sbColumnList, string sbParameterList, object entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var isList = false;

            var type = entityToInsert.GetType();

            if (type.IsArray)
            {
                isList = true;
                type = type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                isList = true;
                type = type.GetGenericArguments()[0];
            }

            var name = tableName;

            var adapter = GetFormatter(connection);

            int returnVal;
            var wasClosed = connection.State == ConnectionState.Closed;
            if (wasClosed) connection.Open();

            if (!isList)    //single entity
            {
                var keyProperties = new List<PropertyInfo>();
                returnVal = adapter.Insert(connection, transaction, commandTimeout, name, sbColumnList,
                    sbParameterList, keyProperties, entityToInsert);
            }
            else
            {
                //insert list of entities
                var cmd = $"insert into {name} ({sbColumnList}) values ({sbParameterList})";
                returnVal = connection.Execute(cmd, entityToInsert, transaction, commandTimeout);
            }
            if (wasClosed) connection.Close();
            return returnVal;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sbColumnList"></param>
        /// <param name="sbParamterList"></param>
        /// <param name="dynamicToUpdate"></param>
        /// <param name="isTrace">可跟踪历史</param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static bool Update(this IDbConnection connection,string tableName,dynamic dynamicToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var id = dynamicToUpdate.Get("Id");
            if (id == null)
            {
                Guard.Against.NullOrEmpty("更新数据Id必须设置.", nameof(dynamicToUpdate));
            }
            var adapter = GetFormatter(connection);
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" UPDATE ").Append(tableName).Append(" SET ");
            foreach (string fildName in dynamicToUpdate.Keys())
            {
                if ("Id".EqualsWithIgnoreCase(fildName))
                {
                    continue;
                }
                adapter.AppendColumnNameEqualsValue(sqlBuilder, fildName);
                sqlBuilder.Append(",");
            }
            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.Append(" where ");
            adapter.AppendColumnNameEqualsValue(sqlBuilder, "Id");
            DynamicParameters parameters= dynamicToUpdate.Parameters();
            int updated= connection.Execute(sqlBuilder.ToString(), parameters, transaction, commandTimeout);
            return updated > 0;
        }
       


        private static readonly Dictionary<string, ISqlAdapter> AdapterDictionary
           = new Dictionary<string, ISqlAdapter>
           {
               ["sqlconnection"] = new SqlServerAdapter(),
               ["sqlceconnection"] = new SqlCeServerAdapter(),
               ["npgsqlconnection"] = new PostgresAdapter(),
               ["sqliteconnection"] = new SQLiteAdapter(),
               ["mysqlconnection"] = new MySqlAdapter(),
               ["fbconnection"] = new FbAdapter()
           };
        private static ISqlAdapter GetFormatter(IDbConnection connection)
        {
            var name = connection.GetType().Name.ToLower();

            return !AdapterDictionary.ContainsKey(name)
                ? new SqlServerAdapter()
                : AdapterDictionary[name];
        }


    }
}
