using Fap.Workflow.Engine.Enums;

namespace Fap.Workflow.Engine.Xpdl
{
    /// <summary>
    /// 接收者
    /// </summary>
    public class Receiver
    {
        /// <summary>
        /// 接收者类型
        /// </summary>
        public ReceiverTypeEnum ReceiverType
        {
            get;
            set;
        }

        /// <summary>
        /// 数目限制
        /// </summary>
        public int Candidates
        {
            get;
            set;
        }
    }
}
