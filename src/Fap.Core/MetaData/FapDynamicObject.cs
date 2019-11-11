using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Fap.Core.Metadata
{
    /// <summary>
    /// 动态类型对象，用来获取动态数据
    /// by sunchangtan
    /// </summary>
    [Serializable]
    public class FapDynamicObject : System.Dynamic.DynamicObject, IEnumerable<KeyValuePair<string, object>>
    {        
        private IDictionary<string, object> map = new Dictionary<string, object>();     
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return map.Keys;
        }
        /// <summary>
        /// 重写get方法
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            if (map.ContainsKey(binder.Name))
            {
                result = map[binder.Name];
                return true;
            }
            else if (map.ContainsKey(TableName + "_" + binder.Name))
            {
                result = map[TableName + "_" + binder.Name];
                return true;
            }
            else
            {
                result = "Invalid Property!";
                return false;
            }
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            map[binder.Name] = value;

            return true;
        }
        /// <summary>
        /// 本身直接转换为Dictionary。例如Dictionary dic=dynamicObj;
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.Type == typeof(IDictionary<string, object>))
            {
                result = map;
                return true;
            }
            return base.TryConvert(binder, out result);
        }
        public const string OperGet = "Get";
        public const string OperAdd = "Add";
        public const string OperKeys = "Keys";
        public const string OperContainsKey = "ContainsKey";
        public const string OperRemove = "Remove";
        public const string OperParamKeys = "ParamKeys";
        public const string OperColumnKeys = "ColumnKeys";
        public override bool TryInvokeMember(System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
        {
            if (binder.Name == OperAdd && binder.CallInfo.ArgumentCount == 2)
            {
                string key = args[0] as string;
                if (key == null)
                {
                    //throw new ArgumentException("name");
                    result = null;
                    return false;
                }
                object value = args[1];
                if (map == null)
                {
                    map = new Dictionary<string, object>();
                }
                if (map.TryAdd(key, value))
                {
                    result = value;
                }
                else
                {
                    result = null;
                }
                return true;

            }
            else if (binder.Name ==OperGet && binder.CallInfo.ArgumentCount == 1)
            {
                string key = args[0] as string;
                if (map.TryGetValue(key, out object obj))
                {
                    result = obj;
                }
                else
                {
                    result = null;
                }
                return true;
            }
            else if (binder.Name == OperKeys && binder.CallInfo.ArgumentCount == 0)
            {
                result = new List<string>(map.Keys);
                return true;
            }
            else if (binder.Name == OperContainsKey && binder.CallInfo.ArgumentCount == 1)
            {
                string key = args[0] as string;
                result = map.ContainsKey(key);
                return true;
            }
            else if (binder.Name == OperRemove && binder.CallInfo.ArgumentCount == 1)
            {
                string key = args[0] as string;
                if (map.ContainsKey(key))
                {
                    result = map.Remove(key);
                }
                else
                {
                    result = false;
                }
                return true;
            }
            else if (binder.Name == OperParamKeys && binder.CallInfo.ArgumentCount == 0)
            {
                List<string> paramKeys = new List<string>();
                foreach (var key in map.Keys)
                {
                    //排除自增长Id
                    if (key.Equals("Id", StringComparison.CurrentCultureIgnoreCase))
                        continue;
                    //排除带MC的字段
                    if (key.EndsWith("MC"))
                        continue;
                    paramKeys.Add($"@{key}");
                }
                result = paramKeys;
                return true;
            }
            else if (binder.Name == OperColumnKeys && binder.CallInfo.ArgumentCount == 0)
            {
                List<string> columnKeys = new List<string>();
                foreach (var key in map.Keys)
                {
                    //排除自增长Id
                    if (key.Equals("Id", StringComparison.CurrentCultureIgnoreCase))
                        continue;
                    //排除带MC的字段
                    if (key.EndsWith("MC"))
                        continue;
                    columnKeys.Add(key);
                }
                result = columnKeys;
                return true;
            }
            return base.TryInvokeMember(binder, args, out result);
        }

        public override int GetHashCode()
        {
            int hashCode = 17; // we *know* we are using this in a dictionary, so pre-compute this
            List<string> keyList = new List<string>(map.Keys);
            foreach (var key in keyList)
            {
                object obj = map[key];
                hashCode = hashCode * 23 + (obj == null ? 0 : obj.GetHashCode());
            }
            return hashCode;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return map.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return map.GetEnumerator();
        }

       
    }
}
