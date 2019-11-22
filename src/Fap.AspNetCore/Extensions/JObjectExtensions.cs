using Fap.Core.Metadata;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fap.Core.Extensions;
using Newtonsoft.Json;

namespace Fap.AspNetCore.Extensions
{
    public static class JObjectExtensions
    {
        /// <summary>
        /// JObject对象转换成FapDynamicObject对象
        /// </summary>
        /// <param name="jobj"></param>
        /// <param name="dynamciObj">FapDynamicObject对象</param>
        /// <param name="excludeKeys">指定字段，排除要赋值的字段</param>
        /// <returns></returns>
        public static dynamic ToFapDynamicObject(this JObject jobj, IEnumerable<FapColumn> columnList,  params string[] excludeKeys)
        {
            dynamic dynamciObj = new FapDynamicObject(columnList.First().TableName);
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
                        dynamciObj.Add("Id", item.Value.ToString());
                    }
                }
                else
                {
                    FapColumn column = columnList.Where(c => c.ColName == item.Name).FirstOrDefault();
                    if (column != null)
                    {
                        if (column.IsIntType()) //整型
                        {
                            dynamciObj.Add(item.Name, item.Value.ToString().ToInt());
                        }
                        else if (column.IsLongType()) //长整型
                        {
                            dynamciObj.Add(item.Name, item.Value.ToString().ToLong());
                        }
                        else if (column.IsDoubleType()) //浮点型
                        {
                            dynamciObj.Add(item.Name, item.Value.ToString().ToDecimal());
                        }
                        else //字符串
                        {
                            dynamciObj.Add(item.Name, item.Value.ToString());
                        }
                    }
                    else
                    {
                        dynamciObj.Add(item.Name, item.Value.ToString());
                    }
                }
            }
            return dynamciObj;
        }

        
    }
}
