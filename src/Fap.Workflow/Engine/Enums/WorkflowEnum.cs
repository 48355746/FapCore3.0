using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Workflow.Engine.Enums
{
    /// <summary>
    /// 流程启动类型
    /// </summary>
    public enum EnumWorkflowStartType
    {
        [Description("流程发起")]
        Startup,
        [Description("流程处理")]
        Process,
        [Description("流程重新提交")]
        ProcessResubmit,
        [Description("流程查看")]
        View
    }

    /// <summary>
    /// 流程审批意见
    /// </summary>
    public enum EnumWorkflowApproveState
    {
        [Description("同意")]
        Agree,
        [Description("不同意")]
        Disagree,
        [Description("已阅")]
        Seen,
    }

    /// <summary>
    /// 表单类型
    /// </summary>
    public enum EnumFormType
    {
        [Description("外挂表单")]
        AddonForm,
        //[Description("自由表单")]
        //FreeForm,
        [Description("无表单")]
        NoneForm,
    }

    /// <summary>
    /// 外挂表单类型
    /// </summary>
    public enum EnumAddonType
    {
        [Description("系统表单")]
        Internal,
        [Description("自由表单")]
        FreeForm
    }
}
