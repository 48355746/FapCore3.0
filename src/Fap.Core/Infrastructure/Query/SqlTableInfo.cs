using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Query
{
    /// <summary>
    /// 查询SQL中的表信息
    /// </summary>
    public class SqlTableInfo
    {
        private string _tableName = string.Empty;
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get { return _tableName; } set { _tableName = value; } }

        private string _tableAlias = string.Empty;
        /// <summary>
        /// 表别名
        /// </summary>
        public string TableAlias { get { return _tableAlias; } set { _tableAlias = value; } }
    }
}
