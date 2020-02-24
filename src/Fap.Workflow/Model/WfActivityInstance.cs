using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 流程活动实例
    /// </summary>
    public class WfActivityInstance : BaseModel
    {
        /// <summary>
        /// 流程实例
        /// </summary>
        public string ProcessInsUid { get; set; }
        /// <summary>
        /// 所属流程
        /// </summary>
        public string ProcessUid { get; set; }
        /// <summary>
        /// 所属流程 的显性字段MC
        /// </summary>
        [Computed]
        public string ProcessUidMC { get; set; }
        /// <summary>
        /// 活动节点Id
        /// </summary>
        public string NodeId { get; set; }
        /// <summary>
        /// 活动节点名称
        /// </summary>
        public string NodeName { get; set; }
        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 活动类型
        /// </summary>
        public string ActivityType { get; set; }
        /// <summary>
        /// 审批方式
        /// </summary>
        public string ApproverMethod { get; set; }
        /// <summary>
        /// 审批方式 的显性字段MC
        /// </summary>
        [Computed]
        public string ApproverMethodMC { get; set; }
        /// <summary>
        /// 活动状态
        /// </summary>
        public string ActivityState { get; set; }
        [Computed]
        public string ActivityStateMC { get; set; }
        /// <summary>
        /// 执行人
        /// </summary>
        public string AssignedToUserIds { get; set; }
        /// <summary>
        /// 执行人名称
        /// </summary>
        public string AssignedToUserNames { get; set; }
        /// <summary>
        /// 当前执行人名称
        /// </summary>
        public string CurrApproverEmpName { get; set; }
        /// <summary>
        /// 当前执行人
        /// </summary>
        public string CurrApproverEmpUid { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 方向类型
        /// </summary>
        public string DirectionType { get; set; }
        /// <summary>
        /// 路由类型
        /// </summary>
        public string RouteType { get; set; }
        /// <summary>
        /// 能否重启实例
        /// </summary>
        public int CanRenewInstance { get; set; }
        /// <summary>
        /// 是否需要令牌
        /// </summary>
        public int TokensRequired { get; set; }
        /// <summary>
        /// 令牌数
        /// </summary>
        public int TokensHad { get; set; }
        /// <summary>
        /// 附属类型
        /// </summary>
        public string ComplexType { get; set; }
        /// <summary>
        /// 主活动Id
        /// </summary>
        public string MainActivityId { get; set; }
        /// <summary>
        /// 完成顺序
        /// </summary>
        public int CompleteOrder { get; set; }
        /// <summary>
        /// 加签类型
        /// </summary>
        public string SignForwardType { get; set; }
        /// <summary>
        /// 会签节点合并类型
        /// </summary>
        public string MergeType { get; set; }
        /// <summary>
        /// 退回类型
        /// </summary>
        public string BackwardType { get; set; }
        /// <summary>
        /// 回退时源活动Id
        /// </summary>
        public string BackSrcActivityId { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// bill uid
        /// </summary>
        public string BillUid { get; set; }
        /// <summary>
        /// 业务
        /// </summary>
        public string BusinessUid { get; set; }
        /// <summary>
        /// 所属业务 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string BizTypeIdMC { get; set; }
        /// <summary>
        /// 流程发起人
        /// </summary>
        public string AppEmpUid { get; set; }
        /// <summary>
        /// 流程发起人 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string AppEmpUidMC { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        public string ProcessState { get; set; }
        /// <summary>
        /// 流程状态 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ProcessStateMC { get; set; }
        /// <summary>
        /// 流程发起时间
        /// </summary>
        public string AppStartTime { get; set; }
        /// <summary>
        /// 审批状态
        /// </summary>
        public string ApproveState { get; set; }
        /// <summary>
        /// 审批状态 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ApproveStateMC { get; set; }
        /// <summary>
        /// 表单模板
        /// </summary>
        public string FormTemplate { get; set; }
        /// <summary>
        /// 表单权限
        /// </summary>
        public string FormPower { get; set; }

        /// <summary>
        /// 会签通过率
        /// </summary>
        public int PassingRate { get; set; }
        /// <summary>
        /// 自动执行时间(时间任务节点)
        /// </summary>
        public string AutoExecTime { get; set; }
        /// <summary>
        /// 消息设置
        /// </summary>
        public string MessageSetting { get; set; }

    }

}
