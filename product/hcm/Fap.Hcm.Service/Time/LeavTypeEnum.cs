using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Fap.Hcm.Service.Time
{
    /// <summary>
    /// 请假类型
    /// </summary>
    public enum LeavTypeEnum
    {
        [Description("事假")]
        Business,
        [Description("病假")]
        Sick,
        [Description("年假")]
        Annaul,
        [Description("婚假")]
        Wed,
        [Description("产假")]
        Maternity,
        [Description("哺乳假")]
        Nursing,
        [Description("丧假")]
        Funeral,
        [Description("调休")]
        Tuneoff
    }
}
