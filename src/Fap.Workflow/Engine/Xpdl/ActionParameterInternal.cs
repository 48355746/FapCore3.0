using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Workflow.Engine.Xpdl
{
    /// <summary>
    /// 调用外部API的参数封装类
    /// </summary>
    public class ActionParameterInternal
    {
        public object[] ConstructorParameters { get; set; }
        public object[] MethodParameters { get; set; }
    }
}
