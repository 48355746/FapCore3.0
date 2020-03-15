using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
/* ==============================================================================
* 功能描述：  
* 创 建 者：wyf
* 创建日期：2016/5/17 11:32:49
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Hcm.Service.Time
{
    /// <summary>
    /// 休息日
    /// </summary>
    [Serializable]
    public class TmHoliday : BaseModel
    {
        /// <summary>
        /// 日期
        /// </summary>
        public string Holiday { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string HodidayNote { get; set; }
        /// <summary>
        /// 假日套
        /// </summary>
        public string CaseUid { get; set; }
        /// <summary>
        /// 假日套 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string CaseUidMC { get; set; }

    }

}
