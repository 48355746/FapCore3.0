using System;

namespace Fap.Workflow.Engine.Exceptions
{
    public class ProcessInstanceException : System.ApplicationException
    {
        public ProcessInstanceException(string message)
            : base(message)
        {
        }


        public ProcessInstanceException(string message, Exception ex)
            : base(message, ex)
        {

        }
    }
}
