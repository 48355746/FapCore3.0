using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Workflow.Engine.Exceptions
{
    /// <summary>
    /// 流程运行时异常类
    /// </summary>
    public class WfRuntimeException : System.ApplicationException
    {
        public WfRuntimeException(string message)
            : base(message)
        {
        }

        public WfRuntimeException(string message, System.Exception ex)
            : base(message, ex)
        {

        }
    }
}
