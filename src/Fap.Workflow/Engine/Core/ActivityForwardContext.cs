using Fap.Workflow.Engine.Manager;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Engine.Xpdl.Entity;
using Fap.Workflow.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Workflow.Engine.Core
{
    /// <summary>
    /// 活动节点执行上下文对象
    /// </summary>
    internal class ActivityForwardContext
    {
        #region ActivityForwardContext 属性列表

        internal IProcessModel ProcessModel { get; set; }
        internal WfProcessInstance ProcessInstance { get; set; }
        internal ActivityEntity Activity { get; set; }
        internal WfActivityInstance FromActivityInstance { get; set; }
        internal WfTask TaskView { get; set; }

        #endregion

        #region ActivityForwardContext 构造函数
        /// <summary>
        /// 开始节点的构造执行上下文对象
        /// </summary>
        /// <param name="processModel"></param>
        /// <param name="processInstance"></param>
        /// <param name="activity"></param>
        /// <param name="activityResource"></param>
        private ActivityForwardContext(IProcessModel processModel,
            WfProcessInstance processInstance,
            ActivityEntity activity)
        {
            ProcessModel = processModel;
            ProcessInstance = processInstance;
            Activity = activity;
        }

        /// <summary>
        /// 任务执行的上下文对象
        /// </summary>
        /// <param name="task"></param>
        /// <param name="processModel"></param>
        /// <param name="activityResource"></param>
        private ActivityForwardContext(WfTask task,
            IProcessModel processModel, WfProcessInstance processInstance,WfActivityInstance fromActivityInstance)
        {
            this.TaskView = task;

            //check task condition has load activity instance
            this.FromActivityInstance = fromActivityInstance ;
            this.ProcessInstance = processInstance;
            this.Activity = processModel.GetActivity(task.NodeId);
            this.ProcessModel = processModel;
        }

        /// <summary>
        /// 启动流程的上下文对象
        /// </summary>
        /// <param name="processModel"></param>
        /// <param name="processInstance"></param>
        /// <param name="activity"></param>
        /// <param name="activityResource"></param>
        /// <returns></returns>
        internal static ActivityForwardContext CreateStartupContext(IProcessModel processModel,
            WfProcessInstance processInstance,
            ActivityEntity activity)
        {
            return new ActivityForwardContext(processModel, processInstance, activity);
        }

        /// <summary>
        /// 创建任务执行上下文对象
        /// </summary>
        /// <param name="task"></param>
        /// <param name="processModel"></param>
        /// <param name="activityResource"></param>
        /// <returns></returns>
        internal static ActivityForwardContext CreateRunningContext(WfTask task,
            IProcessModel processModel, WfProcessInstance processInstance, WfActivityInstance fromActivityInstance)
        {
            return new ActivityForwardContext(task, processModel,processInstance, fromActivityInstance);
        }

        /// <summary>
        /// 创建流程跳转上下文对象
        /// </summary>
        /// <param name="jumpforwardActivity"></param>
        /// <param name="processModel"></param>
        /// <param name="processInstance"></param>
        /// <param name="activityResource"></param>
        /// <returns></returns>
        internal static ActivityForwardContext CreateJumpforwardContext(ActivityEntity jumpforwardActivity,
            IProcessModel processModel,
            WfProcessInstance processInstance)
        {
            return new ActivityForwardContext(processModel, processInstance, jumpforwardActivity);
        }

        #endregion
    }
}
