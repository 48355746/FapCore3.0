using Fap.Workflow.Engine.Xpdl.Entity;
using Fap.Workflow.Model;

namespace Fap.Workflow.Engine.Xpdl.Node
{
    /// <summary>
    /// 节点的基类
    /// </summary>
    public abstract class NodeBase
    {
        #region 属性和构造函数
        /// <summary>
        /// 节点定义属性
        /// </summary>
        public ActivityEntity Activity
        {
            get;
            set;
        }

        /// <summary>
        /// 节点实例
        /// </summary>
        public WfActivityInstance ActivityInstance
        {
            get;
            set;
        }
        

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="currentActivity"></param>
        public NodeBase(ActivityEntity currentActivity)
        {
            Activity = currentActivity;
        }
        #endregion
    }
}
