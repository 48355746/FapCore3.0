using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Query
{
    /// <summary>
    /// 通用分页结果类 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageInfo<T>
    {
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages
        {
            get
            {
                return TotalSizes == 0 ? 1 : (int)Math.Ceiling(TotalSizes / (double)PageSize);
            }
        }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalSizes { get; set; }
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item;
            }
        }
        /// <summary>
        /// 页记录数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        /// 最大Id的值
        /// </summary>
        public long MaxIdValue { get; set; }

        /// <summary>
        /// 统计字段数据， 比如：select '总计:' + cast(SUM(Id) as varchar(100)) as Id   from FapTable
        /// </summary>
        public dynamic StatFieldData { get; set; }
      
    }
}
