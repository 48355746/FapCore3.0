using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Fap.Core.MetaData
{

    [Serializable]
    public class BaseModel : ICloneable
    {
        [Key]
        public long Id { get; set; }
        public string Fid { get; set; }
        /// <summary>
        /// 所属组织
        /// </summary>
        public string OrgUid { get; set; }
        /// <summary>
        /// 所属集团
        /// </summary>
        public string GroupUid { get; set; }
        /// <summary>
        /// 有效开始时间
        /// </summary>
        public string EnableDate { get; set; }
        /// <summary>
        /// 有效截至时间
        /// </summary>
        public string DisableDate { get; set; } // 有效截至时间
        /// <summary>
        /// 删除标记， 默认是0
        /// </summary>
        public int Dr { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public long Ts { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateBy { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreateName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateDate { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>
        public string UpdateBy { get; set; }
        /// <summary>
        /// 更新人名称
        /// </summary>
        public string UpdateName { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdateDate { get; set; }
        /// <summary>
        /// 扩展字段
        /// </summary>
        [Computed]
        public object Exp { get; set; }
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// 将动态对象转换为实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dynamicObj"></param>
        /// <returns></returns>
        public static T ToEntity<T>(dynamic dynamicObj) where T : BaseModel
        {
            //T entity = default(T);
            Type t = typeof(T);
            T entity = (T)Activator.CreateInstance(t);
            IDictionary<string, object> dataDict = dynamicObj.Data;
            foreach (var item in dataDict)
            {
                PropertyInfo pi = t.GetProperty(item.Key);
                if (pi != null && pi.CanWrite)
                {
                    //pi.SetValue(entity, item.Value, null);
                    if (pi.PropertyType == typeof(string))
                    {
                        var value = item.Value != null ? item.Value.ToString() : "";
                        pi.SetValue(entity, value, null);
                    }
                    else if (pi.PropertyType == typeof(int))
                    {

                        int value = 0;
                        if (item.Value != null)
                        {
                            int.TryParse(item.Value.ToString(), out value);
                        }
                        pi.SetValue(entity, value, null);
                    }
                    else if (!pi.PropertyType.IsGenericType)
                    {
                        var value = item.Value != null ? item.Value.ToString() : "";
                        //非泛型
                        pi.SetValue(entity, string.IsNullOrEmpty(value) ? null : Convert.ChangeType(value, pi.PropertyType), null);
                    }
                    else
                    {
                        var value = item.Value != null ? item.Value.ToString() : "";
                        //泛型Nullable<>
                        Type genericTypeDefinition = pi.PropertyType.GetGenericTypeDefinition();
                        if (genericTypeDefinition == typeof(Nullable<>))
                        {
                            pi.SetValue(entity, string.IsNullOrEmpty(value) ? null : Convert.ChangeType(value, Nullable.GetUnderlyingType(pi.PropertyType)), null);
                        }
                    }
                }
            }

            return entity;
        }

    }
}