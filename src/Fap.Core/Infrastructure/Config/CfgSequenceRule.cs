using Fap.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Config
{
    /// <summary>
    /// 自增号规则
    /// </summary>
    [Serializable]
    public class CfgSequenceRule : BaseModel
    {
        /// <summary>
        /// 唯一标记名称
        /// </summary>
        public string SeqName { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public int MinValue { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        public int MaxValue { get; set; }
        /// <summary>
        /// 步长
        /// </summary>
        public int StepBy { get; set; }
        /// <summary>
        /// 当前值
        /// </summary>
        public int CurrValue { get; set; }

    }

}
