using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.AspNetCore.Model
{
    public class DataResultView
    {

        /// <summary>
        /// 表的字段元数据，不仅支持单表， 也可以支持多表
        /// </summary>
        //public IEnumerable<FapColumn> ColumnList { get; set; }

        /// <summary>
        /// 查询结果数据集合（动态对象）
        /// </summary>
        public IEnumerable<FapDynamicObject> Data { get; set; }

        /// <summary>
        /// 查询结果数据集合的JSON字符串
        /// </summary>
        public string DataJson { get; set; }

        /// <summary>
        /// 查询初始化下的数据（即只含默认值）
        /// </summary>
        public dynamic DefaultData { get; set; }

        /// <summary>
        /// 子表数据
        /// </summary>
        public Dictionary<string, List<dynamic>> SubTableData { get; set; }

        /// <summary>
        /// 输出的SQL语句的
        /// </summary>
        public string OutputSqlLog { get; set; }

        /// <summary>
        /// 统计字段数据， 比如：select '总计:' + cast(SUM(Id) as varchar(100)) as Id   from FapTable
        /// </summary>
        public dynamic StatFieldData { get; set; }
        /// <summary>
        /// 统计字段数据JSON
        /// </summary>
        public string StatFieldDataJson { get; set; }
    }
}
