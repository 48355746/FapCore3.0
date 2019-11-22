using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Query
{
    /// <summary>
    /// 查询数据的打包对象，用于缓存
    /// </summary>
    [Serializable]
    public class QueryDataObject
    {
        /// <summary>
        /// 原始的结果数据集合（dappermap）
        /// </summary>
        [NonSerialized]
        private IEnumerable<dynamic> _orgin_data;
        public IEnumerable<dynamic> OrginData { set { _orgin_data = value; } get { return _orgin_data; } }
        /// <summary>
        /// 查询的数据集合
        /// </summary>
        [NonSerialized]
        private IEnumerable<dynamic> _data;
        /// <summary>
        /// 查询结果数据集合（动态对象）
        /// </summary>
        public IEnumerable<dynamic> Data { set { _data = value; } get { return _data; } }
        /// <summary>
        /// 查询结果数据集合的JSON字符串
        /// </summary>
        public string DataJson { set; get; }

        /// <summary>
        /// 查询结果数据集合(符合jqGrid)
        /// </summary>
        public IEnumerable<dynamic> DataForJqGrid { get; set; }
        /// <summary>
        /// 数据的总记录数
        /// </summary>
        public int TotalCount { set; get; }
    }
}
