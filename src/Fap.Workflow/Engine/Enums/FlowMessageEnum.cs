using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Fap.Workflow.Engine.Enums
{
    public enum FlowMessageEnum
    {
        [Description("通知审批人")]
        NoticeApprover,
        [Description("完成通知申请人")]
        CompleteNoticeApplier,


    }
}
