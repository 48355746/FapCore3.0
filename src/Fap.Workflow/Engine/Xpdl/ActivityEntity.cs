using System.Collections.Generic;
using Fap.Workflow.Engine.Xpdl.Node;
using Fap.Workflow.Engine.Xpdl.Entity;
using Fap.Workflow.Engine.Enums;

namespace Fap.Workflow.Engine.Xpdl
{
    /// <summary>
    /// 活动节点属性定义
    /// </summary>
    public class ActivityEntity
    {
        /// <summary>
        /// 活动GUID
        /// </summary>
        public string ActivityID { get; set; }

        /// <summary>
        /// 流程GUID
        /// </summary>
        public string ProcessUid { get; set; }

        /// <summary>
        /// 活动类型
        /// </summary>
        public ActivityTypeEnum ActivityType { get; set; }
        /// <summary>
        /// 审批方式
        /// </summary>
        public ApproverMethodEnum ApproverMethod { get; set; }
        /// <summary>
        /// 工作项类型
        /// </summary>
        public WorkItemTypeEnum WorkItemType { get; set; }

        /// <summary>
        /// 活动类型Detail
        /// </summary>
        public ActivityTypeDetail ActivityTypeDetail { get; set; }

        /// <summary>
        /// 节点
        /// </summary>
        public NodeBase Node { get; set; }

        /// <summary>
        /// 网关分支合并类型
        /// </summary>
        public GatewaySplitJoinTypeEnum GatewaySplitJoinType { get; set; }

        /// <summary>
        /// 网关方向类型
        /// </summary>
        public GatewayDirectionEnum GatewayDirectionType { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// 活动代码
        /// </summary>
        public string ActivityCode { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 操作列表
        /// </summary>
        public List<ActionEntity> ActionList { get; set; }

        //custom
        /// <summary>
        /// 通过率
        /// </summary>
        public int PassRate { get; set; }
        /// <summary>
        /// 超期时间（时间任务）
        /// </summary>
        public int Expiration { get; set; }
        /// <summary>
        /// 是否由上级指定审批人
        /// </summary>
        public bool IsAppoint { get; set; }
        /// <summary>
        /// 通知审批人
        /// </summary>
        public bool NoticeApprover { get; set; }
        /// <summary>
        /// 通知申请人
        /// </summary>
        public bool NoticeApplicant { get; set; }
        /// <summary>
        /// 邮件
        /// </summary>
        public bool IsMail { get; set; }
        /// <summary>
        /// 站内信
        /// </summary>
        public bool IsMessage { get; set; }
        /// <summary>
        /// 日历
        /// </summary>
        public bool IsCalendar { get; set; }
        /// <summary>
        /// 单据模板
        /// </summary>
        public string BillTemplate { get; set; }
        /// <summary>
        /// 字段权限
        /// </summary>
        public IList<FieldEntity> FieldItems{get;set;}
        /// <summary>
        /// 处理人角色
        /// </summary>
        public IList<Participant> Participant { get; set; }
        /// <summary>
        /// 处理人
        /// </summary>
        public IList<Performer> Performers { get; set; }
        /// <summary>
        /// 子流程
        /// </summary>
        public string SubProcess { get; set; }
    }
}
