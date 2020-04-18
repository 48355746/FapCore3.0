using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Assess
{
    /// <summary>
    /// 打分方式
    /// </summary>
    public class PerfScoreModel : BaseModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string SmName { get; set; }
        /// <summary>
        /// 是否下拉选择
        /// </summary>
        public int IsChoose { get; set; }
        /// <summary>
        /// 最大分
        /// </summary>
        public double MaxScore { get; set; }
        /// <summary>
        /// 最低分
        /// </summary>
        public double MinScore { get; set; }
        /// <summary>
        /// 下拉选择项设置
        /// </summary>
        public string ChooseSource { get; set; }

    }

}
