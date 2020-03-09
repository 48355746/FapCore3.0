using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Metadata;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fap.Core.Extensions
{
    public static class JObjectExtension
    {
        /// <summary>
        /// 从JObject对象中获取字符串的值
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string GetStringValue(this JObject jobj, string key, string defaultValue)
        {
            string result = string.Empty;
            JToken t = jobj[key];
            if (t != null)
            {
                result = t.ToString();
            }
            else
            {
                result = defaultValue;
            }

            return result;
        }

        public static string GetStringValue(this JToken t, string key, string defaultValue)
        {
            string result = string.Empty;
            if (t != null)
            {
                result = t.ToString();
            }
            else
            {
                result = defaultValue;
            }

            return result;
        }
        /// <summary>
        /// 从JObject对象中获取字符串的值
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetStringValue(this JObject jobj, string key)
        {
            return GetStringValue(jobj, key, "");
        }

        public static string GetStringValue(this JToken jobj, string key)
        {
            return GetStringValue(jobj, key, "");
        }

        /// <summary>
        /// 从JObject对象中获取整型的值
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int GetIntValue(this JObject jobj, string key, int defaultValue)
        {
            int result = 0;
            JToken t = jobj[key];
            if (t != null)
            {
                result = int.Parse(t.ToString());
            }
            else
            {
                result = defaultValue;
            }

            return result;
        }
        /// <summary>
        /// 从JObject对象中获取整型的值
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetIntValue(this JObject jobj, string key)
        {
            return GetIntValue(jobj, key, 0);
        }

        /// <summary>
        /// 从JObject对象中获取DOUBLE的值
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static double GetDoubleValue(this JObject jobj, string key, double defaultValue)
        {
            double result = 0.0D;
            JToken t = jobj[key];
            if (t != null)
            {
                result = double.Parse(t.ToString());
            }
            else
            {
                result = defaultValue;
            }

            return result;
        }

        /// <summary>
        /// 从JObject对象中获取DOUBLE的值
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static double GetDoubleValue(this JObject jobj, string key)
        {
            return GetDoubleValue(jobj, key, 0.0D);
        }

        /// <summary>
        /// 判断是否存在值
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasKey(this JObject jobj, string key)
        {
            return jobj.Property(key) != null;
        }

        /// <summary>
        /// JObject对象转换成FapDynamicObject对象
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="dynamciObj">FapDynamicObject对象</param>
        /// <param name="excludeKeys">指定字段，排除要赋值的字段</param>
        /// <returns></returns>
        public static FapDynamicObject ToFapDynamicObject(this JObject jobj, IEnumerable<FapColumn> columnList, params string[] excludeKeys)
        {
            FapDynamicObject dynamciObj = new FapDynamicObject(columnList);
            IEnumerable<JProperty> ojp = jobj.Properties();
            foreach (var item in ojp)
            {
                bool isExclude = false;
                foreach (var excludeKey in excludeKeys)
                {
                    if (excludeKey.Equals(item.Name))
                    {
                        isExclude = true;
                        break;
                    }
                }

                if (isExclude) continue;

                if ("id".Equals(item.Name))
                {
                    if (!(string.IsNullOrEmpty(item.Value.ToString()) || "_empty".Equals(item.Value.ToString())))
                    {
                        dynamciObj.SetValue("Id", item.Value.ToString());
                    }
                }
                else
                {
                    FapColumn column = columnList.Where(c => c.ColName == item.Name).FirstOrDefault();
                    if (column != null)
                    {
                        if (column.IsIntType()) //整型
                        {
                            dynamciObj.SetValue(item.Name, item.Value.ToInt());
                        }
                        else if (column.IsLongType()) //长整型
                        {
                            dynamciObj.SetValue(item.Name, item.Value.ToLong());
                        }
                        else if (column.IsDoubleType()) //浮点型
                        {
                            dynamciObj.SetValue(item.Name, item.Value.ToDouble());
                        }
                        else //字符串
                        {
                            dynamciObj.SetValue(item.Name, item.Value.ToString());
                        }
                    }
                    else
                    {
                        dynamciObj.SetValue(item.Name, item.Value.ToString());
                    }
                }
            }
            return dynamciObj;
        }

        public static dynamic ToFapDynamicObject(this JObject jobj, IFapPlatformDomain appDomain, string tableName, params string[] excludeKeys)
        {
            if (appDomain.ColumnSet.TryGetValueByTable(tableName, out IEnumerable<FapColumn> columns))
            {
                columns = columns.Where(f => f.IsObsolete == 0).ToList();
            }
            else
            {
                columns = new List<FapColumn>();
            }
            return JObjectExtension.ToFapDynamicObject(jobj, columns,  excludeKeys);
        }

        /// <summary>
        /// 将JObject转换成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jobj"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        [Obsolete("推荐使用 Newtonsoft JSON library的 ToObject方法")]
        public static T ConvertToEntity<T>(this JObject jobj, ref T obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties();
            foreach (var p in properties)
            {
                string propertyName = p.Name;
                JProperty jp = jobj.Property(propertyName);
                if (jp != null)
                {
                    if (p.PropertyType.Equals(typeof(string)))
                    {
                        p.SetValue(obj, jobj.GetStringValue(propertyName), null);
                    }
                    else if (p.PropertyType.Equals(typeof(int)))
                    {
                        p.SetValue(obj, jobj.GetIntValue(propertyName), null);
                    }
                    else if (p.PropertyType.Equals(typeof(double)))
                    {
                        p.SetValue(obj, jobj.GetDoubleValue(propertyName), null);
                    }
                }
            }

            return obj;
        }
    }
}
