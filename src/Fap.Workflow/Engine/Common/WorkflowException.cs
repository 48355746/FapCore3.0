using System;
using System.Runtime.Serialization;

namespace Fap.Workflow.Engine.Common
{
    /// <summary>
    /// 流程运行异常类
    /// </summary>
    public class WorkflowException : ApplicationException
    {
        public WorkflowException() : base()
        {

        }

        public WorkflowException(string s) : base(s)
        {
        }

        public WorkflowException(string s, Exception e) : base(s, e)
        {
            
        }

        public WorkflowException(Type type, Exception e) : base(e.Message, e)
        {

        }

        protected WorkflowException(SerializationInfo info, StreamingContext cxt) : base(info, cxt)
        {

        }
    }
}
