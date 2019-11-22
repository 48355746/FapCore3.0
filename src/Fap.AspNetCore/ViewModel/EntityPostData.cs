/* ==============================================================================
 * 功能描述：查询任意条件的实体  
 * 创 建 者：wyf
 * 创建日期：2017-06-22 10:57:12
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.AspNetCore.ViewModel
{
    /// <summary>
    /// 实体PostData
    /// </summary>
    public class EntityPostData
    {
        /// <summary>
        /// 实体名称（tableName）
        /// </summary>
        public string EntityName { get; set; }
        /// <summary>
        /// 过滤条件(jqgrid标准的条件格式)
        /// </summary>
        public string Filters { get; set; }
        /// <summary>
        /// 查询列
        /// </summary>
        public string QueryCols { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public string SortBy { get; set; }
    }
}
