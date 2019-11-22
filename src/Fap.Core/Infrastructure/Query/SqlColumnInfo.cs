using Fap.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Query
{
    /// <summary>
    /// SQL中字段信息
    /// </summary>
    public class SqlColumnInfo
    {
        /// <summary>
        /// 表
        /// </summary>
        public SqlTableInfo TableInfo { get; set; }

        public string _tableAlias = string.Empty;
        /// <summary>
        /// 表别名
        /// </summary>
        public string TableAlias { get { return _tableAlias; } set { _tableAlias = value; } }

        public string _columnName = string.Empty;
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get { return _columnName; } set { _columnName = value; } }

        public string _columnAlias = string.Empty;
        /// <summary>
        /// 列表名
        /// </summary>
        public string ColumnAlias { get { return _columnAlias; } set { _columnAlias = value; } }

        public bool _isExpired = false;
        /// <summary>
        /// 是否作废
        /// </summary>
        public bool IsExpired { get { return _isExpired; } set { _isExpired = value; } }

        public bool _isMulti = false;
        /// <summary>
        /// 是否多选
        /// </summary>
        public bool IsMulti { get { return _isMulti; } set { _isMulti = value; } }
        /// <summary>
        /// 相对应的FapColumn
        /// </summary>
        public FapColumn ColumnInCorrespond { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int ColumnOrder { get; set; }

        public bool _isExpression = false;
        /// <summary>
        /// 是否表达式，
        /// 比如：(select Id from tableName) AS fieldMC 是表达式
        /// 比如：a.name AS name 不是表达式
        /// </summary>
        public bool IsExpression { get { return _isExpression; } set { _isExpression = value; } }

        public string _orginSql = string.Empty;
        /// <summary>
        /// 原始的SQL语句
        /// </summary>
        public string OrginSql { get { return _orginSql; } set { _orginSql = value; } }
    }
}
