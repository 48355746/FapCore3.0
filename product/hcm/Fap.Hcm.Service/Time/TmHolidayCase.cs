/* ==============================================================================
 * 功能描述：  
 * 创 建 者：wyf
 * 创建日期：2016/5/17 11:32:16
 * ==============================================================================*/
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Hcm.Service.Time
{
    /// <summary>
    /// 休息日套
    /// </summary>
    public class TmHolidayCase : BaseModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string CaseCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string CaseName { get; set; }
        /// <summary>
        /// 分组
        /// </summary>
        public string CaseGroup { get; set; }
        /// <summary>
        /// 共享
        /// </summary>
        public int IsShare { get; set; }

    }

}
