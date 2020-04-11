using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Workflow.Engine.Enums
{
    /// <summary>
    /// 处理人选择模式
    /// </summary>
    public enum ParticipantSelectModeEnum
    {
        /// <summary>
        /// 不选择执行人，则执行人由流程定义时确认。如果定义了多人，则顺序审批
        /// </summary>
        [Description("NoSelect")]
        NoSelect,
        /// <summary>
        /// 由上一步节点选择执行人。流程定义时定义其人员范围。
        /// </summary>
        [Description("NeedSelect")]
        NeedSelect
    }
}
