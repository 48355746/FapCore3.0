using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
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
