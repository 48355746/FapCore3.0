using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 流程业务表
    /// </summary>
    [Serializable]
    public class WfBusiness :BaseModel
    {
        /// <summary>
        /// 业务分类
        /// </summary>
        public string BizTypeUid { get; set; }
        /// <summary>
        /// 业务分类 的显性字段MC
        /// </summary>
        [Computed]
        public string BizTypeUidMC { get; set; }
        /// <summary>
        /// 单据实体
        /// </summary>
        public string DocEntityUid { get; set; }
        /// <summary>
        /// 单据实体 的显性字段MC
        /// </summary>
        [Computed]
        public string DocEntityUidMC { get; set; }
        /// <summary>
        /// 主业务实体
        /// </summary>
        public string BizEntityUid { get; set; }
        /// <summary>
        /// 主业务实体 的显性字段MC
        /// </summary>
        [Computed]
        public string BizEntityUidMC { get; set; }
        /// <summary>
        /// 所属流程
        /// </summary>
        public string WfProcessUid { get; set; }
        /// <summary>
        /// 所属流程 的显性字段MC
        /// </summary>
        [Computed]
        public string WfProcessUidMC { get; set; }
        /// <summary>
        /// 业务表单URL
        /// </summary>
        public string BizFormURL { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int BizStatus { get; set; }
        /// <summary>
        /// 表单类型
        /// </summary>
        public string BizFormType { get; set; }
        /// <summary>
        /// 表单类型 的显性字段MC
        /// </summary>
        [Computed]
        public string BizFormTypeMC { get; set; }


    }
}
