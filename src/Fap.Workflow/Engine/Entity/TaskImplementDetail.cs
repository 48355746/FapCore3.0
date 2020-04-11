using Fap.Workflow.Engine.Enums;

namespace Fap.Workflow.Engine.Xpdl.Entity
{
    public class TaskImplementDetail
    {
        /// <summary>
        /// 任务实现类型定义
        /// </summary>
        public ImplementationTypeEnum ImplementationType
        {
            get;
            set;
        }

        public string Assembly
        {
            get;
            set;
        }

        public string Interface
        {
            get;
            set;
        }

        public string Method
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }
    }
}
