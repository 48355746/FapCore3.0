using System.Collections.Generic;

namespace Fap.Workflow.Engine.Xpdl.Entity
{
    /// <summary>
    /// 任务的执行者对象
    /// </summary>
    public class Performer
    {

        public string UserId
        {
            get;
            set;
        }

        public string UserName
        {
            get;
            set;
        }
    }

    public class PerformerList : List<Performer>
    {
        public PerformerList()
        {
        }
    }
}
