using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Event;
using Fap.Workflow.Engine.Exceptions;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Engine.Xpdl.Node;
using Fap.Workflow.Model;
using Microsoft.Extensions.Logging;
using System;

namespace Fap.Workflow.Engine.Core
{
    /// <summary>
    /// 流程运行时管理
    /// </summary>
    internal abstract class WfRuntimeManager
    {
        #region 抽象方法
        internal abstract void ExecuteInstanceImp();
        #endregion

        #region 流转属性和基础方法
        internal WfAppRunner AppRunner { get; set; }
        internal IDbContext _dataAccessor { get; set; }
        internal ILoggerFactory _loggerFactory { get; set; }
        internal IProcessModel ProcessModel { get; set; }
        internal WfProcessInstance ParentProcessInstance { get; set; }
        internal NodeBase InvokedSubProcessNode { get; set; }
        //internal ActivityResource ActivityResource { get; set; }
        //internal TaskViewEntity TaskView { get; set; }
        internal WfActivityInstance RunningActivityInstance { get; set; }

        //流程返签或退回时的属性
        internal BackwardContext BackwardContext { get; set; }
        internal Boolean IsBackward { get; set; }
        internal WfTask TaskView { get; set; }
        /// <summary>
        /// 流程执行结果对象
        /// </summary>
        internal WfExecutedResult WfExecutedResult { get; set; }

        /// <summary>
        /// 获取退回时最早节点实例ID，支持连续退回
        /// </summary>
        /// <returns></returns>
        protected string GetBackwardMostPreviouslyActivityInstanceID()
        {
            //获取退回节点实例ID
            string backMostPreviouslyActivityInstanceID;
            if (BackwardContext.BackwardToTaskActivityInstance.Fid != null)
                backMostPreviouslyActivityInstanceID = BackwardContext.BackwardToTaskActivityInstance.Fid;
            else
                backMostPreviouslyActivityInstanceID = BackwardContext.BackwardToTaskActivityInstance.Fid;

            return backMostPreviouslyActivityInstanceID;
        }
        #endregion

        #region 构造方法
        internal WfRuntimeManager(IDbContext dbContext,ILoggerFactory loggerFactory)
        {
            _dataAccessor = dbContext;
            _loggerFactory = loggerFactory;
            BackwardContext = new BackwardContext();
        }
        #endregion

        #region 执行方法
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <returns></returns>
        internal bool Execute()
        {
            try
            {
                ExecuteInstanceImp();
            }
            catch (WfRuntimeException rx)
            {
                throw rx;
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                Callback(WfExecutedResult);
            }

            return true;
        }

        /// <summary>
        /// 事件回调
        /// </summary>
        /// <param name="runtimeType"></param>
        /// <param name="result"></param>
        internal void Callback(WfExecutedResult result)
        {
            WfEventArgs args = new WfEventArgs(result);
            if (_onWfProcessExecuted != null)
            {
                _onWfProcessExecuted(this, args);
            }
        }
        #endregion

        #region 流程事件定义
        private event EventHandler<WfEventArgs> _onWfProcessExecuted;
        internal event EventHandler<WfEventArgs> OnWfProcessExecuted
        {
            add
            {
                _onWfProcessExecuted += value;
            }
            remove
            {
                _onWfProcessExecuted -= value;
            }
        }
        #endregion
    }
}
