using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 流程代理人设置
    /// </summary>
    public class WfAgentSetting : BaseModel
    {
        /// <summary>
		/// 流程业务
		/// </summary>
		public string BusinessUid { get; set; }
        /// <summary>
        /// 流程业务 的显性字段MC
        /// </summary>
        [Computed]
        public string BusinessUidMC { get; set; }
        /// <summary>
        /// 被代理人
        /// </summary>
        public string Principal { get; set; }
        /// <summary>
        /// 被代理人 的显性字段MC
        /// </summary>
        [Computed]
        public string PrincipalMC { get; set; }
        /// <summary>
        /// 代理人
        /// </summary>
        public string Agent { get; set; }
        /// <summary>
        /// 代理人 的显性字段MC
        /// </summary>
        [Computed]
        public string AgentMC { get; set; }
        /// <summary>
        /// 启用状态
        /// </summary>
        public int State { get; set; }

    }
}
