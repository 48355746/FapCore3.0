using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Workflow.Engine.Node
{
    /// <summary>
    /// 回退返回的参数类
    /// </summary>
    public class ReturnDataContext
    {
        public string ActivityInstanceUid { get; set; }
        public string ProcessInstanceUid { get; set; }
    }
}
