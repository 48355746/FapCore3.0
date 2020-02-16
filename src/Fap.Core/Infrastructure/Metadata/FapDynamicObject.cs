using Ardalis.GuardClauses;
using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.MultiLanguage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Fap.Core.Infrastructure.Metadata
{
    /// <summary>
    /// 动态类型对象，用来获取动态数据
    /// by sunchangtan
    /// </summary>
    public class FapDynamicObject
        : IDynamicMetaObjectProvider
        , IDictionary<string, object>
        , IReadOnlyDictionary<string, object>
    {
        //存储值
        private IDictionary<string, object> fapKeyValues = new Dictionary<string, object>();
        private IEnumerable<FapColumn> _fapColumns;
        private IEnumerable<string> allColNames;
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string PrimaryKey { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tableName">table name</param>
        /// <param name="id">primarykey value</param>
        /// <param name="ts">timestamp</param>
        public FapDynamicObject(IEnumerable<FapColumn> fapColumns)
        {
            Guard.Against.Null(fapColumns, nameof(fapColumns));
            _fapColumns = fapColumns;
            TableName = fapColumns.First().TableName;
            PrimaryKey = fapColumns.First(c => c.ColType == FapColumn.COL_TYPE_PK)?.ColName ?? FapDbConstants.FAPCOLUMN_FIELD_Id;
            allColNames = fapColumns.Select(c => c.ColName).Union(GetExtColNames(fapColumns));
        }
        private IEnumerable<string> GetExtColNames(IEnumerable<FapColumn> columns)
        {
            var rcs= columns.Where(c => c.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX || c.CtrlType == FapColumn.CTRL_TYPE_REFERENCE);
            foreach (var rc in rcs)
            {
                yield return rc.ColName + "MC";
            }
            var lcs= columns.Where(c => c.IsMultiLang == 1);
            var langs = typeof(MultiLanguageEnum).EnumItems();
            foreach (var lc in lcs)
            {
                foreach (var lang in langs)
                {
                    yield return lc.ColName + lang.Value;
                }
            }
        }
        public override int GetHashCode()
        {
            int hashCode = 17; // we *know* we are using this in a dictionary, so pre-compute this
            List<string> keyList = new List<string>(fapKeyValues.Keys);
            foreach (var key in keyList)
            {
                object obj = fapKeyValues[key];
                hashCode = hashCode * 23 + (obj == null ? 0 : obj.GetHashCode());
            }
            return hashCode;
        }

        int ICollection<KeyValuePair<string, object>>.Count => fapKeyValues.Count;
        #region custom
        public bool ContainsKey(string key)
        {
            return fapKeyValues.ContainsKey(key);
        }
        public ICollection<string> Keys => fapKeyValues.Keys;
        #endregion
        public object Get(string key)
        {
            TryGetValue(key, out object value);
            return value;
        }

        public bool TryGetValue(string key, out object value)
        {
            if (fapKeyValues.TryGetValue(key, out object v))
            {
                value = v;
            }
            else
            {
                value = null;
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder().Append($"{{{nameof(FapDynamicObject)}}}");
            foreach (var kv in this)
            {
                var value = kv.Value;
                sb.Append(", ").Append(kv.Key);
                if (value != null)
                {
                    sb.Append(" = '").Append(kv.Value).Append('\'');
                }
                else
                {
                    sb.Append(" = NULL");
                }
            }

            return sb.Append('}').ToString();
        }

        System.Dynamic.DynamicMetaObject System.Dynamic.IDynamicMetaObjectProvider.GetMetaObject(
            System.Linq.Expressions.Expression parameter)
        {
            return new FapRowDataMetaObject(parameter, System.Dynamic.BindingRestrictions.Empty, this);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var entry in fapKeyValues)
            {
                yield return new KeyValuePair<string, object>(entry.Key, entry.Value);
            }

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region Implementation of ICollection<KeyValuePair<string,object>>

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            IDictionary<string, object> dic = this;
            dic.Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            fapKeyValues.Clear();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return TryGetValue(item.Key, out object value) && Equals(value, item.Value);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            foreach (var kv in this)
            {
                array[arrayIndex++] = kv; // if they didn't leave enough space; not our fault
            }
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            IDictionary<string, object> dic = this;
            return dic.Remove(item.Key);
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => false;
        #endregion

        #region Implementation of IDictionary<string,object>

        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return fapKeyValues.ContainsKey(key);
        }

        void IDictionary<string, object>.Add(string key, object value)
        {
            SetValue(key, value, true);
        }

        bool IDictionary<string, object>.Remove(string key)
        {
            return fapKeyValues.Remove(key);
        }

        object IDictionary<string, object>.this[string key]
        {
            get { TryGetValue(key, out object val); return val; }
            set { SetValue(key, value, false); }
        }

        public  object SetValue(string key, object value)
        {
            return SetValue(key, value, false);
        }
        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="isAdd"></param>
        /// <param name="language">多语言</param>
        /// <returns></returns>
        private object SetValue(string key, object value, bool isAdd)
        {
            Guard.Against.Null(key, nameof(key));
            
            if (allColNames.Contains(key))
            {
                if (fapKeyValues.ContainsKey(key) && isAdd)
                {
                    // then semantically, this value already exists
                    throw new ArgumentException("An item with the same key has already been added", nameof(key));
                }
                if (!fapKeyValues.TryAdd(key, value))
                {
                    fapKeyValues[key] = value;
                }
                return value;
            }
            else
            {
                return null;
                //throw new ArgumentException("key 非法，不包含在元数据中", nameof(key));
            }
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get { return this.Select(kv => kv.Key).ToArray(); }
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get { return this.Select(kv => kv.Value).ToArray(); }
        }

        #endregion


        #region Implementation of IReadOnlyDictionary<string,object>


        int IReadOnlyCollection<KeyValuePair<string, object>>.Count => fapKeyValues.Count;


        bool IReadOnlyDictionary<string, object>.ContainsKey(string key)
        {
            return fapKeyValues.ContainsKey(key);
        }

        object IReadOnlyDictionary<string, object>.this[string key]
        {
            get { TryGetValue(key, out object val); return val; }
        }

        IEnumerable<string> IReadOnlyDictionary<string, object>.Keys
        {
            get { return this.Select(kv => kv.Key); }
        }

        IEnumerable<object> IReadOnlyDictionary<string, object>.Values
        {
            get { return this.Select(kv => kv.Value); }
        }

        #endregion

    }

}
