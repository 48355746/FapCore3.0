using Ardalis.GuardClauses;
using Dapper;
using Dapper.Contrib.Extensions;
using Fap.Core.Extensions;
using Fap.Core.Metadata;
using Fap.Core.Utility;
using System;
using System.Collections.Concurrent;
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
        public static bool Update(this IDbConnection connection, string tableName, dynamic dynamicToUpdate, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var id = dynamicToUpdate.Get(FapDbConstants.FAPCOLUMN_FIELD_Id);
            if (id == null)
            {
                Guard.Against.NullOrEmpty("更新数据Id必须设置.", nameof(dynamicToUpdate));
            }
            var ts = dynamicToUpdate.Get(FapDbConstants.FAPCOLUMN_FIELD_Ts);
            if (ts == null)
            {
                Guard.Against.NullOrEmpty("更新数据Ts必须设置.", nameof(dynamicToUpdate));
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
            sqlBuilder.Append(" and ");
            adapter.AppendColumnName(sqlBuilder, "Ts");
            sqlBuilder.Append($" = {ts}");
            DynamicParameters parameters = dynamicToUpdate.DynamicParameters();
            parameters.Add("Ts", UUIDUtils.Ts);
            //Console.WriteLine(sqlBuilder.ToString());
            int updated = connection.Execute(sqlBuilder.ToString(), parameters, transaction, commandTimeout);
            return updated > 0;
        }

        public static bool Update<T>(this IDbConnection connection, T entityUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : BaseModel
        {
            var id = entityUpdate.Id ?? 0;
            if (id < 1)
            {
                Guard.Against.NullOrEmpty("更新数据Id必须设置.", nameof(entityUpdate));
            }
            var ts = entityUpdate.Ts ?? 0;
            if (ts < 1)
            {
                Guard.Against.NullOrEmpty("更新数据Ts必须设置.", nameof(entityUpdate));
            }
            var type = typeof(T);
            var adapter = GetFormatter(connection);
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.Append(" UPDATE ").Append(typeof(T).Name).Append(" SET ");
            var allProperties = TypePropertiesCache(type);
            var keyProperties = KeyPropertiesCache(type);
            var computedProperties = ComputedPropertiesCache(type);
            var allPropertiesExceptKeysAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            for (int i = 0; i < allPropertiesExceptKeysAndComputed.Count; i++)
            {
                var property = allPropertiesExceptKeysAndComputed[i];                
                adapter.AppendColumnNameEqualsValue(sqlBuilder, property.Name);
                sqlBuilder.Append(",");
            }
            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);
            sqlBuilder.Append(" where ");
            adapter.AppendColumnNameEqualsValue(sqlBuilder, "Id");
            sqlBuilder.Append(" and ");
            adapter.AppendColumnName(sqlBuilder, "Ts");
            sqlBuilder.Append($" = {entityUpdate.Ts}");
            entityUpdate.Ts = UUIDUtils.Ts;

            int updated = connection.Execute(sqlBuilder.ToString(), entityUpdate, transaction, commandTimeout);
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
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ComputedProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static List<PropertyInfo> TypePropertiesCache(Type type)
        {
            if (TypeProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pis))
            {
                return pis.ToList();
            }

            var properties = type.GetProperties().Where(IsWriteable).ToArray();
            TypeProperties[type.TypeHandle] = properties;
            return properties.ToList();
        }

        private static bool IsWriteable(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(typeof(WriteAttribute), false).AsList();
            if (attributes.Count != 1) return true;

            var writeAttribute = (WriteAttribute)attributes[0];
            return writeAttribute.Write;
        }
        private static List<PropertyInfo> ComputedPropertiesCache(Type type)
        {
            if (ComputedProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pi))
            {
                return pi.ToList();
            }

            var computedProperties = TypePropertiesCache(type).Where(p => p.GetCustomAttributes(true).Any(a => a is ComputedAttribute)).ToList();

            ComputedProperties[type.TypeHandle] = computedProperties;
            return computedProperties;
        }
        private static List<PropertyInfo> KeyPropertiesCache(Type type)
        {
            if (KeyProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pi))
            {
                return pi.ToList();
            }

            var allProperties = TypePropertiesCache(type);
            var keyProperties = allProperties.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).ToList();

            if (keyProperties.Count == 0)
            {
                var idProp = allProperties.Find(p => string.Equals(p.Name, "id", StringComparison.CurrentCultureIgnoreCase));
                if (idProp != null && !idProp.GetCustomAttributes(true).Any(a => a is ExplicitKeyAttribute))
                {
                    keyProperties.Add(idProp);
                }
            }

            KeyProperties[type.TypeHandle] = keyProperties;
            return keyProperties;
        }
    }
}
